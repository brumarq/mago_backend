package dtos

type FileSendResponseDTO struct {
	BaseDTO              // Embedded
	UpdateStatus *string `json:"updateStatus,omitempty"`
	DeviceID     int     `json:"deviceId"`
	File         *string `json:"file,omitempty"`
	CurrPart     int     `json:"currPart"`
	TotParts     int     `json:"totParts"`
}
