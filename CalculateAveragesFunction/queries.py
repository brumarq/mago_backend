import pyodbc
from datetime import datetime, timedelta
from db import connect_to_database, execute_select_query, execute_insert_query, execute_update_query

def calculate_and_save_aggregates() -> None:
    connection = connect_to_database()
    try:
        fields_data = __get_all_fields_data(connection)

        for field_data in fields_data:

            weekly_result = __calculate_average_for_last_interval('Weekly', field_data[0], connection)
            # monthly_result = __calculate_average_for_last_interval('Monthly', field_data[0], connection)
            # yearly_result = __calculate_average_for_last_interval('Yearly', field_data[0], connection)

            if weekly_result is not None:
                average_value, min_value, max_value, device_id = weekly_result
                __save_averages('WeeklyAverage', field_data[0], average_value, min_value, max_value, device_id, connection)

            # if monthly_result is not None:
            #     average_value, min_value, max_value, device_id = monthly_result
            #     __save_averages('MonthlyAverage', field_data[0], average_value, min_value, max_value, device_id, connection, is_first_of_month=True)

            # if yearly_result is not None:
            #     average_value, min_value, max_value, device_id = yearly_result
            #     __save_averages('YearlyAverage', field_data[0], average_value, min_value, max_value, device_id, connection, is_first_of_year=True)

    finally:
        connection.close()

def __get_all_fields_data(connection):
    fields_query = "SELECT * FROM Field"
    return execute_select_query(connection, fields_query)

def __calculate_average_for_last_interval(interval: str, field_id: int, connection: pyodbc.Connection) -> tuple:
    end_date = datetime.utcnow()

    if interval == 'Weekly':
        start_date = end_date - timedelta(weeks=1)
    elif interval == 'Monthly':
        start_date = end_date - timedelta(weeks=4)  # Assuming 4 weeks per month for simplicity
    elif interval == 'Yearly':
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

def __save_averages(type: str, field_id: int, average_value: float, min_value: float, max_value: float, device_id: int, connection: pyodbc.Connection) -> None:
    current_datetime = datetime.utcnow()

    insert_query = f"""
        INSERT INTO {type} (created_at, updated_at, average_value, min_value, max_value, device_id, field_id)
        VALUES (?, ?, ?, ?, ?, ?, ?)
    """
    insert_params = (current_datetime, current_datetime, average_value, min_value, max_value, device_id, field_id)

    execute_insert_query(connection, insert_query, insert_params)

def __update_averages(type: str, record_id: int, average_value: float, min_value: float, max_value: float, device_id: int, connection: pyodbc.Connection) -> None:
    current_datetime = datetime.utcnow()

    update_query = f"""
        UPDATE {type}
        SET
            updated_at = ?,
            average_value = ?,
            min_value = ?,
            max_value = ?,
            device_id = ?
        WHERE
            id = ?
    """
    update_params = (current_datetime, average_value, min_value, max_value, device_id, record_id)

    execute_update_query(connection, update_query, update_params)


def is_new_week(last_entry_date):
    return last_entry_date is None or last_entry_date < datetime.utcnow() - timedelta(weeks=1)

def is_new_month(last_entry_date):
    return last_entry_date is None or last_entry_date.month != datetime.utcnow().month

def is_new_year(last_entry_date):
    return last_entry_date is None or last_entry_date.year != datetime.utcnow().year