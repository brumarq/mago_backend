Feature: DeviceService Features
        In order to manage devices
        As a software developer
        I want to ensure that the device service behaves correctly under various conditions
    
    Scenario: Ensure Device Exists throws HttpRequestException when device service is down
        Given the device service is down
        When I call EnsureDeviceExists
        Then an HttpRequestException should be thrown
        And response status code should be 503 (Service Unavailable)