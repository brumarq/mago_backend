from app.main.domain.enums.aggregated_log_date_type import AggregatedLogDateType
from app.main.application.service.abstract.base_aggregated_logs_service import BaseAggregatedLogsService
from app.main.infrastructure.repositories.repository import Repository
from app.main.domain.entities.weekly_average import WeeklyAverage
from app.main.domain.entities.monthly_average import MonthlyAverage
from app.main.domain.entities.yearly_average import YearlyAverage
from app.main.domain.entities.field import Field
from flask import abort
from app.main.webapp.middleware.authentication import has_required_permission


class AggregatedLogsService(BaseAggregatedLogsService):
    def __init__(self, field_respository: Repository(Field), weekly_average_repository: Repository(WeeklyAverage), monthly_average_repository: Repository(MonthlyAverage), yearly_average_repository: Repository(YearlyAverage)):
        self.field_repository = field_respository
        self.weekly_average_repository = weekly_average_repository
        self.monthly_average_repository = monthly_average_repository
        self.yearly_average_repository = yearly_average_repository

    def get_aggregated_logs(self, aggregated_log_date_type: str, device_id: int, field_id: int):

        if not (has_required_permission("client") or has_required_permission("admin")):
            abort(401, "This user does not have sufficient permissions")

        aggregated_log_date_type = aggregated_log_date_type.upper()  # to avoid case problems

        if not any(aggregated_log_date_type == item.value.upper() for item in AggregatedLogDateType):
            abort(400, "Invalid date type entered (must be 'Weekly', 'Monthly' or 'Yearly').")

        if device_id <= 0:
            abort(400, "Device id cannot be 0 or negative.")

        if field_id <= 0:
            abort(400, "Field id cannot be 0 or negative.")

        field_exists = self.field_repository.exists_by_id(field_id)

        if not field_exists:
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