package dtos

import "time"

type BaseDTO struct {
	ID        int       `json:"id"`
	CreateAt  time.Time `json:"createAt"`
	UpdatedAt time.Time `json:"updatedAt"`
}
