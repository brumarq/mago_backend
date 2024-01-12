Feature: Metrics
    
    # Device metrics tests

    Scenario: Retrieving latest device metrics as an admin
        Given the user is logged in as admin
        When the user tries to retrieve latest device metrics for device id 1
        Then the response should return 200
        And the response should return a list of latest device metrics with 2 entries

    Scenario: Retrieving latest device metrics as a client owner of device
        Given the user is logged in as client
        When the user tries to retrieve latest device metrics for device id 1
        Then the response should return 200
        And the response should return a list of latest device metrics with 2 entries

    Scenario: Retrieving latest device metrics as an invalid user
        Given the user is logged in as invalidUser
        When the user tries to retrieve latest device metrics for device id 1
        Then the response should return 401

    Scenario: Retrieving latest device metrics as a client owner of device with invalid device id
        Given the user is logged in as client
        When the user tries to retrieve latest device metrics for device id -10
        Then the response should return 404

    Scenario: Retrieving latest device metrics as a client that does not own the device
        Given the user is logged in as forbiddenClient
        When the user tries to retrieve latest device metrics for device id 1
        Then the response should return 403