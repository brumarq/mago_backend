from datetime import datetime, timedelta
from dateutil.relativedelta import relativedelta
from db import Database
from queries import QueryHandler
from models import DeviceStats

class AggregatedLogsProcessor:
    def __init__(self):
        self.database = Database()
        self.queryhandler = QueryHandler(self.database)

    def perform_daily_processing(self):
        self.__process_logs("WeeklyAverage")
        self.__process_logs("MonthlyAverage")
        self.__process_logs("YearlyAverage")

    def __process_log_records(self, table_name: str, start_date: datetime.date, end_date: datetime.date):
        all_log_value_entries = self.queryhandler.get_all_logs_by_date_range(start_date, end_date)
        device_stats = DeviceStats()
        for log_value_entry in all_log_value_entries:
            value, field_id, device_id = log_value_entry

            device_field_stats = device_stats.get_device_stats(device_id, field_id)

            if device_field_stats.count == 0: # if count is 0 we do not have a value yet for min and max (0) so we need to set the current values
                device_field_stats.min = value
                device_field_stats.max = value
            else: # count is more than 1, so we have values to calc min/max from
                device_field_stats.min = min(device_field_stats.min, value) 
                device_field_stats.max = max(device_field_stats.max, value)

            device_field_stats.sum += value
            device_field_stats.count += 1

        for device_id, field_stats in device_stats.items(): # foreach device get each field and the statsitics of the values (min,max,average) and insert them into database.
            for field_id, log_value_stats in field_stats.items():
                if log_value_stats.count > 0:
                    average_value = log_value_stats.sum / log_value_stats.count
                    self.queryhandler.save_averages(table_name, field_id, average_value, log_value_stats.min, log_value_stats.max, device_id, start_date)
   

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
                current_date = datetime.now().date() #+ relativedelta(years=1) #remove later (relativedelta part)

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
                current_date = datetime.now().date() #+ relativedelta(years=1) #remove later

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
                current_date = datetime.now().date() #+ relativedelta(years=1) #remove later

                if end_date > current_date:
                    return
                
                self.__process_log_records(table_name, start_date, end_date)
                last_record_date = start_date