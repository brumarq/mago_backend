Feature: Notifications
	
	Scenario: Retrieve notifications for Device
		Given the request is set to User Device Notification Orchestrator
		When a get request is made to get a notification
		Then the response code should be 200
		Then an item with id 1 should exist
