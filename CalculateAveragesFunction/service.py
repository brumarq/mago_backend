import pyodbc
from datetime import datetime, timedelta
from db import connect_to_database
from queries import get_all_field_ids, calculate_average_for_table, save_averages, get_aggregated_logs_by_table_name_and_timeframe

def calculate_and_save_aggregates() -> None:
    connection = connect_to_database()
    try:
        __aggregate_and_transfer('WeeklyAverage', connection)

        # Check and execute monthly aggregation
        if __is_new_month():
            __aggregate_and_transfer('MonthlyAverage', connection)

        # Check and execute yearly aggregation
        if __is_new_year():
            __aggregate_and_transfer('YearlyAverage', connection)

    finally:
        connection.close()

def __aggregate_and_transfer(table_name: str, connection: pyodbc.Connection) -> None:
    field_ids = get_all_field_ids(connection)

    for field_id in field_ids:
        
        # this part is done for weekly and will always be done regardless
        result = calculate_average_for_table(table_name, field_id, connection)

        if result is not None:
            average_value, min_value, max_value, device_id = result
            save_averages(table_name, field_id, average_value, min_value, max_value, device_id, connection)

        # TODO: check for dates because we do not want to insert records that are ALREADY in the table.
        # this part is done for monthly (when a new month happens) and retrieves entries from WeeklyAverage table rather than LogValue and saves it in MonthlyAverage
        if table_name == 'MonthlyAverage':
            # Assuming start_date and end_date represent the first and last day of the last month
            start_date = (datetime.utcnow() - timedelta(days=datetime.utcnow().day)).replace(day=1)
            end_date = datetime.utcnow().replace(day=1) - timedelta(days=1)
            
            result = get_aggregated_logs_by_table_name_and_timeframe('WeeklyAverage', connection, start_date, end_date)
            average_value, min_value, max_value, device_id = result
            save_averages(table_name, field_id, average_value, min_value, max_value, device_id, connection)

        if table_name == 'YearlyAverage':
            # Assuming start_date and end_date represent the first and last day of the last 12 months
            start_date = datetime.utcnow() - timedelta(days=365)
            end_date = datetime.utcnow()

            result = get_aggregated_logs_by_table_name_and_timeframe('MonthlyAverage', connection, start_date, end_date)
            average_value, min_value, max_value, device_id = result
            save_averages(table_name, field_id, average_value, min_value, max_value, device_id, connection)


def __is_new_week():
    return datetime.utcnow().isoweekday() == 1  # Assuming the week starts on Monday

def __is_new_month():
    return datetime.utcnow().day == 1

def __is_new_year():
    return datetime.utcnow().date() == datetime.utcnow().replace(month=1, day=1).date()