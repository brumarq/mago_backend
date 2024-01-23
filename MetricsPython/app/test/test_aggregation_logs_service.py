import pytest
from app.main.domain.entities.weekly_average import WeeklyAverage
from app.main.domain.entities.monthly_average import MonthlyAverage
from app.main.domain.entities.yearly_average import YearlyAverage
from app.main.domain.entities.field import Field
from datetime import datetime, timedelta
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
def test_get_aggregated_logs_device_id_0(app, aggregated_logs_service, date_type):
    with app.test_request_context():
        with pytest.raises(Exception) as excinfo:
            aggregated_logs_service.get_aggregated_logs(date_type, device_id=0, field_id=1)
        assert "400 Bad Request: Device id cannot be 0 or negative." in str(excinfo.value)

@patch('app.main.application.service.aggregated_logs_service.has_required_permission', return_value=True)
@pytest.mark.parametrize("date_type", ["Weekly", "Monthly", "Yearly"])
def test_get_aggregated_logs_non_existent_field(app, aggregated_logs_service, date_type):
    with app.test_request_context():
        with pytest.raises(Exception) as excinfo:
            aggregated_logs_service.get_aggregated_logs(date_type, device_id=1, field_id=999)

        assert "404 Not Found: Field with id 999 does not exist." in str(excinfo.value)

@patch('app.main.application.service.aggregated_logs_service.has_required_permission', return_value=True)
@pytest.mark.parametrize("date_type, start_date, end_date", [
    ("Weekly", 'fake_date', str(datetime(2022, 1, 1).date())),  
    ("Monthly", str(datetime(2022, 1, 1).date()), 'fake_date'),
    ("Yearly", 'fake_date', 'fake_date'),
])
def test_get_aggregated_logs_invalid_start_or_end_date(app, aggregated_logs_service, date_type, start_date, end_date):
    with app.test_request_context():
        field = aggregated_logs_service.field_repository.create(Field(name='TestField'))
        with pytest.raises(Exception) as excinfo:
            aggregated_logs_service.get_aggregated_logs(date_type, device_id=1, field_id=field.id, start_date=start_date, end_date=end_date)

        assert "400 Bad Request: Invalid date format. Please use the format YYYY-MM-DD." in str(excinfo.value)

@patch('app.main.application.service.aggregated_logs_service.has_required_permission', return_value=True)
@pytest.mark.parametrize("date_type, start_date, end_date", [
    ("Weekly", None, str(datetime(2022, 1, 1).date())),  
    ("Monthly", str(datetime(2022, 1, 1).date()), None),
    ("Yearly", None, str(datetime(2022, 1, 1).date())),
])
def test_get_aggregated_logs_only_start_or_end_date_provided(app, aggregated_logs_service, date_type, start_date, end_date):
    with app.test_request_context():
        field = aggregated_logs_service.field_repository.create(Field(name='TestField'))
        with pytest.raises(Exception) as excinfo:
            aggregated_logs_service.get_aggregated_logs(date_type, device_id=1, field_id=field.id, start_date=start_date, end_date=end_date)

        assert "400 Bad Request: Both start date and end date must be provided or none of them." in str(excinfo.value)

@patch('app.main.application.service.aggregated_logs_service.has_required_permission', return_value=True)
@pytest.mark.parametrize("date_type, page_number, page_size", [
    ("Weekly", 1, 0),  
    ("Monthly", 0, 1),
    ("Yearly", 0, 0),
    ("Weekly", -1, -1)
])
def test_get_aggregated_logs_invalid_pagination(app, aggregated_logs_service, date_type, page_number, page_size):
    with app.test_request_context():
        field = aggregated_logs_service.field_repository.create(Field(name='TestField'))
        with pytest.raises(Exception) as excinfo:
            aggregated_logs_service.get_aggregated_logs(date_type, device_id=1, field_id=field.id, page_number=page_number, page_size=page_size)

        assert "400 Bad Request: Page number and/or page size cannot be 0 or less." in str(excinfo.value)

@pytest.mark.parametrize("date_type, entity_type", [
    ('Weekly', WeeklyAverage),
    ('Monthly', MonthlyAverage),
    ('Yearly', YearlyAverage),
])
@patch('app.main.application.service.aggregated_logs_service.has_required_permission', return_value=True)
def test_get_aggregated_logs_valid_device_and_date_type_and_field_no_start_or_end_date(app, aggregated_logs_service, date_type, entity_type):
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
            field_id=created_field.id,
            reference_date=datetime(2022, 1, 3).date()
        )
        repository.create(aggregated_logs_entity)

        # Call the get_aggregated_logs method for Weekly/Monthly/Yearly
        aggregated_logs_result = aggregated_logs_service.get_aggregated_logs(date_type, device_id=1, field_id=created_field.id)

        assert isinstance(aggregated_logs_result, list)  # Check if the result is a list
        assert len(aggregated_logs_result) == 1  # Check if the result contains the inserted record
        assert date_type in ['Weekly', 'Monthly', 'Yearly']  # Check if date type is valid 

# Perform correct test using all 3 tables
@pytest.mark.parametrize("date_type, entity_type", [
    ('Weekly', WeeklyAverage),
    ('Monthly', MonthlyAverage),
    ('Yearly', YearlyAverage),
])
@patch('app.main.application.service.aggregated_logs_service.has_required_permission', return_value=True)
def test_get_aggregated_logs_valid_device_and_date_type_and_field_with_start_and_end_date(app, aggregated_logs_service, date_type, entity_type):
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
            field_id=created_field.id,
            reference_date=datetime(2022, 1, 3).date()
        )
        repository.create(aggregated_logs_entity)

        # Call the get_aggregated_logs method for Weekly/Monthly/Yearly
        # Create fake dates (based on previously entered reference date)
        start_date = str(datetime(2022, 1, 3).date())
        end_date = str(datetime(2022, 1, 3).date() + timedelta(days=1))
        aggregated_logs_result = aggregated_logs_service.get_aggregated_logs(date_type, device_id=1, field_id=created_field.id, start_date=start_date, end_date=end_date)

        assert isinstance(aggregated_logs_result, list)  # Check if the result is a list
        assert len(aggregated_logs_result) == 1  # Check if the result contains the inserted record
        assert date_type in ['Weekly', 'Monthly', 'Yearly']  # Check if date type is valid 