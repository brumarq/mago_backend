import unittest
from app.test.test_base import BaseTest
from app.main.domain.enums.aggregated_log_date_type import AggregatedLogDateType


def get_aggregated_logs(self, date_type):
    return self.client.get(f'/metrics/aggregated-logs/{date_type}')


class AggregatedLogTestApi(BaseTest):
  
    def test_invalid_date_type_should_return_400_and_proper_message(self):
        with self.client:
            response = get_aggregated_logs(self, "Daily")
            self.assertEqual(response.status_code, 400)
            self.assertEqual(response.get_json()['message'], "Invalid date type entered (must be 'Weekly', 'Monthly' or 'Yearly').")

    def test_valid_yearly_date_type_should_return_200(self):
        with self.client:
            response = get_aggregated_logs(self, "Yearly")
            self.assertEqual(response.status_code, 200, "Expected status code 200, but got {}".format(response.status_code))

    def test_content_type_should_return_json(self):
        with self.client:
            response = get_aggregated_logs(self, "Weekly")
            self.assertEqual(response.status_code, 200)
            self.assertEqual(response.headers['Content-Type'], 'application/json')

    def test_valid_date_type_should_return_true_for_valid_enum_value(self):
        with self.client:
            response = get_aggregated_logs(self, 'Monthly')

            # Extract the date_type from the URL
            url_parts = response.request.path.split('/')
            date_type = url_parts[-1]

            self.assertTrue(any(date_type == item.value for item in AggregatedLogDateType))
                

if __name__ == '__main__':
    unittest.main()