Feature: Notifications

    Scenario: Retrieve notifications for Device
        Given I am logged in as admin
        Given the request is set to User Device Notification Orchestrator
        When a get request is made to /orchestrator/notification/device/1
        Then the response code should be 200
        And an item with id 1 should exist
    
    Scenario: Retrieve notifications for Device as client
        Given I am logged in as client
        Given the request is set to User Device Notification Orchestrator
        When a get request is made to /orchestrator/notification/device/1
        Then the response code should be 200
        And an item with id 1 should exist
        
    Scenario: Retrieve notifications for Device as client unauthorized
        Given I am logged in as forbiddenClient
        Given the request is set to User Device Notification Orchestrator
        When a get request is made to /orchestrator/notification/device/1
        Then the response code should be 401

    Scenario: Retrieve notifications for non-existent Device
        Given I am logged in as admin
        Given the request is set to User Device Notification Orchestrator
        When a get request is made to /orchestrator/notification/device/99999
        Then the response code should be 404

    Scenario: Retrieve notification that exists
        Given I am logged in as admin
        Given the request is set to User Device Notification Orchestrator
        When a get request is made to /orchestrator/notification/1
        Then the response code should be 200
        And an object with id 1 should exist

    Scenario: Retrieve notification that does not exist
        Given I am logged in as admin
        Given the request is set to User Device Notification Orchestrator
        When a get request is made to /orchestrator/notification/99999
        Then the response code should be 404
        
    Scenario: Retrieve notification that exists as client
        Given I am logged in as client
        Given the request is set to User Device Notification Orchestrator
        When a get request is made to /orchestrator/notification/1
        Then the response code should be 200
    
    Scenario: Retrieve notification that exists as client unauthorized
        Given I am logged in as forbiddenClient
        Given the request is set to User Device Notification Orchestrator
        When a get request is made to /orchestrator/notification/1
        Then the response code should be 403

    Scenario: Create a notification and verify response properties
        Given I am logged in as admin
        Given the request is set to User Device Notification Orchestrator
        When a post request is made to /orchestrator/notification with payload:
        """
        {
            "deviceID": 1,
            "statusTypeID": 1,
            "message": "Machine is down"
        }
        """
        Then the response code should be 200
        And the response object should contain "statusTypeId" with value "1"
        And the response object should contain "message" with value "Machine is down"
        And the response object should contain "deviceID" with value "1"

    Scenario: Create a notification with a non-existent device id
        Given I am logged in as admin
        Given the request is set to User Device Notification Orchestrator
        When a post request is made to /orchestrator/notification with payload:
        """
        {
            "deviceID": 99999,
            "statusTypeID": 1,
            "message": "Machine is down"
        }
        """
        Then the response code should be 404

    Scenario: Create a notification with a non-existent notification type id
        Given I am logged in as admin
        Given the request is set to User Device Notification Orchestrator
        When a post request is made to /orchestrator/notification with payload:
        """
        {
            "deviceID": 1,
            "statusTypeID": 99999,
            "message": "Machine is down"
        }
        """
        Then the response code should be 404
