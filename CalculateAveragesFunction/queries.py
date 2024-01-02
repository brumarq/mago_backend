import pyodbc
from datetime import datetime, timedelta
from dateutil.relativedelta import relativedelta
from db import execute_select_query, execute_insert_query, connect_to_database

def perform_daily_processing():
    connection = connect_to_database()
    process_logs("WeeklyAverage", connection)
    process_logs("MonthlyAverage", connection)
    process_logs("YearlyAverage", connection)

def get_most_recent_reference_date(table_name: str, connection: pyodbc.Connection) -> datetime.date:

    query = f"SELECT MAX(reference_date) FROM {table_name};"
    result = execute_select_query(connection, query)

    if result:
        most_recent_timestamp = result[0][0]
        return most_recent_timestamp

    return None


def get_oldest_log_creation_date(table_name: str, connection: pyodbc.Connection) -> datetime.date:

    query = f"""
        SELECT
            MIN(created_at) AS oldest_date
        FROM
            LogValue;
    """
    result = execute_select_query(connection, query)

    if result and result[0][0] is not None:
        oldest_datetime = result[0][0]
        oldest_date = oldest_datetime.date()

        # This is needed so that the CURRENT records in the LogValue do not get skipped during retrieval of logs
        if table_name == "WeeklyAverage":
            oldest_date -= relativedelta(weeks=1)
        elif table_name == "MonthlyAverage":
            oldest_date -= relativedelta(months=1)
        elif table_name == "YearlyAverage":
            oldest_date -= relativedelta(years=1)

        return oldest_date

    return None


def get_aggregated_logs_by_date_time(field_id: int, start_date: datetime.date, end_date: datetime.time, connection: pyodbc.Connection) -> tuple:
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
            lv.field_id = ? AND lv.created_at >= ? AND lv.created_at < ?
        GROUP BY
            lc.device_id;
    """

    parameters = (field_id, start_date, end_date)
    result = execute_select_query(connection, query, parameters)

    if not result:
        return None

    return result[0]

def process_log_records(table_name: str, start_date: datetime.date, end_date: datetime.date, connection: pyodbc.Connection):
    field_ids = get_all_field_ids(connection)

    for field_id in field_ids:
        result = get_aggregated_logs_by_date_time(field_id, start_date, end_date, connection)

        if result is not None:
            average_value, min_value, max_value, device_id = result
            save_averages(table_name, field_id, average_value, min_value, max_value, device_id, start_date, connection) # start_date is the reference_date..
    #TODO FINISH??


def get_reference_date_or_log_date(table_name: str, connection: pyodbc.Connection):
    last_record_date = get_most_recent_reference_date(table_name, connection) # Try fetching lastest record from WeeklyAverage (reference_date)

    if not last_record_date:
        oldest_log_date = get_oldest_log_creation_date(table_name, connection) # If nothing is found in the latest records (meaning that WeeklyAverage table is empty), try to fetch the MOST recent log from LogValue.

        if not oldest_log_date: # If no oldest creat_at is found in LogValue, that means that LogValue is also empty, hence return nothing...
            return None
        
        last_record_date = oldest_log_date # If something is found in oldest_log_date, then set it as the last_record date and return it.

    return last_record_date


def process_logs(table_name: str, connection: pyodbc.Connection):
    if table_name == "WeeklyAverage":
        last_record_date = get_reference_date_or_log_date(table_name, connection)

        if not last_record_date:
            return
    
        while True:
            start_date = last_record_date

            while start_date.weekday() != 0: #need to make sure that we refer to first day of the week (Monday) (0 == Monday)
                start_date -= timedelta(days=1)
            start_date += timedelta(days=7) # move to next week's Monday
            end_date = start_date + timedelta(days=7) 
            current_date = datetime.now().date() + relativedelta(years=1) #remove later (relativedelta part)

            if end_date > current_date: # no unhandled records for any previous completed weeks
                return
            
            process_log_records(table_name, start_date, end_date, connection) #ADJUST LATER
            last_record_date = start_date
            
    elif table_name == "MonthlyAverage":
        last_record_date = get_reference_date_or_log_date(table_name, connection)

        if not last_record_date:
            return
        
        while True:
            start_date = datetime(last_record_date.year, last_record_date.month, 1).date() + relativedelta(months=1)
            end_date = start_date + relativedelta(months=1)
            current_date = datetime.now().date() + relativedelta(years=1) #remove later

            if end_date > current_date:
                return
            
            #ProcessLogRecords()
            process_log_records(table_name, start_date, end_date, connection)
            last_record_date = start_date


    elif table_name == "YearlyAverage":
        last_record_date = get_reference_date_or_log_date(table_name, connection)

        if not last_record_date:
            return
        
        while True:
            start_date = datetime(last_record_date.year, 1, 1).date() + relativedelta(years=1)
            end_date = start_date + relativedelta(years=1)
            current_date = datetime.now().date() + relativedelta(years=1) #remove later

            if end_date > current_date:
                return
            
            process_log_records(table_name, start_date, end_date, connection)
            last_record_date = start_date


def save_averages(table_name: str, field_id: int, average_value: float, min_value: float, max_value: float, device_id: int, reference_date: datetime.date, connection: pyodbc.Connection) -> None:
    current_datetime = datetime.utcnow()

    insert_query = f"""
        INSERT INTO {table_name} (created_at, updated_at, average_value, min_value, max_value, device_id, field_id, reference_date)
        VALUES (?, ?, ?, ?, ?, ?, ?, ?)
    """
    insert_params = (current_datetime, current_datetime, average_value, min_value, max_value, device_id, field_id, reference_date)

    execute_insert_query(connection, insert_query, insert_params)




def get_all_field_ids(connection):
    fields_query = "SELECT id FROM Field"
    
    result = execute_select_query(connection, fields_query)

    if not result:
        return None

    # Extract the IDs from the result and return as a list
    field_ids = [row[0] for row in result]
    return field_ids

# def get_aggregated_logs_by_table_name_and_timeframe(table_name: str, start_date: datetime.date, end_date: datetime.date, connection: pyodbc.Connection):
#     query = f"""
#         SELECT
#             average_value,
#             min_value,
#             max_value,
#             device_id,
#             field_id,
#             reference_date
#         FROM
#             {table_name}
#         WHERE
#             created_at BETWEEN ? AND ?;
#     """
#     parameters = (start_date, end_date)  # Correct order of parameters
#     result = execute_select_query(connection, query, parameters)

#     if not result:
#         return None

#     return result