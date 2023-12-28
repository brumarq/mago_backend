import pytest
from app.main.domain.entities.log_value import LogValue
from app.main.domain.entities.log_collection import LogCollection
from app.main.domain.entities.field import Field
import json
from unittest.mock import patch

@patch('app.main.application.service.metrics_service.has_required_permission', return_value=False)
def test_get_device_metrics_invalid_permissions(app, metrics_service):
    with app.test_request_context():
        with pytest.raises(Exception) as excinfo:
            metrics_service.get_device_metrics_by_device(device_id=1)
        assert "401 Unauthorized: This user does not have sufficient permissions" in str(excinfo.value)

@patch('app.main.application.service.metrics_service.has_required_permission', return_value=True)
def test_get_device_metrics_invalid_device_id(app, metrics_service):
    with app.test_request_context():
        with pytest.raises(Exception) as excinfo:
            metrics_service.get_device_metrics_by_device(device_id=0)
        assert '400 Bad Request: Device id cannot be 0 or negative!' in str(excinfo.value)

@patch('app.main.application.service.metrics_service.has_required_permission', return_value=True)
@pytest.mark.parametrize("repository", [LogValue], indirect=True)
def test_get_device_metrics_valid_request(app, metrics_service, repository):
    with app.test_request_context():
        # Insert some test data into the repository
        field_entity = Field(name='TestField')
        created_field = repository.create(field_entity)

        log_collection_entity = LogCollection(device_id=1)
        created_log_collection = repository.create(log_collection_entity)

        log_value_entity = LogValue(value=42.0, field=created_field, log_collection=created_log_collection)
        repository.create(log_value_entity)

        # Call the get_device_metrics_by_device method
        device_metrics = metrics_service.get_device_metrics_by_device(device_id=1)

        # Assert
        assert isinstance(device_metrics, list)  # Check if the result is a list
        assert len(device_metrics) == 1  # Check if the result contains the inserted record
        assert device_metrics[0].value == 42.0  # Check if the values match
        assert device_metrics[0].field.name == 'TestField'  # Check if the field relationship is correct