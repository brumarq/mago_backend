// @title FirmwareGO API
// @description This is a Microservice for managing Mago Bio Solutions Composter Firmware
// @version 1.0
// @BasePath /
// @securityDefinitions.apikey JWTAuth
// @in header
// @name Authorization
package main

import (
	"FirmwareGO/application/services"
	_ "FirmwareGO/docs"
	. "FirmwareGO/domain/entities"
	"FirmwareGO/infrastructure/repositories"
	"FirmwareGO/webapp/controllers"
	"fmt"
	"github.com/gin-gonic/gin"
	swaggerFiles "github.com/swaggo/files"
	ginSwagger "github.com/swaggo/gin-swagger"
	"gorm.io/driver/sqlserver"
	"gorm.io/gorm"
	"log"
	"os"
)

func main() {
	dsn := os.Getenv("FIRMWARE_DB_CONNECTION_STRING")
	fmt.Println("DSN: ", dsn)

	db, err := gorm.Open(sqlserver.Open(dsn), &gorm.Config{})
	if err != nil {
		log.Fatal("Failed to connect to database", err)
	}

	db.AutoMigrate(&FileSend{})

	// Initialize Repository
	repository := repositories.NewRepository[*FileSend](db)

	// Initialize Services
	firmwareService := services.NewFirmwareService(repository)

	// Initialize Controller
	firmwareController := controllers.NewFirmwareController(firmwareService)

	// Set up the router
	router := gin.Default()

	// Register swagger endpoint
	router.GET("/swagger/*any", func(c *gin.Context) {
		ginSwagger.WrapHandler(swaggerFiles.Handler,
			ginSwagger.URL(fmt.Sprintf("%s://%s/swagger/doc.json", c.Request.URL.Scheme, c.Request.Host)))(c)
	})

	// Register routes
	firmwareController.RegisterRoutes(router)

	// Start the server
	if err := router.Run(":6969"); err != nil {
		log.Fatal("Failed to run server: ", err)
	}
}
