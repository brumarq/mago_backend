from app.main.domain.enums.aggregated_log_date_type import AggregatedLogDateType
from app.main.application.service.abstract.base_aggregated_logs_service import BaseAggregatedLogsService
from app.main.infrastructure.repositories.repository import Repository
from app.main.application.helpers.aggregation_logs_helper import AggregationLogsHelper
from app.main.application.dtos.export_aggregated_logs_csv_dto import ExportAggregatedLogsCsvDto
from app.main.domain.entities.weekly_average import WeeklyAverage
from app.main.domain.entities.monthly_average import MonthlyAverage
from app.main.domain.entities.yearly_average import YearlyAverage
from app.main.domain.entities.field import Field
from flask import abort, make_response


class AggregatedLogsService(BaseAggregatedLogsService):
    def __init__(self, field_respository: Repository(Field), weekly_average_repository: Repository(WeeklyAverage), monthly_average_repository: Repository(MonthlyAverage), yearly_average_repository: Repository(YearlyAverage)):
        self.field_repository = field_respository
        self.weekly_average_repository = weekly_average_repository
        self.monthly_average_repository = monthly_average_repository
        self.yearly_average_repository = yearly_average_repository

    def get_aggregated_logs(self, aggregated_log_date_type: str, device_id: int, field_id: int):

        aggregated_log_date_type = aggregated_log_date_type.upper()  # to avoid case problems

        if not any(aggregated_log_date_type == item.value.upper() for item in AggregatedLogDateType):
            abort(400, "Invalid date type entered (must be 'Weekly', 'Monthly' or 'Yearly').")

        if not self.field_repository.exists_by_id(field_id):
            abort(404, f"Field with id {field_id} does not exist.")

        if aggregated_log_date_type == AggregatedLogDateType.WEEKLY.value.upper():
            repository = self.weekly_average_repository
        elif aggregated_log_date_type == AggregatedLogDateType.MONTHLY.value.upper():
            repository = self.monthly_average_repository
        elif aggregated_log_date_type == AggregatedLogDateType.YEARLY.value.upper():
            repository = self.yearly_average_repository

        condition = (repository.model.device_id == device_id) & (repository.model.field_id == field_id)

        aggregated_logs = repository.get_all_by_condition(condition)

        return aggregated_logs

    
    def export_aggregated_logs_csv(self, export_aggregated_logs_csv_dto: ExportAggregatedLogsCsvDto):     

        if not export_aggregated_logs_csv_dto.file_name.isalnum():
            abort(400, f"Filename '{export_aggregated_logs_csv_dto.file_name}' is invalid. Please provide a valid filename.")

        aggregated_logs = self.get_aggregated_logs(export_aggregated_logs_csv_dto.aggregated_log_date_type, export_aggregated_logs_csv_dto.device_id, export_aggregated_logs_csv_dto.field_id)

        csv_data = AggregationLogsHelper.generate_csv(aggregated_logs)

        suffix = export_aggregated_logs_csv_dto.aggregated_log_date_type.lower()

        response = make_response(csv_data.getvalue(), 201)
        response.headers["Content-Disposition"] = f"attachment; filename={export_aggregated_logs_csv_dto.file_name}_{suffix}.csv"
        response.headers["Content-Type"] = "application/octet-stream"
        response.headers["Content-Transfer-Encoding"] = "bytes"

        return response
    




        
        
