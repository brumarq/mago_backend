from app.main.domain.enums.aggregated_log_date_type import AggregatedLogDateType
from datetime import date, timedelta
from app.main.application.service.abstract.aggregated_logs_abstract_service import AggregatedLogsAbstractService
from app.main.infrastructure.repositories.repository import Repository
#from app.main.application.helpers.metrics_helper import MetricsHelper
from app.main.application.dtos.export_aggregated_logs_csv_dto import ExportAggregatedLogsCsvDto
from app.main.domain.entities.weekly_average import WeeklyAverage
from app.main.domain.entities.monthly_average import MonthlyAverage
from app.main.domain.entities.yearly_average import YearlyAverage
from flask import abort
#from flask import make_response


class MetricsService(AggregatedLogsAbstractService):

    def __init__(self):
        self.weekly_average_repository = Repository(WeeklyAverage)
        self.monthly_average_repository = Repository(MonthlyAverage)
        self.yearly_average_repository = Repository(YearlyAverage)


    def get_aggregated_logs(self, device_id: int, field_id: int, aggregated_log_date_type: str):

        aggregated_log_date_type = aggregated_log_date_type.upper()  # to avoid case problems

        if not any(aggregated_log_date_type == item.value.upper() for item in AggregatedLogDateType):
            abort(400, "Invalid date type entered (must be 'Weekly', 'Monthly' or 'Yearly').")

        if aggregated_log_date_type == AggregatedLogDateType.WEEKLY.value.upper():
            repository = self.weekly_average_repository
        elif aggregated_log_date_type == AggregatedLogDateType.MONTHLY.value.upper():
            repository = self.monthly_average_repository
        elif aggregated_log_date_type == AggregatedLogDateType.YEARLY.value.upper():
            repository = self.yearly_average_repository

        if repository is None:
            abort(500, "Unexpected error determining the repository.")

        condition = (repository.model.device_id == device_id) & (repository.model.field_id == field_id)

        aggregated_logs = repository.get_all_by_condition(condition)

        return aggregated_logs

    
    # def export_aggregated_logs_csv(self, export_aggregated_logs_csv_dto: ExportAggregatedLogsCsvDto):     

    #     if not export_aggregated_logs_csv_dto.file_name.isalnum():
    #         abort(400, f"Filename '{export_aggregated_logs_csv_dto.file_name}' is invalid. Please provide a valid filename.")

    #     aggregated_logs = self.get_aggregated_logs(export_aggregated_logs_csv_dto.aggregated_log_date_type)

    #     csv_data = MetricsHelper.generate_csv(aggregated_logs)

    #     response = make_response(csv_data.getvalue(), 201)
    #     response.headers["Content-Disposition"] = f"attachment; filename={export_aggregated_logs_csv_dto.file_name}.csv"
    #     response.headers["Content-Type"] = "application/octet-stream"
    #     response.headers["Content-Transfer-Encoding"] = "bytes"

    #     return response




        
        
