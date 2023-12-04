package repositories

import (
	. "FirmwareGO/domain/entities/interfaces"
	"context"
	"gorm.io/gorm"
)

// Repository is a generic GORM-based repository
type Repository[T IEntity] struct {
	DB *gorm.DB
}

// NewRepository creates a new instance of Repository
func NewRepository[T IEntity](db *gorm.DB) *Repository[T] {
	return &Repository[T]{DB: db}
}

// Create adds a new entity to the database
func (r *Repository[T]) Create(ctx context.Context, entity *T) error {
	return r.DB.WithContext(ctx).Create(entity).Error
}

// GetAll retrieves all entities from the database
func (r *Repository[T]) GetAll(ctx context.Context) ([]T, error) {
	var entities []T
	err := r.DB.WithContext(ctx).Find(&entities).Error
	return entities, err
}

// GetCollectionByCondition retrieves a collection of entities based on a condition
func (r *Repository[T]) GetCollectionByCondition(ctx context.Context, condition map[string]interface{}) ([]T, error) {
	var entities []T
	err := r.DB.WithContext(ctx).Where(condition).Find(&entities).Error
	return entities, err
}

// GetByCondition retrieves an entity based on a condition
func (r *Repository[T]) GetByCondition(ctx context.Context, condition map[string]interface{}) (*T, error) {
	var entity T
	err := r.DB.WithContext(ctx).Where(condition).First(&entity).Error
	return &entity, err
}

// Update updates an entity in the database
func (r *Repository[T]) Update(ctx context.Context, entity *T) error {
	return r.DB.WithContext(ctx).Save(entity).Error
}

// Delete removes an entity from the database
func (r *Repository[T]) Delete(ctx context.Context, id uint) error {
	var entity T
	return r.DB.WithContext(ctx).Delete(&entity, id).Error
}
