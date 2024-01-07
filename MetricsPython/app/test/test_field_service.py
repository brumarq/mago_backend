import pytest
from unittest.mock import patch
import json
from flask import jsonify

@patch('app.main.application.service.field_service.has_required_permission', return_value=False)
def test_post_field_invalid_permissions(app, field_service):
    with app.test_request_context():
        with pytest.raises(Exception) as excinfo:
            data = {
                "name": 'Temperature',
                "unitId": 1,
                "deviceTypeId": 1,
                "loggable": True
            }
            field_service.create_field(data)
        assert "401 Unauthorized: This user does not have sufficient permissions" in str(excinfo.value)

@pytest.mark.parametrize("name", ['', None]) 
@patch('app.main.application.service.field_service.has_required_permission', return_value=True)
def test_post_field_invalid_name(app, field_service, name):
    with app.test_request_context():
        with pytest.raises(Exception) as excinfo:
            data = {
                "name": name,
                "unitId": 1,
                "deviceTypeId": 1,
                "loggable": True
            }
            field_service.create_field(data)
        assert '400 Bad Request: The field name is required.' in str(excinfo.value)


@pytest.mark.parametrize("unit_id", [0, -1, -2, -3]) 
@patch('app.main.application.service.field_service.has_required_permission', return_value=True)
def test_post_field_invalid_unit_id(app, field_service, unit_id):
    with app.test_request_context():
        with pytest.raises(Exception) as excinfo:
            data = {
                "name": 'Temperature',
                "unitId": unit_id,
                "deviceTypeId": 1,
                "loggable": True
            }
            field_service.create_field(data)
        assert '400 Bad Request: Unit id cannot be 0 or negative.' in str(excinfo.value)

@pytest.mark.parametrize("device_type_id", [0, -1, -2, -3]) 
@patch('app.main.application.service.field_service.has_required_permission', return_value=True)
def test_post_field_invalid_device_type_id(app, field_service, device_type_id):
    with app.test_request_context():
        with pytest.raises(Exception) as excinfo:
            data = {
                "name": 'Temperature',
                "unitId": 1,
                "deviceTypeId": device_type_id,
                "loggable": True
            }
            field_service.create_field(data)
        assert '400 Bad Request: Device type id cannot be 0 or negative.' in str(excinfo.value)


@pytest.mark.parametrize("name, unit_id, device_type_id, loggable", [
    ('ValidName', 1, 1, True),
    ('Another valid name', 2, 2, False)
])
@patch('app.main.application.service.field_service.has_required_permission', return_value=True)
def test_post_field_valid_data(app, field_service, name, unit_id, device_type_id, loggable):
    with app.test_request_context():
        data = {
            "name": name,
            "unitId": unit_id,
            "deviceTypeId": device_type_id,
            "loggable": loggable
        }
        response = field_service.create_field(data)

        expected_response = jsonify(data)

        # Compare expected json with current json
        assert json.loads(response.get_data(as_text=True)) == json.loads(expected_response.get_data(as_text=True))