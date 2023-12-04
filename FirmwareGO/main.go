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
	url := ginSwagger.URL("http://localhost:8080/swagger/doc.json")
	router.GET("/swagger/*any", ginSwagger.WrapHandler(swaggerFiles.Handler, url))

	// Register routes
	firmwareController.RegisterRoutes(router)

	// Start the server
	if err := router.Run(); err != nil {
		log.Fatal("Failed to run server: ", err)
	}
}
