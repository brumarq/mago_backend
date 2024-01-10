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
