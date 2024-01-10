Feature: Firmware

    Scenario: Full Firmware FileSend workflow for admin user
        Given the user is logged in as an admin
        When the user creates multiple FileSends for an existing device with id 1
        Then the user should be able to retrieve a list of created FileSends for device with id 1