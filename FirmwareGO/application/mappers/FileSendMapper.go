package mappers

import (
	. "FirmwareGO/application/dtos"
	. "FirmwareGO/domain/entities"
)

// ToFileSendResponseDTO maps a FileSend entity to a FileSendResponseDTO
func ToFileSendResponseDTO(fileSend *FileSend) FileSendResponseDTO {
	return FileSendResponseDTO{
		BaseDTO:      ToBaseDTO(fileSend.BaseEntity),
		UpdateStatus: fileSend.UpdateStatus,
		DeviceID:     fileSend.DeviceId,
		UserID:       fileSend.UserId,
		File:         fileSend.File,
		CurrPart:     fileSend.CurrPart,
		TotParts:     fileSend.TotParts,
	}
}

// ToFileSendResponseDTOList maps a collection of FileSend entities to an array of FileSendResponseDTO using ToFileSendResponseDTO()
func ToFileSendResponseDTOList(fileSends []*FileSend) []*FileSendResponseDTO {
	var responseDTOs []*FileSendResponseDTO

	for _, fileSend := range fileSends {
		responseDTO := ToFileSendResponseDTO(fileSend)
		responseDTOs = append(responseDTOs, &responseDTO)
	}

	return responseDTOs
}

// ToFileSend maps a FileSendResponseDTO to a FileSend Entity
func ToFileSend(createFileSendDto CreateFileSendDTO) *FileSend {
	return &FileSend{
		DeviceId: createFileSendDto.DeviceID,
		UserId:   createFileSendDto.UserID,
		File:     &createFileSendDto.File,
	}
}
