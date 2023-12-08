import unittest
from app.test.test_base import BaseTest

def get_daily_aggregated_logs(self):
    return self.client.get('/metrics/aggregated-logs/Daily')

def get_yearly_aggregated_logs(self):
    return self.client.get('metrics/aggregated-logs/Yearly')


class AggregatedLogTestApi(BaseTest):
  
    def test_invalid_date_type_should_return_400_and_proper_message(self):
        with self.client:
            response = get_daily_aggregated_logs(self)
            self.assertEqual(response.status_code, 400)
            self.assertEqual(response.get_json()['message'], "Invalid date type entered (must be 'Weekly', 'Monthly' or 'Yearly').")

    def test_valid_yearly_date_type_should_return_200(self):
        with self.client:
            response = get_yearly_aggregated_logs(self)
            self.assertEqual(response.status_code, 200, "Expected status code 200, but got {}".format(response.status_code))

    def test_content_type_should_return_json(self):
        with self.client:
            response = get_yearly_aggregated_logs(self)
            self.assertEqual(response.status_code, 200)
            self.assertEqual(response.headers['Content-Type'], 'application/json')
            

if __name__ == '__main__':
    unittest.main()