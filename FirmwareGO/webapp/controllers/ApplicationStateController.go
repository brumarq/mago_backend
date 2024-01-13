package controllers

import (
	. "FirmwareGO/application/services"
	"FirmwareGO/webapp/custommetrics"
	"FirmwareGO/webapp/status"
	"github.com/gin-gonic/gin"
	"net/http"
)

type ApplicationStateController struct {
	ApplicationStateService *ApplicationStateService
}

func NewApplicationStateController(applicationStateService *ApplicationStateService) *ApplicationStateController {
	return &ApplicationStateController{ApplicationStateService: applicationStateService}
}

func (controller *ApplicationStateController) RegisterRoutes(router *gin.Engine) {
	router.GET("/health", controller.HealthCheck)
	router.GET("/ready", controller.ReadyCheck)
}

func (controller *ApplicationStateController) HealthCheck(context *gin.Context) {
	custommetrics.SetHealthStatus(true)
	context.JSON(http.StatusOK, gin.H{"status": "up"})
}

func (controller *ApplicationStateController) ReadyCheck(context *gin.Context) {
	if controller.ApplicationStateService.DbIsConnected() {
		if status.IsMigrationSuccessful() {
			custommetrics.SetReadinessStatus(true)
			context.JSON(http.StatusOK, gin.H{"status": "ready"})
		} else {
			custommetrics.SetReadinessStatus(false)
			context.JSON(http.StatusServiceUnavailable, gin.H{"status": "failed to apply pending database migrations."})
		}
	} else {
		custommetrics.SetReadinessStatus(false)
		context.JSON(http.StatusServiceUnavailable, gin.H{"status": "could not connect to database."})
	}
}
