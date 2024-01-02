from app.main.domain.enums.aggregated_log_date_type import AggregatedLogDateType
from app.main.application.service.abstract.base_aggregated_logs_service import BaseAggregatedLogsService
from app.main.infrastructure.repositories.repository import Repository
from app.main.domain.entities.weekly_average import WeeklyAverage
from app.main.domain.entities.monthly_average import MonthlyAverage
from app.main.domain.entities.yearly_average import YearlyAverage
from app.main.domain.entities.field import Field
from datetime import datetime
from flask import abort
from flask import request
from app.main.webapp.middleware.authentication import has_required_permission

class AggregatedLogsService(BaseAggregatedLogsService):
    def __init__(self, field_respository: Repository(Field), weekly_average_repository: Repository(WeeklyAverage), monthly_average_repository: Repository(MonthlyAverage), yearly_average_repository: Repository(YearlyAverage)):
        self.field_repository = field_respository
        self.weekly_average_repository = weekly_average_repository
        self.monthly_average_repository = monthly_average_repository
        self.yearly_average_repository = yearly_average_repository

    def get_aggregated_logs(self, aggregated_log_date_type: str, device_id: int, field_id: int, start_date: str = None, end_date: str = None):
        
        self.__validate_agg_logs_parameters(aggregated_log_date_type, device_id, field_id)  

        aggregated_log_date_type = aggregated_log_date_type.upper()

        if aggregated_log_date_type == AggregatedLogDateType.WEEKLY.value.upper():
            repository = self.weekly_average_repository
        elif aggregated_log_date_type == AggregatedLogDateType.MONTHLY.value.upper():
            repository = self.monthly_average_repository
        elif aggregated_log_date_type == AggregatedLogDateType.YEARLY.value.upper():
            repository = self.yearly_average_repository

        condition = (repository.model.device_id == device_id) & (repository.model.field_id == field_id)

        # start_date = request.args.get('start_date')
        # end_date = request.args.get('end_date')
        if (start_date is None and end_date is not None) or (start_date is not None and end_date is None):
            abort(400, "Both start date and end date must be provided or none of them.")

        if start_date is not None and end_date is not None:
            if not (self.__is_valid_date(start_date) and self.__is_valid_date(end_date)):
                abort(400, "Invalid date format. Please use the format YYYY-MM-DD.")
            condition &= (repository.model.reference_date >= start_date) & (repository.model.reference_date <= end_date)

        aggregated_logs = repository.get_all_by_condition(condition)

        return aggregated_logs
    
    def __is_valid_date(self, date_str):
        try:
            datetime.strptime(date_str, '%Y-%m-%d')  # Assuming the date format is YYYY-MM-DD
            return True
        except ValueError:
            return False
        
    def __validate_agg_logs_parameters(self, aggregated_log_date_type: str, device_id: int, field_id: int):
        if not (has_required_permission("client") or has_required_permission("admin")):
            abort(401, "This user does not have sufficient permissions")

        if not any(aggregated_log_date_type.upper() == item.value.upper() for item in AggregatedLogDateType): #to avoid caps issues
            abort(400, "Invalid date type entered (must be 'Weekly', 'Monthly' or 'Yearly').")

        if device_id <= 0:
            abort(400, "Device id cannot be 0 or negative.")

        if field_id <= 0:
            abort(400, "Field id cannot be 0 or negative.")
        
        field_exists = self.field_repository.exists_by_id(field_id)

        if not field_exists:
            abort(404, f"Field with id {field_id} does not exist.")  