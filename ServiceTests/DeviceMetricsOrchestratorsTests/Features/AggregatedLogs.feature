Feature: AggregatedLogs

    Scenario: Retrieving aggregated logs as an admin for Weekly
        Given the user is logged in as admin
        When the user tries to retrieve aggregated logs for Weekly with device id 1 and field id 1 and startDate empty and endDate empty
        Then the response should return 200
        And the response should return a list of aggregated logs with 2 entries

    Scenario: Retrieving aggregated logs as a client owner of device for Monthly
        Given the user is logged in as client
        When the user tries to retrieve aggregated logs for Monthly with device id 1 and field id 1 and startDate empty and endDate empty
        Then the response should return 200
        And the response should return a list of aggregated logs with 2 entries

    Scenario: Retrieving aggregated logs as a client owner of device for Yearly
        Given the user is logged in as client
        When the user tries to retrieve aggregated logs for Yearly with device id 1 and field id 1 and startDate empty and endDate empty
        Then the response should return 200
        And the response should return a list of aggregated logs with 2 entries

    Scenario: Retrieving aggregated logs as an admin with valid startDate and valid endDate for Weekly
        Given the user is logged in as admin
        When the user tries to retrieve aggregated logs for Weekly with device id 1 and field id 1 and startDate 2024-01-01 and endDate 2024-01-08
        Then the response should return 200
        And the response should return a list of aggregated logs with 2 entries

    Scenario: Retrieving aggregated logs as an admin with valid startDate and valid endDate for Monthly
        Given the user is logged in as admin
        When the user tries to retrieve aggregated logs for Monthly with device id 1 and field id 1 and startDate 2024-01-01 and endDate 2024-01-01
        Then the response should return 200
        And the response should return a list of aggregated logs with 1 entries

    Scenario: Retrieving aggregated logs as an admin with invalid date type
        Given the user is logged in as admin
        When the user tries to retrieve aggregated logs for InvalidDate with device id 1 and field id 1 and startDate empty and endDate empty
        Then the response should return 400

    Scenario: Retrieving aggregated logs as an admin with invalid device id 
        Given the user is logged in as admin
        When the user tries to retrieve aggregated logs for Weekly with device id -10 and field id 1 and startDate empty and endDate empty
        Then the response should return 404

    Scenario: Retrieving aggregated logs as an admin with invalid field id
        Given the user is logged in as admin
        When the user tries to retrieve aggregated logs for Weekly with device id 1 and field id 0 and startDate empty and endDate empty
        Then the response should return 400

    Scenario: Retrieving aggregated logs as an admin with valid startDate but no endDate
        Given the user is logged in as admin
        When the user tries to retrieve aggregated logs for Weekly with device id 1 and field id 1 and startDate 2024-01-01 and endDate empty
        Then the response should return 400

    Scenario: Retrieving aggregated logs as an admin with no startDate but valid endDate
        Given the user is logged in as admin
        When the user tries to retrieve aggregated logs for Weekly with device id 1 and field id 1 and startDate empty and endDate 2024-01-01
        Then the response should return 400

    Scenario: Retrieving aggregated logs as an invalid user
        Given the user is logged in as invalidUser
        When the user tries to retrieve aggregated logs for Weekly with device id 1 and field id 1 and startDate empty and endDate empty
        Then the response should return 401

    Scenario: Retrieving aggregated logs as a client that does not own the device
        Given the user is logged in as forbiddenClient
        When the user tries to retrieve aggregated logs for Weekly with device id 1 and field id 1 and startDate empty and endDate empty
        Then the response should return 403

   Scenario: Retrieving aggregated logs as an admin with invalid startDate and invalid endDate
        Given the user is logged in as admin
        When the user tries to retrieve aggregated logs for Weekly with device id 1 and field id 1 and startDate invalidStartDate and endDate invalidEndDate
        Then the response should return 400