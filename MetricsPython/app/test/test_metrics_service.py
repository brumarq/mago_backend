import pytest
from app.main.domain.entities.log_value import LogValue
from app.main.domain.entities.log_collection import LogCollection
from app.main.domain.entities.field import Field
from unittest.mock import patch

@patch('app.main.application.service.metrics_service.has_required_permission', return_value=False)
def test_get_device_metrics_invalid_permissions(app, metrics_service):
    with app.test_request_context():
        with pytest.raises(Exception) as excinfo:
            metrics_service.get_latest_device_metrics_by_device_id(device_id=1)
        assert "401 Unauthorized: This user does not have sufficient permissions" in str(excinfo.value)

@patch('app.main.application.service.metrics_service.has_required_permission', return_value=True)
def test_get_device_metrics_invalid_device_id(app, metrics_service):
    with app.test_request_context():
        with pytest.raises(Exception) as excinfo:
            metrics_service.get_latest_device_metrics_by_device_id(device_id=0)
        assert '400 Bad Request: Device id cannot be 0 or negative!' in str(excinfo.value)

@patch('app.main.application.service.metrics_service.has_required_permission', return_value=True)
@pytest.mark.parametrize("device_id, page_number, page_size", [
    (1, 1, 0),  
    (1, 0, 1),
    (1, 0, 0),
    (1, -1, -1)
])
def test_get_device_metrics_invalid_magination(app, metrics_service, device_id, page_number, page_size):
    with app.test_request_context():
        with pytest.raises(Exception) as excinfo:
            metrics_service.get_latest_device_metrics_by_device_id(device_id=device_id, page_number=page_number, page_size=page_size)
        assert "400 Bad Request: Page number and/or page size cannot be 0 or less." in str(excinfo.value)

@patch('app.main.application.service.metrics_service.has_required_permission', return_value=True)
@pytest.mark.parametrize("repository", [LogValue], indirect=True)
def test_get_device_metrics_valid_request(app, metrics_service, repository):
    with app.test_request_context():
        # Insert some test data into the repository

        # Create field and insert
        field_entity = Field(name='TestField')
        created_field = repository.create(field_entity)

        # Create log collection and insert
        log_collection_entity = LogCollection(device_id=1)
        created_log_collection = repository.create(log_collection_entity)

        # Create log value and insert
        log_value_entity = LogValue(value=42.0, field=created_field, log_collection=created_log_collection)
        repository.create(log_value_entity)

        # Call the get_device_metrics_by_device method and get data
        device_metrics = metrics_service.get_latest_device_metrics_by_device_id(device_id=1)

        # Assert
        assert isinstance(device_metrics, list)  # Check if the result is a list
        assert len(device_metrics) == 1  # Check if the result contains the inserted record
        assert device_metrics[0].value == 42.0  # Check if the values match
        assert device_metrics[0].field.name == 'TestField'  # Check if the field relationship is correct