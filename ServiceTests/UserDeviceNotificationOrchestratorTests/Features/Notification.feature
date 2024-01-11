Feature: Notifications
	
	Scenario: Retrieve notifications for Device
		Given the request is set to User Device Notification Orchestrator
		When a get request is made to get notification by device
		Then the response code should be 200
		Then an item with id 1 should exist
		
	Scenario: Retrieve notifications for non-existant Device
		Given the request is set to User Device Notification Orchestrator
		When a get request is made to get notification by device when device does not exist
		Then the response code should be 404
		
	Scenario: Retrieve notification that exists
		Given the request is set to User Device Notification Orchestrator
		When a get request is made to get notification
		Then the response code should be 200
		Then an object with id 1 should exist
		
	Scenario: Retrieve notification that does not exists
		Given the request is set to User Device Notification Orchestrator
		When a get request is made to get notification that does not exist
		Then the response code should be 404
	
	Scenario: Create a notification and verify response properties
		Given the request is set to User Device Notification Orchestrator
		When a post request is made to create a notification with existing device and notification type
		Then the response code should be 200
		And the response object should contain "statusTypeId" with value "1"
		And the response object should contain "message" with value "Machine is down"
		And the response object should contain "deviceID" with value "1"
		
	Scenario: Create a notification with a non-existent device id
		Given the request is set to User Device Notification Orchestrator
		When a post request is made to create a notification with non-existent device
		Then the response code should be 404
		
	Scenario: Create a notification with a non-existent notification type id
		Given the request is set to User Device Notification Orchestrator
		When post request is made to create a notification with non-existent notification type
		Then the response code should be 404
		
	
