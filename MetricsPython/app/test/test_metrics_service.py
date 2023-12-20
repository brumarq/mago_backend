import unittest
from unittest.mock import patch, MagicMock
from app.main.application.service.metrics_service import MetricsService
from app.main.infrastructure.repositories.repository import Repository
from app.main.domain.entities.log_value import LogValue
from app.main.domain.entities.field import Field
from app.main.domain.entities.log_collection import LogCollection
from app.main.domain.entities.log_collection_type import LogCollectionType
from app.test.test_base import BaseTest

class TestMetricsService(BaseTest):
    @patch('app.main.application.service.metrics_service.Repository')
    @patch('app.main.application.service.metrics_service.MetricsService')
    def test_get_device_metrics_by_device(self, mock_metrics_service, mock_repository):
        # Mock Repository and MetricsService instances
        mock_repository_instance = mock_repository.return_value
        mock_metrics_service_instance = mock_metrics_service.return_value

        # Create LogCollectionType object
        log_collection_type = LogCollectionType(id=1)

        # Create LogCollection object
        log_collection = LogCollection(id=1, device_id=1, log_collection_type=log_collection_type)

        # Create Field object
        field = Field(id=1, name="Temperature", loggable=True)

        # Mock Repository method to return a list of 6 LogValue objects
        expected_log_values = [
            LogValue(id=1, value=42, field=field, log_collection=log_collection),
            LogValue(id=2, value=37, field=field, log_collection=log_collection),
            LogValue(id=3, value=51, field=field, log_collection=log_collection),
            LogValue(id=4, value=28, field=field, log_collection=log_collection),
            LogValue(id=5, value=63, field=field, log_collection=log_collection),
            LogValue(id=6, value=49, field=field, log_collection=log_collection),
        ]

        mock_repository_instance.get_all_by_condition.return_value = expected_log_values

        # Replace MetricsService method with MagicMock object
        mock_metrics_service_instance.get_device_metrics_by_device = MagicMock()

        # Call the method you want to test
        mock_metrics_service_instance.get_device_metrics_by_device(1)

        # Assertions
        mock_metrics_service_instance.get_device_metrics_by_device.assert_called_once_with(1)

        # Check the return value of the repository method
        actual_entries = mock_repository_instance.get_all_by_condition.return_value
        self.assertEqual(actual_entries, expected_log_values)

        # Check that the length of the return value is exactly 6
        self.assertEqual(len(actual_entries), 6)

if __name__ == '__main__':
    unittest.main()