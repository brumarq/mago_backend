from app.main.domain.entities.aggregated_log import AggregatedLog
from app.main.domain.entities.log_value import LogValue
from app.main.domain.entities.log_collection import LogCollection
from datetime import date, timedelta
from typing import List
from app.main.application.service.interfaces.i_metrics_service import IMetricsService
from app.main.infrastructure.repositories.repository import Repository
from flask import abort


class MetricsService(IMetricsService):

    def __init__(self):
        self.aggregated_log_repository = Repository(AggregatedLog)
        self.log_value_repository = Repository(LogValue)

    def get_aggregated_logs(self, aggregated_log_date_type) -> List[AggregatedLog]:

        aggregated_logs = self.aggregated_log_repository.get_all()

        current_date = date.today()

        time_delta_dict = {
            "Weekly": timedelta(days=7),
            "Monthly": timedelta(weeks=4),
            "Yearly": timedelta(weeks=52),
        }

        time_delta = time_delta_dict.get(aggregated_log_date_type)

        aggregated_logs = [log for log in aggregated_logs if log.date >= current_date - time_delta]

        return aggregated_logs

    def get_device_metrics_by_device(self, device_id) -> List[LogValue]:   

        if device_id <= 0:
            abort(400, "Device id cannot be 0 or negative!")

        #check if logcollection in logvalue has device_id specified and if so, it returns those entries
        device_metrics = self.log_value_repository.get_all_by_condition(LogValue.log_collection.has(LogCollection.device_id == device_id)) 

        return device_metrics
        
        
