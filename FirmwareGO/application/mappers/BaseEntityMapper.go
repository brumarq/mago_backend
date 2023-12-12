package mappers

import (
	. "FirmwareGO/application/dtos"
	. "FirmwareGO/domain/entities"
)

func ToBaseDTO(baseEntity BaseEntity) BaseDTO {
	return BaseDTO{
		ID:        baseEntity.ID,
		CreatedAt: baseEntity.CreatedAt,
		UpdatedAt: baseEntity.UpdatedAt,
	}
}

func ToBaseEntity(baseDto BaseDTO) BaseEntity {
	return BaseEntity{
		ID:        baseDto.ID,
		CreatedAt: baseDto.CreatedAt,
		UpdatedAt: baseDto.UpdatedAt,
	}
}
