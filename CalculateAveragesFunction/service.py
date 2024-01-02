from datetime import datetime, timedelta
from dateutil.relativedelta import relativedelta
from db import Database
from queries import QueryHandler

class AggregatedLogsProcessor:
    def __init__(self):
        self.database = Database()
        self.queryhandler = QueryHandler(self.database)

    def perform_daily_processing(self):
        self.__process_logs("WeeklyAverage")
        self.__process_logs("MonthlyAverage")
        self.__process_logs("YearlyAverage")

    def __process_log_records(self, table_name: str, start_date: datetime.date, end_date: datetime.date):
        field_ids = self.queryhandler.get_all_field_ids()

        for field_id in field_ids:
            result = self.queryhandler.get_aggregated_logs_by_date_time(field_id, start_date, end_date)

            if result is not None:
                average_value, min_value, max_value, device_id = result
                self.queryhandler.save_averages(table_name, field_id, average_value, min_value, max_value, device_id, start_date) # start_date is the reference_date..
        #TODO FINISH??


    def __get_reference_date_or_log_date(self, table_name: str):
        last_record_date = self.queryhandler.get_most_recent_reference_date(table_name) # Try fetching lastest record from WeeklyAverage (reference_date)

        if not last_record_date:
            oldest_log_date = self.queryhandler.get_oldest_log_creation_date(table_name) # If nothing is found in the latest records (meaning that WeeklyAverage table is empty), try to fetch the MOST recent log from LogValue.

            if not oldest_log_date: # If no oldest creat_at is found in LogValue, that means that LogValue is also empty, hence return nothing...
                return None
            
            last_record_date = oldest_log_date # If something is found in oldest_log_date, then set it as the last_record date and return it.

        return last_record_date


    def __process_logs(self, table_name: str):
        if table_name == "WeeklyAverage":
            last_record_date = self.__get_reference_date_or_log_date(table_name)

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
                
                self.__process_log_records(table_name, start_date, end_date) #ADJUST LATER
                last_record_date = start_date
                
        elif table_name == "MonthlyAverage":
            last_record_date = self.__get_reference_date_or_log_date(table_name)

            if not last_record_date:
                return
            
            while True:
                start_date = datetime(last_record_date.year, last_record_date.month, 1).date() + relativedelta(months=1)
                end_date = start_date + relativedelta(months=1)
                current_date = datetime.now().date() + relativedelta(years=1) #remove later

                if end_date > current_date:
                    return
                
                self.__process_log_records(table_name, start_date, end_date)
                last_record_date = start_date


        elif table_name == "YearlyAverage":
            last_record_date = self.__get_reference_date_or_log_date(table_name)

            if not last_record_date:
                return
            
            while True:
                start_date = datetime(last_record_date.year, 1, 1).date() + relativedelta(years=1)
                end_date = start_date + relativedelta(years=1)
                current_date = datetime.now().date() + relativedelta(years=1) #remove later

                if end_date > current_date:
                    return
                
                self.__process_log_records(table_name, start_date, end_date)
                last_record_date = start_date