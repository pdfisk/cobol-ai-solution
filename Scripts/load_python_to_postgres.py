#!/usr/bin/env python3
"""
Load Python source files from ServerApp/data/python into the PostgreSQL
python_source_files table. Uses UPSERT so re-running updates existing rows.
Usage: python load_python_to_postgres.py
"""

import psycopg2
from pathlib import Path

DB_NAME = "cobol_studio"
DB_USER = "admin"
DB_PASSWORD = "chichi"
DB_HOST = "localhost"
DB_PORT = 5432


def get_python_dir() -> Path:
    """Path to ServerApp/data/python relative to this script."""
    script_dir = Path(__file__).resolve().parent
    repo_root = script_dir.parent
    return repo_root / "ServerApp" / "data" / "python"


def load_python_files(conn) -> int:
    """Load .py files from ServerApp/data/python into python_source_files. Returns count loaded."""
    python_dir = get_python_dir()
    if not python_dir.is_dir():
        raise FileNotFoundError(f"Python directory not found: {python_dir}")

    cur = conn.cursor()
    count = 0
    for path in sorted(python_dir.glob("*.py")):
        file_name = path.name
        content = path.read_text(encoding="utf-8", errors="replace")
        cur.execute(
            """
            INSERT INTO python_source_files (file_name, content, updated_at)
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
    python_dir = get_python_dir()
    print(f"Python directory: {python_dir}")
    conn = psycopg2.connect(**conn_params)
    try:
        count = load_python_files(conn)
        print(f"Loaded {count} file(s) into python_source_files.")
    finally:
        conn.close()


if __name__ == "__main__":
    main()
