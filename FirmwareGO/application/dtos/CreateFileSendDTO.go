package dtos

// CreateFileSendDTO represents the data transfer object for creating a FileSend
type CreateFileSendDTO struct {
	DeviceID int    `json:"deviceId"`
	File     string `json:"file,omitempty"`
}
