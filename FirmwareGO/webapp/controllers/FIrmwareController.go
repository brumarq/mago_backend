package controllers

import (
	. "FirmwareGO/application/dtos"
	. "FirmwareGO/application/services"
	. "FirmwareGO/webapp/middleware/auth"
	"github.com/gin-gonic/gin"
	"net/http"
	"strconv"
)

type FirmwareController struct {
	FirmwareService *FirmwareService
}

func NewFirmwareController(firmwareService *FirmwareService) *FirmwareController {
	return &FirmwareController{FirmwareService: firmwareService}
}

func (controller *FirmwareController) RegisterRoutes(router *gin.Engine) {
	secureGroup := router.Group("/firmware")
	secureGroup.Use(
		JwtMiddlewareHandler(EnsureValidJwt()),
		JwtMiddlewareHandler(EnsureAdminPermission()),
	)

	router.GET("/health", controller.HealthCheck)
	router.GET("/ready", controller.ReadyCheck)
	secureGroup.POST("", controller.CreateFirmwareFileSend)
	secureGroup.GET("/devices/:deviceId", controller.GetFirmwareHistoryForDevice)
}

func (controller *FirmwareController) HealthCheck(context *gin.Context) {
	context.JSON(http.StatusOK, gin.H{"status": "up"})
}

func (controller *FirmwareController) ReadyCheck(context *gin.Context) {
	context.JSON(http.StatusOK, gin.H{"status": "ready"})
}

// CreateFirmwareFileSend godoc
// @Summary Create a new firmware file send record
// @Description creates a new file send record for firmware
// @Tags firmware
// @Accept json
// @Produce json
// @Security JWTAuth
// @Param createFileSendDTO body CreateFileSendDTO true "Create File Send Data"
// @Success 200 {object} FileSendResponseDTO "Successfully created firmware file send record"
// @Failure 400 {object} map[string]string "Bad Request"
// @Failure 500 {object} map[string]string "Internal Server Error"
// @Router /firmware [post]
func (controller *FirmwareController) CreateFirmwareFileSend(context *gin.Context) {
	var createFileSendDTO CreateFileSendDTO

	if err := context.ShouldBindJSON(&createFileSendDTO); err != nil {
		context.JSON(http.StatusBadRequest, gin.H{"Bad request": err.Error()})
		return
	}

	result, err := controller.FirmwareService.CreateFileSend(context, createFileSendDTO)
	if err != nil {
		context.JSON(http.StatusInternalServerError, gin.H{"Internal server error": err.Error()})
	}

	if result == nil {
		context.JSON(http.StatusInternalServerError, gin.H{"Internal server error": "The Firmware Record could not be created."})
	}

	context.JSON(http.StatusCreated, result)
}

// GetFirmwareHistoryForDevice godoc
// @Summary Get firmware history for a device
// @Description retrieves the firmware update history for a specific device
// @Tags firmware
// @Accept json
// @Produce json
// @Security JWTAuth
// @Param deviceId path int true "Device ID"
// @Success 200 {array} FileSendResponseDTO "List of firmware records"
// @Failure 404 {object} map[string]string "Firmware Record not found"
// @Failure 500 {object} map[string]string "Internal server error"
// @Router /firmware/devices/{deviceId} [get]
func (controller *FirmwareController) GetFirmwareHistoryForDevice(context *gin.Context) {
	deviceId, _ := strconv.Atoi(context.Param("deviceId"))

	result, err := controller.FirmwareService.GetFirmwareHistoryByDeviceId(context, deviceId)
	if err != nil {
		context.JSON(http.StatusInternalServerError, gin.H{"Internal server error": err.Error()})
		return
	}

	if result == nil {
		context.JSON(http.StatusNotFound, gin.H{"Message": "Firmware Record not found"})
		return
	}

	context.JSON(http.StatusOK, result)
}
