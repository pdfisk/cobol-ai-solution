#!/usr/bin/env python3
"""
Load COBOL source files from ServerApp/data/cobol into the PostgreSQL
cobol_source_files table. Uses UPSERT so re-running updates existing rows.
Usage: python load_cobol_to_postgres.py
"""

import psycopg2
from pathlib import Path

DB_NAME = "d40dlqsc2cuu2l"
DB_USER = "u8if3a0emvt160"
DB_PASSWORD = "p6e59ccd597838ffebe1fd36ad6eeadbd5451c2e0a99b8f4f08d25265fb20d04c"
DB_HOST = "ca932070ke6bv1.cluster-czrs8kj4isg7.us-east-1.rds.amazonaws.com"
DB_PORT = 5432



def get_cobol_dir() -> Path:
    """Path to ServerApp/data/cobol relative to this script."""
    script_dir = Path(__file__).resolve().parent
    return script_dir /  "app_data" / "cobol"


def load_cobol_files(conn) -> int:
    """Load .cobol files from ServerApp/data/cobol into cobol_source_files. Returns count loaded."""
    cobol_dir = get_cobol_dir()
    if not cobol_dir.is_dir():
        raise FileNotFoundError(f"COBOL directory not found: {cobol_dir}")

    cur = conn.cursor()
    count = 0
    for path in sorted(cobol_dir.glob("*.cobol")):
        file_name = path.name
        content = path.read_text(encoding="utf-8", errors="replace")
        cur.execute(
            """
            INSERT INTO cobol_source_files (file_name, content, updated_at)
            VALUES (%s, %s, CURRENT_TIMESTAMP)
            ON CONFLICT (file_name) DO UPDATE SET
                content = EXCLUDED.content,
                updated_at = CURRENT_TIMESTAMP;
            """,
            (file_name, content),
        )
        count += 1
        print(f"  {file_name}")

    conn.commit()
    cur.close()
    return count


def main() -> None:
    conn_params = {
        "dbname": DB_NAME,
        "user": DB_USER,
        "password": DB_PASSWORD,
        "host": DB_HOST,
        "port": DB_PORT,
    }
    cobol_dir = get_cobol_dir()
    print(f"COBOL directory: {cobol_dir}")
    conn = psycopg2.connect(**conn_params)
    try:
        count = load_cobol_files(conn)
        print(f"Loaded {count} file(s) into cobol_source_files.")
    finally:
        conn.close()


if __name__ == "__main__":
    print(get_cobol_dir())
    main()