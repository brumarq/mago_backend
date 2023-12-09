from app.main.domain.entities.aggregated_log import AggregatedLog
from app.main.domain.entities.log_value import LogValue
from app.main.domain.entities.log_collection import LogCollection
from app.main.domain.enums.aggregated_log_date_type import AggregatedLogDateType
from datetime import date, timedelta
from typing import List
from app.main.application.service.abstract.metrics_abstract_service import MetricsAbstractService
from app.main.infrastructure.repositories.repository import Repository
from app.main.application.helpers.metrics_helper import MetricsHelper
from app.main.application.dtos.export_aggregated_logs_csv_dto import ExportAggregatedLogsCsvDto
from typing import Dict
from flask import abort
from flask import make_response


class MetricsService(MetricsAbstractService):

    def __init__(self):
        self.aggregated_log_repository = Repository(AggregatedLog)
        self.log_value_repository = Repository(LogValue)

    def get_aggregated_logs(self, aggregated_log_date_type) -> List[AggregatedLog]:

        aggregated_log_date_type = aggregated_log_date_type.upper() # to avoid case problems

        if not any(aggregated_log_date_type == item.value.upper() for item in AggregatedLogDateType):
            abort(400, "Invalid date type entered (must be 'Weekly', 'Monthly' or 'Yearly').")

        time_delta_dict = {
            AggregatedLogDateType.WEEKLY: timedelta(days=7),
            AggregatedLogDateType.MONTHLY: timedelta(weeks=4),
            AggregatedLogDateType.YEARLY: timedelta(weeks=52),
        }

        # Use the uppercase input for dictionary lookup
        time_delta = time_delta_dict.get(AggregatedLogDateType[aggregated_log_date_type])

        aggregated_logs = self.aggregated_log_repository.get_all()

        current_date = date.today()

        aggregated_logs = [log for log in aggregated_logs if log.created_at.date() >= current_date - time_delta]

        return aggregated_logs

    def get_device_metrics_by_device(self, device_id) -> List[LogValue]:   

        if device_id <= 0:
            abort(400, "Device id cannot be 0 or negative!")

        #check if logcollection in logvalue has device_id specified and if so, it returns those entries
        device_metrics = self.log_value_repository.get_all_by_condition(LogValue.log_collection.has(LogCollection.device_id == device_id)) 

        return device_metrics
    
    def export_aggregated_logs_csv(self, export_csv_dto: ExportAggregatedLogsCsvDto):     

        if not export_csv_dto.file_name.isalnum():
            abort(400, f"Filename '{export_csv_dto.file_name}' is invalid. Please provide a valid filename")

        aggregated_logs = self.get_aggregated_logs(export_csv_dto.aggregated_log_date_type)

        csv_data = MetricsHelper.generate_csv(aggregated_logs)

        response = make_response(csv_data.getvalue(), 201)
        response.headers["Content-Disposition"] = f"attachment; filename={export_csv_dto.file_name}.csv"
        response.headers["Content-Type"] = "application/octet-stream"
        response.headers["Content-Transfer-Encoding"] = "bytes"

        return response




        
        
