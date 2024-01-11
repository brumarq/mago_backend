Feature: Metrics

Scenario: Retrieve device metrics by device id
    Given the device id provided is 1
    When a get request is called to retrieve device metrics
    Then the result should return 200
    And the result should contain 1 entry


Scenario: Retrieve aggregated logs by aggregation date, device id and field id
    Given the aggregation date Yearly is provided
    And device id of 1 is provided
    And field id of 1 is provided
    When a get request is called to retrieved aggregated logs
    Then the result should return 200
    And result should contain 8 entries


Scenario: Creating a field in metrics
    Given a field object is provided
    When a post request is called to create a field
    Then the result should return 201
    And a new object has to be added to the list