import unittest
from app.test.test_base import BaseTest

def get_daily_aggregated_logs(self):
    return self.client.get('/metrics/aggregated-logs/Daily')

def get_yearly_aggregated_logs(self):
    return self.client.get('metrics/aggregated-logs/Yearly')


class AggregatedLogTest(BaseTest):
  
    def test_status_code_and_message_for_wrong_date_type_entered(self):
        with self.client:
            response = get_daily_aggregated_logs(self)
            self.assertEqual(response.status_code, 400)
            self.assertEqual(response.get_json()['message'], "Invalid date type entered (must be 'Weekly', 'Monthly' or 'Yearly').")

    def test_status_code_for_correct_date_type_entered(self):
        with self.client:
            response = get_yearly_aggregated_logs(self)
            self.assertEqual(response.status_code, 200, "Expected status code 200, but got {}".format(response.status_code))

    def test_if_response_empty_for_correct_date_type_entered(self):
        with self.client:
            response = get_yearly_aggregated_logs(self)

            self.assertEqual(response.status_code, 200)

            response_data = response.get_json()
            
            self.assertEqual(response_data, [])
            

if __name__ == '__main__':
    unittest.main()