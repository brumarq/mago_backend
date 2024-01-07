from datetime import datetime
from dateutil.relativedelta import relativedelta

class QueryHandler:
    def __init__(self, database):
        self.database = database

    def get_most_recent_reference_date(self, table_name: str) -> datetime.date:

        query = f"SELECT MAX(reference_date) FROM {table_name};"
        result = self.database.execute_select_query(query)

        if result:
            most_recent_timestamp = result[0][0]
            return most_recent_timestamp

        return None


    def get_oldest_log_creation_date(self, table_name: str) -> datetime.date:

        query = f"""
            SELECT
                MIN(created_at) AS oldest_date
            FROM
                LogValue;
        """
        result = self.database.execute_select_query(query)

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


    # def get_aggregated_logs_by_date_time(self, field_id: int, start_date: datetime.date, end_date: datetime.time) -> tuple:
    #     query = """
    #         SELECT
    #             AVG(CAST(lv.value AS FLOAT)) AS average_value,
    #             MIN(CAST(lv.value AS FLOAT)) AS min_value,
    #             MAX(CAST(lv.value AS FLOAT)) AS max_value,
    #             lc.device_id
    #         FROM
    #             LogValue lv
    #         JOIN
    #             LogCollection lc ON lv.log_collection_id = lc.id
    #         JOIN
    #             LogCollectionType lct ON lc.log_collection_type_id = lct.id
    #         WHERE
    #             lv.field_id = ? AND lv.created_at >= ? AND lv.created_at < ?
    #         GROUP BY
    #             lc.device_id;
    #     """

    #     parameters = (field_id, start_date, end_date)
    #     result = self.database.execute_select_query(query, parameters)

    #     if not result:
    #         return None

    #     return result[0]

    def save_averages(self, table_name: str, field_id: int, average_value: float, min_value: float, max_value: float, device_id: int, reference_date: datetime.date) -> None:
        current_datetime = datetime.utcnow()

        insert_query = f"""
            INSERT INTO {table_name} (created_at, updated_at, average_value, min_value, max_value, device_id, field_id, reference_date)
            VALUES (?, ?, ?, ?, ?, ?, ?, ?)
        """
        insert_params = (current_datetime, current_datetime, average_value, min_value, max_value, device_id, field_id, reference_date)

        self.database.execute_insert_query(insert_query, insert_params)


    def get_all_field_ids(self):
        fields_query = "SELECT id FROM Field"
        
        result = self.database.execute_select_query(fields_query)

        if not result:
            return None

        # Extract the IDs from the result and return as a list
        field_ids = [row[0] for row in result]
        return field_ids
    

    def get_all_logs_by_date_range(self, start_date: datetime.date, end_date: datetime.date) -> list:
        query = """
            SELECT
                lv.value,
                lv.field_id,
                lc.device_id
            FROM
                LogValue lv
            JOIN
                LogCollection lc ON lv.log_collection_id = lc.id
            JOIN
                LogCollectionType lct ON lc.log_collection_type_id = lct.id
            WHERE
                lv.created_at >= ? AND lv.created_at < ?
        """

        parameters = (start_date, end_date)
        result = self.database.execute_select_query(query, parameters)

        return result
