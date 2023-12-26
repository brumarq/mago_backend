import os
import pyodbc
from datetime import datetime, timedelta
from typing import List, Tuple
from dotenv import load_dotenv

load_dotenv()

def connect_to_database() -> pyodbc.Connection:
    
    return pyodbc.connect(driver='{ODBC Driver 18 for SQL Server}',
                          server='mago-database-server.database.windows.net,1433',
                          database=os.environ.get('AZURE_SQL_METRICS_DB'),
                          uid=os.environ.get('AZURE_SQL_METRICS_UID'),
                          pwd=os.environ.get('AZURE_SQL_METRICS_PWD'),
    )

def execute_select_query(connection: pyodbc.Connection, query: str, parameters=None) -> List[Tuple]:
    cursor = connection.cursor()

    try:
        if __is_select_query(query):
            if parameters:
                cursor.execute(query, parameters)
            else:
                cursor.execute(query)

            rows = cursor.fetchall()
            return rows

        else:
            raise ValueError("select_query function should only be used for SELECT queries.")

    except Exception as e:
        print(f"Error executing query: {e}")
        raise

    finally:
        cursor.close()

def execute_insert_query(connection: pyodbc.Connection, query: str, parameters=None) -> None:
    cursor = connection.cursor()

    try:
        if __is_insert_query(query):
            if parameters:
                cursor.execute(query, parameters)
            else:
                cursor.execute(query)

            connection.commit()

        else:
            raise ValueError("insert_query function should only be used for INSERT queries.")

    except Exception as e:
        print(f"Error executing query: {e}")
        raise

    finally:
        cursor.close()

def execute_update_query(connection: pyodbc.Connection, query: str, parameters=None) -> None:
    cursor = connection.cursor()

    try:
        if __is_update_query(query):
            if parameters:
                cursor.execute(query, parameters)
            else:
                cursor.execute(query)

            connection.commit()

        else:
            raise ValueError("update_query function should only be used for UPDATE queries.")

    except Exception as e:
        print(f"Error executing query: {e}")
        raise

    finally:
        cursor.close()

def __is_update_query(query: str) -> bool:
    return query.strip().upper().startswith("UPDATE")

def __is_insert_query(query: str) -> bool:
    return query.strip().upper().startswith("INSERT")

def __is_select_query(query: str) -> bool:
    return query.strip().upper().startswith("SELECT")
