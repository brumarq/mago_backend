Feature: Metrics

    Scenario: Creating a field as an admin
        Given the user is logged in as an admin
        When the user tries to create a field object for unit id 1 and device type 1
        Then the response should return 201


    Scenario: Creating a field as a client
        Given the user is logged in as a client
        When the user tries to create a field object for unit id 1 and device type id 1
        Then the response should return 403


    Scenario: Creating a field as a client for invalid unit id
        Given the user is logged in as a client
        When the user tries to create a field object for unit id -10 and device type 1
        Then the response should return 400