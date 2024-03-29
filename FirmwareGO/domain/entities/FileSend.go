package entities

import (
	"FirmwareGO/domain/entities/interfaces"
	_ "gorm.io/gorm"
)

type FileSend struct {
	BaseEntity           //Embedded
	UpdateStatus *string `gorm:"size:255;default:New"`
	DeviceId     int
	File         *string `gorm:"size:255"`
	CurrPart     int
	TotParts     int
}

var _ interfaces.IEntity = (*FileSend)(nil)
