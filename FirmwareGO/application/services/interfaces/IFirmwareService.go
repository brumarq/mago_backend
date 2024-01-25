package interfaces

import (
	. "FirmwareGO/application/dtos"
	"context"
)

type IFirmwareService interface {
	CreateFileSend(ctx context.Context, newFileSendDto CreateFileSendDTO) (*FileSendResponseDTO, error)
	GetFirmwareHistoryByDeviceId(ctx context.Context, deviceId int) ([]*FileSendResponseDTO, error)
}
