import unittest
from app.test.test_base import BaseTest

def get_device_metrics_for_device_1(self):
    return self.client.get('/metrics/devices/1')

def get_device_metrics_for_device_0(self):
    return self.client.get('/metrics/devices/0')


class MetricsTestApi(BaseTest):
  
    # def test_metrics_for_device_1_should_not_be_empty(self):
    #     with self.client:
    #         response = get_device_metrics_for_device_1(self)
    #         self.assertEqual(response.status_code, 200)

    #         metrics_data = response.get_json()

    #         self.assertNotEqual(metrics_data, [])

    # def test_metrics_for_device_1_should_contain_more_than_zero_entries(self):
    #     with self.client:
    #         response = get_device_metrics_for_device_1(self)

    #         self.assertEqual(response.status_code, 200)

    #         metrics_data = response.get_json()

    #         self.assertIsInstance(metrics_data, list)
    #         self.assertGreater(len(metrics_data), 0)

    def test_metrics_for_device_0_should_raise_400_and_provide_proper_error_message(self):
        with self.client:
            response = get_device_metrics_for_device_0(self)

            # Check if the response has a 400 status code
            self.assertEqual(response.status_code, 400)

            # Assuming the response is in JSON format
            response_data = response.get_json()

            # Check if the error message is as expected
            self.assertEqual(response_data['message'], "Device id cannot be 0 or negative!")
    
    def test_metrics_for_device_0_should_have_content_type_is_present_and_is_application_json(self):
        with self.client:
            response = get_device_metrics_for_device_0(self)

            # Check if the 'Content-Type' header is present
            self.assertIn('Content-Type', response.headers)

            # Check if the 'Content-Type' is 'application/json'
            self.assertEqual(response.headers['Content-Type'], 'application/json')
            

if __name__ == '__main__':
    unittest.main()