import psycopg2
from pathlib import Path

DB_NAME = "cobol_studio"
DB_USER = "admin"
DB_PASSWORD = "chichi"
DB_HOST = "localhost"
DB_PORT = 5432

REMOTE_DB_NAME = "d40dlqsc2cuu2l"
REMOTE_DB_USER = "u8if3a0emvt160"
REMOTE_DB_PASSWORD = "p6e59ccd597838ffebe1fd36ad6eeadbd5451c2e0a99b8f4f08d25265fb20d04c"
REMOTE_DB_HOST = "ca932070ke6bv1.cluster-czrs8kj4isg7.us-east-1.rds.amazonaws.com"
REMOTE_DB_PORT = 5432

local_conn_params = {
        "dbname": DB_NAME,
        "user": DB_USER,
        "password": DB_PASSWORD,
        "host": DB_HOST,
        "port": DB_PORT
        }
local_conn = psycopg2.connect(**local_conn_params)
remote_conn = psycopg2.connect(**remote_conn_params)

