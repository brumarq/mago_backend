import pyodbc
from datetime import datetime, timedelta, relativedelta
from db import execute_select_query, execute_insert_query, execute_update_query

def get_all_field_ids(connection):
    fields_query = "SELECT id FROM Field"
    
    result = execute_select_query(connection, fields_query)

    if not result:
        return None
    
    return result[0]

def calculate_average_for_table(table_name: str, field_id: int, connection: pyodbc.Connection) -> tuple:
    end_date = datetime.utcnow()

    if table_name == 'Weekly':
        start_date = end_date - timedelta(weeks=1)
    elif table_name == 'Monthly':
        start_date = end_date - timedelta(weeks=4)  # Assuming 4 weeks per month for simplicity
    elif table_name == 'Yearly':
        start_date = end_date - timedelta(weeks=52)  # Assuming 52 weeks per year for simplicity
    else:
        raise ValueError("Invalid interval. Use 'Weekly', 'Monthly', or 'Yearly'.")

    query = """
        SELECT
            AVG(CAST(lv.value AS FLOAT)) AS average_value,
            MIN(CAST(lv.value AS FLOAT)) AS min_value,
            MAX(CAST(lv.value AS FLOAT)) AS max_value,
            lc.device_id
        FROM
            LogValue lv
        JOIN
            LogCollection lc ON lv.log_collection_id = lc.id
        JOIN
            LogCollectionType lct ON lc.log_collection_type_id = lct.id
        WHERE
            lv.field_id = ? AND lv.created_at BETWEEN ? AND ?
        GROUP BY
            lc.device_id;
    """

    parameters = (field_id, start_date, end_date) 
    result = execute_select_query(connection, query, parameters)

    if not result:
        return None

    return result[0]

def save_averages(type: str, field_id: int, average_value: float, min_value: float, max_value: float, device_id: int, connection: pyodbc.Connection) -> None:
    current_datetime = datetime.utcnow()

    insert_query = f"""
        INSERT INTO {type} (created_at, updated_at, average_value, min_value, max_value, device_id, field_id)
        VALUES (?, ?, ?, ?, ?, ?, ?)
    """
    insert_params = (current_datetime, current_datetime, average_value, min_value, max_value, device_id, field_id)

    execute_insert_query(connection, insert_query, insert_params)



def get_aggregated_logs_by_table_name_and_timeframe(table_name: str, connection: pyodbc.Connection, start_date=None, end_date=None):
    date_filter = ""
    if start_date and end_date:
        date_filter = f"AND created_at BETWEEN ? AND ?"

    query = f"""
        SELECT
            average_value,
            min_value,
            max_value,
            device_id,
            field_id
        FROM
            {table_name}
        WHERE
            1=1
            {date_filter};
    """
    parameters = (start_date, end_date) if date_filter else ()
    result = execute_select_query(connection, query, parameters)

    if not result:
        return None

    return result
