package mappers

import (
	. "FirmwareGO/application/dtos"
	. "FirmwareGO/domain/entities"
)

func ToBaseDTO(baseEntity BaseEntity) BaseDTO {
	return BaseDTO{
		ID:        baseEntity.ID,
		CreateAt:  baseEntity.CreateAt,
		UpdatedAt: baseEntity.UpdatedAt,
	}
}

func ToBaseEntity(baseDto BaseDTO) BaseEntity {
	return BaseEntity{
		ID:        baseDto.ID,
		CreateAt:  baseDto.CreateAt,
		UpdatedAt: baseDto.UpdatedAt,
	}
}
