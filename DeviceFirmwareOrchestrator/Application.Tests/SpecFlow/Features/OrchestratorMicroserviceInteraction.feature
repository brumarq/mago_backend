Feature: FirmwareServiceIntegration
In order to ensure reliable firmware management
As a firmware service
I want to interact correctly with the device service

    Scenario: Successfully creating a new firmware send request for an existing device
        Given a device with ID 1 exists
        When a new firmware send request is created for device 1
        Then the post firmware FileSend request should be processed successfully with status code 201
        
    Scenario: Attempting to create a new firmware send request for a non-existing device
        Given a device with ID 999 does not exist
        When a new firmware send request is created for device 999
        Then a NotFoundException should be thrown