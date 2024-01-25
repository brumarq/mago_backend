package interfaces

import (
	. "FirmwareGO/domain/entities/interfaces"
	"github.com/gin-gonic/gin"
)

// IRepository is a generic interface for data operations
type IRepository[T IEntity] interface {
	IsDatabaseConnected() bool
	Create(ctx *gin.Context, entity *T) error
	GetAll(ctx *gin.Context) ([]T, error)
	GetCollectionByCondition(ctx *gin.Context, condition map[string]interface{}, orderBy ...string) ([]T, error)
	GetByCondition(ctx *gin.Context, condition map[string]interface{}) (*T, error)
	Update(ctx *gin.Context, entity *T) error
	Delete(ctx *gin.Context, id uint) error
}
