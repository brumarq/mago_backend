package database

import (
	"gorm.io/gorm"
)

type FirmwareDbContext struct {
	DB *gorm.DB
}

func NewFirmwareDbContext(db *gorm.DB) *FirmwareDbContext {
	return &FirmwareDbContext{DB: db}
}
