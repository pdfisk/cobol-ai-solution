#!/usr/bin/env python3
"""
Copy all user tables from the local PostgreSQL database to the remote database.

Local connection matches local_db.ps1:
  host=localhost, port=5432, user=admin, db=cobol_studio, password=chichi

Remote connection matches remote_db.ps1:
  host=ca932070ke6bv1.cluster-czrs8kj4isg7.us-east-1.rds.amazonaws.com,
  port=5432, user=u8if3a0emvt160, db=d40dlqsc2cuu2l,
  password=p6e59ccd597838ffebe1fd36ad6eeadbd5451c2e0a99b8f4f08d25265fb20d04c

The script:
- Enumerates all non-system tables in the local database
  (schemas other than pg_catalog and information_schema)
- For each table:
  - Ensures the schema exists on the remote database
  - Truncates the remote table if it already exists
  - Copies all rows from local -> remote with a simple INSERT

Note: This assumes the table structures (columns/types) already match on the
remote database. It does not attempt to recreate or migrate schema differences.
"""

from __future__ import annotations

import sys
from typing import List, Tuple

import psycopg2
from psycopg2 import sql


LOCAL_DB = {
    "dbname": "cobol_studio",
    "user": "admin",
    "password": "chichi",
    "host": "localhost",
    "port": 5432,
}

REMOTE_DB = {
    "dbname": "d40dlqsc2cuu2l",
    "user": "u8if3a0emvt160",
    "password": "p6e59ccd597838ffebe1fd36ad6eeadbd5451c2e0a99b8f4f08d25265fb20d04c",
    "host": "ca932070ke6bv1.cluster-czrs8kj4isg7.us-east-1.rds.amazonaws.com",
    "port": 5432,
}


def get_user_tables(conn) -> List[Tuple[str, str]]:
    """Return list of (schema, table_name) for user tables on the given connection."""
    cur = conn.cursor()
    cur.execute(
        """
        SELECT table_schema, table_name
        FROM information_schema.tables
        WHERE table_type = 'BASE TABLE'
          AND table_schema NOT IN ('pg_catalog', 'information_schema')
        ORDER BY table_schema, table_name;
        """
    )
    rows = cur.fetchall()
    cur.close()
    return [(r[0], r[1]) for r in rows]


def get_table_columns(conn, schema: str, table: str) -> List[str]:
    """Return ordered column names for the specified table."""
    cur = conn.cursor()
    cur.execute(
        """
        SELECT column_name
        FROM information_schema.columns
        WHERE table_schema = %s AND table_name = %s
        ORDER BY ordinal_position;
        """,
        (schema, table),
    )
    cols = [r[0] for r in cur.fetchall()]
    cur.close()
    return cols


def ensure_remote_schema(conn, schema: str) -> None:
    """Create the schema on the remote DB if it does not exist."""
    if schema == "public":
        return
    cur = conn.cursor()
    cur.execute(
        sql.SQL("CREATE SCHEMA IF NOT EXISTS {}").format(sql.Identifier(schema))
    )
    conn.commit()
    cur.close()


def truncate_remote_table(conn, schema: str, table: str) -> None:
    """Truncate the remote table if it exists; ignore errors if it does not."""
    cur = conn.cursor()
    try:
        cur.execute(
            sql.SQL("TRUNCATE TABLE {}.{} RESTART IDENTITY CASCADE")
            .format(sql.Identifier(schema), sql.Identifier(table))
        )
        conn.commit()
    except psycopg2.Error:
        conn.rollback()
    finally:
        cur.close()


def copy_table(local_conn, remote_conn, schema: str, table: str) -> int:
    """Copy all rows for a single table from local -> remote. Returns row count."""
    cols = get_table_columns(local_conn, schema, table)
    if not cols:
        return 0

    ensure_remote_schema(remote_conn, schema)
    truncate_remote_table(remote_conn, schema, table)

    col_idents = [sql.Identifier(c) for c in cols]
    values_placeholders = sql.SQL(", ").join(sql.Placeholder() for _ in cols)

    insert_stmt = sql.SQL("INSERT INTO {}.{} ({}) VALUES ({})").format(
        sql.Identifier(schema),
        sql.Identifier(table),
        sql.SQL(", ").join(col_idents),
        values_placeholders,
    )

    local_cur = local_conn.cursor()
    remote_cur = remote_conn.cursor()
    local_cur.execute(
        sql.SQL("SELECT {} FROM {}.{}").format(
            sql.SQL(", ").join(col_idents),
            sql.Identifier(schema),
            sql.Identifier(table),
        )
    )

    row_count = 0
    while True:
        rows = local_cur.fetchmany(1000)
        if not rows:
            break
        for row in rows:
            remote_cur.execute(insert_stmt, row)
            row_count += 1

    remote_conn.commit()
    local_cur.close()
    remote_cur.close()
    return row_count


def main() -> None:
    print("Connecting to local and remote PostgreSQL databases...")
    try:
        local_conn = psycopg2.connect(**LOCAL_DB)
        remote_conn = psycopg2.connect(**REMOTE_DB)
    except psycopg2.Error as exc:
        print(f"Connection error: {exc}", file=sys.stderr)
        sys.exit(1)

    try:
        tables = get_user_tables(local_conn)
        if not tables:
            print("No user tables found in local database.")
            return

        print("Tables to copy:")
        for schema, table in tables:
            print(f"  {schema}.{table}")

        total_rows = 0
        for schema, table in tables:
            print(f"\nCopying {schema}.{table} ...")
            try:
                copied = copy_table(local_conn, remote_conn, schema, table)
                total_rows += copied
                print(f"  Copied {copied} row(s).")
            except psycopg2.Error as exc:
                print(f"  Error copying {schema}.{table}: {exc}", file=sys.stderr)

        print(f"\nDone. Total rows copied: {total_rows}")
    finally:
        local_conn.close()
        remote_conn.close()


if __name__ == "__main__":
    main()

