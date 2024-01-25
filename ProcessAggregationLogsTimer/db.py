import os
import pyodbc
from typing import List, Tuple
from dotenv import load_dotenv

load_dotenv()

class Database:
    def __init__(self):
        self.connection = pyodbc.connect(os.environ.get('METRICS_DB_CONNECTION_STRING_PYODBC'))

    def execute_select_query(self, query: str, parameters=None) -> List[Tuple]:
        cursor = self.connection.cursor()

        try:
            if self.__is_select_query(query):
                if parameters:
                    cursor.execute(query, parameters)
                else:
                    cursor.execute(query)

                rows = cursor.fetchall()
                return rows

            else:
                raise ValueError("select_query function should only be used for SELECT queries.")

        except Exception as e:
            self.connection.rollback()
            print(f"Error executing query: {e}")
            raise

        finally:
            cursor.close()

    def execute_insert_query(self, query: str, parameters=None) -> None:
        cursor = self.connection.cursor()

        try:
            if self.__is_insert_query(query):
                if parameters:
                    cursor.execute(query, parameters)
                else:
                    cursor.execute(query)

                self.connection.commit()

            else:
                raise ValueError("insert_query function should only be used for INSERT queries.")

        except Exception as e:
            self.connection.rollback()
            print(f"Error executing query: {e}")
            raise

        finally:
            cursor.close()

    def __is_insert_query(self, query: str) -> bool:
        return query.strip().upper().startswith("INSERT")

    def __is_select_query(self, query: str) -> bool:
        return query.strip().upper().startswith("SELECT")
