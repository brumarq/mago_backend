from app.main.domain.entities.field import Field
from app.main.domain.entities.log_value import LogValue
from app.main.domain.entities.weekly_average import WeeklyAverage
from datetime import datetime, timedelta
from app.main.domain.entities.log_collection import LogCollection
from app.main.domain.entities.log_collection_type import LogCollectionType
from app.main.infrastructure.repositories.repository import Repository
from app.main import db


class CalculateAverageService:
    def __init__(self):
        self.log_value_repository = Repository(LogValue)

    def calculate_and_save_weekly_averages(self):
        fields = Field.query.all()
        for field in fields:
            average_value, min_value, max_value, device_id = self.__calculate_average_for_last_week(field)
            self.__save_weekly_average(field, average_value, min_value, max_value, device_id)

    def __calculate_average_for_last_week(self, field):
        end_date = datetime.utcnow()
        start_date = end_date - timedelta(weeks=1)

        condition = (
            (LogValue.field_id == field.id) &
            (LogValue.created_at.between(start_date, end_date))
        ) # construct a query

        log_values = self.log_value_repository.get_all_by_condition(condition) #query using repository

        if not log_values:
            return None

        total_value = sum(log_value.value for log_value in log_values)
        average_value = total_value / len(log_values)
        min_value = min(log_value.value for log_value in log_values)
        max_value = max(log_value.value for log_value in log_values)

        # Since device_id is in LogCollection, we can access it through log_values[0].log_collection.device_id
        device_id = log_values[0].log_collection.device_id if log_values[0].log_collection else None

        return average_value, min_value, max_value, device_id

    def __save_weekly_average(self, field, average_value, min_value, max_value, device_id):
        weekly_average_entry = WeeklyAverage(
            average_value=average_value,
            min_value=min_value,
            max_value=max_value,
            device_id=device_id,
            field_id=field.id
        )

        db.session.add(weekly_average_entry)
        db.session.commit()