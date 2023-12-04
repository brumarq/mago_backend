package dtos

// CreateFileSendDTO represents the data transfer object for creating a FileSend
type CreateFileSendDTO struct {
	DeviceID int    `json:"deviceId"`
	UserID   int    `json:"userId"`
	File     string `json:"file,omitempty"`
}
