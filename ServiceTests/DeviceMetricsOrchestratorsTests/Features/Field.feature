Feature: Field

    Scenario: Creating a field as a an invalid user
        Given the user is logged in as invalidUser
        When the user tries to create a field object for unit id 1 and device type id 1
        Then the response should return 401

    Scenario: Creating a field as a client
        Given the user is logged in as client
        When the user tries to create a field object for unit id 1 and device type id 1
        Then the response should return 403

    Scenario: Creating a field as an admin for invalid unit id
        Given the user is logged in as admin
        When the user tries to create a field object for unit id -10 and device type id 1
        Then the response should return 404

    Scenario: Creating a field as an admin for invalid device type id
        Given the user is logged in as admin
        When the user tries to create a field object for unit id 1 and device type id -10
        Then the response should return 404

    Scenario: Creating a field as an admin
        Given the user is logged in as admin
        When the user tries to create a field object for unit id 1 and device type id 1
        Then the response should return 201
        And the created field object should be returned