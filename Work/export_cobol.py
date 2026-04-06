import psycopg2
import os

# Database connection settings from db.txt
DB_CONFIG = {
    "dbname": "cobol_studio",
    "user": "admin",
    "password": "chichi",
    "host": "localhost",
    "port": 5432,
}

TABLE = "cobol_source_files"

# Create data folder if it doesn't exist
data_dir = os.path.join(os.path.dirname(__file__), "data")
os.makedirs(data_dir, exist_ok=True)

conn = psycopg2.connect(**DB_CONFIG)
cur = conn.cursor()

cur.execute(f"SELECT file_name, content FROM {TABLE}")
rows = cur.fetchall()

for file_name, content in rows:
    name = file_name if file_name.endswith(".cobol") else f"{file_name}.cobol"
    output_path = os.path.join(data_dir, name)
    with open(output_path, "w", encoding="utf-8") as f:
        f.write(content or "")
    print(f"Created: {output_path}")

print(f"\nDone. {len(rows)} file(s) exported to '{data_dir}'.")

cur.close()
conn.close()
