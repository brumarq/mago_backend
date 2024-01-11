Feature: Firmware

    Scenario: Create a Firmware FileSend record as an admin
        Given the user is logged in as an admin
        When the user attempts to create multiple FileSends for an existing device with id 1
        Then the response should contain status code 201
        And the user attempts to retrieve a list of created FileSends for device with id 1
        Then the response should contain status code 200
        And the response should contain a list of created FileSends
        
    Scenario: Create a Firmware FileSend record as a client
        Given the user is logged in as a client
        When the user attempts to create multiple FileSends for an existing device with id 1
        Then the response should contain status code 403
        
    Scenario: Create a Firmware FileSend record as an admin for a non-existent device
        Given the user is logged in as an admin
        When the user attempts to create multiple FileSends for an existing device with id -10
        Then the response should contain status code 404
        