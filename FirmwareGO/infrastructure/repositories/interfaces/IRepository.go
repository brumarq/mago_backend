package interfaces

import (
	. "FirmwareGO/domain/entities/interfaces"
	"context"
)

// IRepository is a generic interface for data operations
type IRepository[T IEntity] interface {
	Create(ctx context.Context, entity *T) error
	GetAll(ctx context.Context) ([]T, error)
	GetCollectionByCondition(ctx context.Context, condition map[string]interface{}) ([]T, error)
	GetByCondition(ctx context.Context, condition map[string]interface{}) (*T, error)
	Update(ctx context.Context, entity *T) error
	Delete(ctx context.Context, id uint) error
}