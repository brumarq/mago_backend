import unittest
from app.test.test_base import BaseTest
import json


def export_aggregated_logs_csv(self, file_name: str, aggregated_log_date_type: str):
    return self.client.post(
        '/metrics/aggregated-logs/export-csv',
        data=json.dumps(dict(
            file_name=file_name,
            aggregated_log_date_type=aggregated_log_date_type
        )),
        content_type="application/json"
    )


class ExportAggregatedLogsCsvTestApi(BaseTest):
  
    def test_providing_invalid_file_name_should_return_400_and_proper_error_message(self):
        response = export_aggregated_logs_csv(self, "my_csv-data", "Weekly")
        self.assertEqual(response.status_code, 400)
        self.assertEqual(response.get_json()['message'], f"Filename 'my_csv-data' is invalid. Please provide a valid filename.")

    def test_providing_invalid_aggregated_log_date_type_should_return_400_and_proper_error_message(self):
        response = export_aggregated_logs_csv(self, "mycsvdata", "Hello")
        self.assertEqual(response.status_code, 400)
        self.assertEqual(response.get_json()['message'], "Invalid date type entered (must be 'Weekly', 'Monthly' or 'Yearly').")

    def test_successful_export_should_return_201_and_csv_file(self):
        response = export_aggregated_logs_csv(self, "mycsvdata", "Weekly")  # Use the same file_name used in the call
        self.assertEqual(response.status_code, 201)
        self.assertEqual(response.headers["Content-Disposition"], "attachment; filename=mycsvdata.csv")  # Update the expected filename
        self.assertEqual(response.headers["Content-Type"], "application/octet-stream")
        self.assertEqual(response.headers["Content-Transfer-Encoding"], "bytes")
              

if __name__ == '__main__':
    unittest.main()