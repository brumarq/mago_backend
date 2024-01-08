from datetime import datetime, timedelta
import random
from db import Database

db = Database()

def truncate_table(connection, table_name):
    cursor = connection.cursor()
    cursor.execute(f"TRUNCATE TABLE {table_name};")
    cursor.commit()
    cursor.close()

def insert_records(connnection, start_date, end_date, field_ids, log_collection_ids, value_range):
    cursor = connnection.cursor()
    current_date = start_date
    while current_date <= end_date:
        for field_id in field_ids:
            for log_collection_id in log_collection_ids:
                field_value = round(random.uniform(*value_range), 3)
                query = f"""
                INSERT INTO LogValue (created_at, updated_at, value, field_id, log_collection_id)
                VALUES ('{current_date}', '{current_date}', {field_value}, {field_id}, {log_collection_id});
                """
                cursor.execute(query)
                cursor.commit()
        current_date += timedelta(days=1)
    cursor.close()

# Define parameters
start_date = datetime(2022, 1, 1)
end_date = datetime(2024, 1, 1)

field_ids = [1, 2]  
log_collection_ids = [10, 11]  
value_range = (10.0, 111.0)

# Establish connection
connection = db.connection

# Remove all records first
truncate_table(connection, 'LogValue')

# Insert necessary records
insert_records(connection, start_date, end_date, field_ids, log_collection_ids, value_range)

# Close off connection
connection.close()
print("Records inserted successfully!")