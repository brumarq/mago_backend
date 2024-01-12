Feature: Device

    Scenario: 1. Assign user to device
        Given I am logged in as admin
        Given the request is set to User Device Notification Orchestrator
        When a post request is made to /orchestrator/device/user-on-device with payload:
        """
        {
            "userId": "auth0|65a132fad4b8f0d7b40e6e8a",
            "deviceId": 5
        }
        """
        Then the response code should be 200
        
    Scenario: 2. Un-assign user to device
        Given I am logged in as admin
        Given the request is set to User Device Notification Orchestrator
        When a delete request is made to /orchestrator/device/user-on-device/auth0%7C65a132fad4b8f0d7b40e6e8a/5
        Then the response code should be 200
        
    Scenario: 3. Assign user to device as client
        Given I am logged in as client
        Given the request is set to User Device Notification Orchestrator
        When a post request is made to /orchestrator/device/user-on-device with payload:
        """
        {
            "userId": "auth0|65a132fad4b8f0d7b40e6e8a",
            "deviceId": 5
        }
        """
        Then the response code should be 403
    
    Scenario: 4. Un-assign user to device as a client
        Given I am logged in as client
        Given the request is set to User Device Notification Orchestrator
        When a delete request is made to /orchestrator/device/user-on-device/auth0%7C65a132fad4b8f0d7b40e6e8a/5
        Then the response code should be 403
        
        
    Scenario: 5. Assign user to non-existent device
        Given I am logged in as admin
        Given the request is set to User Device Notification Orchestrator
        When a post request is made to /orchestrator/device/user-on-device with payload:
        """
        {
            "userId": "auth0|65a132fad4b8f0d7b40e6e8a",
            "deviceId": 99999
        }
        """
        Then the response code should be 404
    
    Scenario: 6. Assign user to non-existent user
        Given I am logged in as admin
        Given the request is set to User Device Notification Orchestrator
        When a post request is made to /orchestrator/device/user-on-device with payload:
        """
        {
            "userId": "auth0|65a132fad4b5f3d1b20e3r8a",
            "deviceId": 5
        }
        """
        Then the response code should be 404