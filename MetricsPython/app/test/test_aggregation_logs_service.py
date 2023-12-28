import pytest
from app.main.domain.entities.weekly_average import WeeklyAverage
from app.main.domain.entities.monthly_average import MonthlyAverage
from app.main.domain.entities.yearly_average import YearlyAverage
from app.main.domain.entities.field import Field
from unittest.mock import patch

@patch('app.main.application.service.aggregated_logs_service.has_required_permission', return_value=False)
def test_insufficient_permissions(app, aggregated_logs_service):
    with app.test_request_context():
        with pytest.raises(Exception) as excinfo:
            aggregated_logs_service.get_aggregated_logs('Weekly', device_id=1, field_id=1)
        assert "401 Unauthorized: This user does not have sufficient permissions" in str(excinfo.value)

@patch('app.main.application.service.aggregated_logs_service.has_required_permission', return_value=True)
def test_get_aggregated_logs_invalid_date_type(app, aggregated_logs_service):
    with app.test_request_context():
        with pytest.raises(Exception) as excinfo:
            aggregated_logs_service.get_aggregated_logs('InvalidType', device_id=1, field_id=1)
        assert "400 Bad Request: Invalid date type entered (must be 'Weekly', 'Monthly' or 'Yearly')." in str(excinfo.value)


@patch('app.main.application.service.aggregated_logs_service.has_required_permission', return_value=True)
@pytest.mark.parametrize("date_type", ["Weekly", "Monthly", "Yearly"])
def test_get_aggregated_logs_field_id_0(app, aggregated_logs_service, date_type):
    with app.test_request_context():
        with pytest.raises(Exception) as excinfo:
            aggregated_logs_service.get_aggregated_logs(date_type, device_id=1, field_id=0)
        assert "400 Bad Request: Field id cannot be 0 or negative." in str(excinfo.value)

@patch('app.main.application.service.aggregated_logs_service.has_required_permission', return_value=True)
@pytest.mark.parametrize("date_type", ["Weekly", "Monthly", "Yearly"])
def test_get_aggregated_logs_non_existent_field(app, aggregated_logs_service, date_type):
    with app.test_request_context():
        with pytest.raises(Exception) as excinfo:
            aggregated_logs_service.get_aggregated_logs(date_type, device_id=1, field_id=999)

        assert "404 Not Found: Field with id 999 does not exist." in str(excinfo.value)

# Perform correct test using all 3 tables
@pytest.mark.parametrize("date_type, entity_type", [
    ('Weekly', WeeklyAverage),
    ('Monthly', MonthlyAverage),
    ('Yearly', YearlyAverage),
])
@patch('app.main.application.service.aggregated_logs_service.has_required_permission', return_value=True)
def test_get_aggregated_logs_valid_date_type_and_field(app, aggregated_logs_service, date_type, entity_type):
    with app.test_request_context():
        # Use the field_repository from the aggregated_logs_service fixture
        created_field = aggregated_logs_service.field_repository.create(Field(name='TestField'))

        if entity_type == WeeklyAverage:
            repository = aggregated_logs_service.weekly_average_repository
        elif entity_type == MonthlyAverage:
            repository = aggregated_logs_service.monthly_average_repository
        elif entity_type == YearlyAverage:
            repository = aggregated_logs_service.yearly_average_repository

        # Insert a record into WeeklyAverage/MonthlyAverage/YearlyAverage for testing
        aggregated_logs_entity = entity_type(
            average_value=42.0,
            min_value=0.0,
            max_value=100.0,
            device_id=1,
            field_id=created_field.id
        )
        repository.create(aggregated_logs_entity)

        # Call the get_aggregated_logs method for Weekly/Monthly/Yearly
        aggregated_logs_result = aggregated_logs_service.get_aggregated_logs(date_type, device_id=1, field_id=created_field.id)

        assert isinstance(aggregated_logs_result, list)  # Check if the result is a list
        assert len(aggregated_logs_result) == 1  # Check if the result contains the inserted record
        assert date_type in ['Weekly', 'Monthly', 'Yearly']  # Check if date type is valid 