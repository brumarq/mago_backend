package interfaces

import (
	. "FirmwareGO/application/dtos"
	. "FirmwareGO/domain/entities"
	"context"
)

type IFirmwareService interface {
	DbIsConnected() bool
	CreateFileSend(ctx context.Context, fileSend *FileSend) (*FileSendResponseDTO, error)
	GetFirmwareHistoryByDeviceId(ctx context.Context, deviceId int) ([]*FileSendResponseDTO, error)
}
