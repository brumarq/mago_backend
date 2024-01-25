package services

import (
	. "FirmwareGO/application/dtos"
	"FirmwareGO/application/mappers"
	. "FirmwareGO/domain/entities"
	. "FirmwareGO/infrastructure/repositories/interfaces"
	"errors"
	"github.com/gin-gonic/gin"
)

type FirmwareService struct {
	Repo IRepository[*FileSend]
}

func NewFirmwareService(repo IRepository[*FileSend]) *FirmwareService {
	return &FirmwareService{Repo: repo}
}

func (service *FirmwareService) CreateFileSend(ctx *gin.Context, newFileSendDto CreateFileSendDTO) (*FileSendResponseDTO, error) {
	// Validate DTO before proceeding
	if err := validateFileSendDTO(newFileSendDto); err != nil {
		return nil, err
	}

	// Map DTO to entity
	newFileSend := mappers.ToFileSend(newFileSendDto)

	// Create entity in database
	err := service.Repo.Create(ctx, &newFileSend)
	if err != nil {
		return nil, err //return the error
	}

	// Map entity back to DTO
	responseDto := mappers.ToFileSendResponseDTO(newFileSend)
	return &responseDto, nil
}

func (service *FirmwareService) GetFirmwareHistoryByDeviceId(ctx *gin.Context, deviceId int) ([]*FileSendResponseDTO, error) {

	condition := map[string]interface{}{"device_id": deviceId}

	fileSends, err := service.Repo.GetCollectionByCondition(ctx, condition, "created_at ASC")
	if err != nil {
		return nil, err
	}

	return mappers.ToFileSendResponseDTOList(fileSends), nil
}

func validateFileSendDTO(newFileSendDTO CreateFileSendDTO) error {
	if newFileSendDTO == (CreateFileSendDTO{}) {
		return errors.New("file DTO cannot be null")
	}

	if newFileSendDTO.File == "" {
		return errors.New("file cannot be null or empty")
	}

	if newFileSendDTO.DeviceID <= 0 {
		return errors.New("device id cannot be negative or 0")
	}

	return nil
}
