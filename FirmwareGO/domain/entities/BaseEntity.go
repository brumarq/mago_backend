package entities

import (
	"gorm.io/gorm"
	"time"
)

type BaseEntity struct {
	ID        int `gorm:"primaryKey"`
	CreatedAt time.Time
	UpdatedAt time.Time
}

func (be *BaseEntity) IsEntity() {}

// BeforeCreate - GORM hook
func (be *BaseEntity) BeforeCreate(tx *gorm.DB) (err error) {
	now := time.Now().UTC()
	be.CreatedAt = now
	be.UpdatedAt = now
	return
}

// BeforeUpdate - GORM hook
func (be *BaseEntity) BeforeUpdate(tx *gorm.DB) (err error) {
	be.UpdatedAt = time.Now().UTC()
	return
}
