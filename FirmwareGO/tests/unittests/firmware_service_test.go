package services

import (
	. "FirmwareGO/application/dtos"
	. "FirmwareGO/application/services"
	. "FirmwareGO/domain/entities"
	. "FirmwareGO/tests/mocks"
	"context"
	"errors"
	"github.com/golang/mock/gomock"
	"github.com/stretchr/testify/assert"
	"testing"
	"time"
)

func TestFirmwareService_CreateFileSend(t *testing.T) {
	ctrl := gomock.NewController(t)
	defer ctrl.Finish()

	mockRepo := NewMockIRepository(ctrl)
	service := NewFirmwareService(mockRepo)

	// Define test cases
	testCases := []struct {
		name    string
		dto     CreateFileSendDTO
		setup   func()
		wantErr bool
	}{
		{
			name:    "Success",
			dto:     CreateFileSendDTO{DeviceID: 1, File: "Test File"},
			setup:   func() { mockRepo.EXPECT().Create(gomock.Any(), gomock.Any()).Return(nil) },
			wantErr: false,
		},
		{
			name:    "Request body is null",
			dto:     CreateFileSendDTO{ /* invalid data */ },
			setup:   func() {},
			wantErr: true,
		},
		{
			name:    "File name is empty string",
			dto:     CreateFileSendDTO{DeviceID: 1, File: ""},
			setup:   func() {},
			wantErr: true,
		},
		{
			name:    "DeviceId is negative",
			dto:     CreateFileSendDTO{DeviceID: -1, File: "Test File"},
			setup:   func() {},
			wantErr: true,
		},
		{
			name:    "DeviceId is 0",
			dto:     CreateFileSendDTO{DeviceID: 0, File: "Test File"},
			setup:   func() {},
			wantErr: true,
		},
		{
			name: "Repository Create Failure",
			dto:  CreateFileSendDTO{DeviceID: 1, File: "Test File"},
			setup: func() {
				mockRepo.EXPECT().Create(gomock.Any(), gomock.Any()).Return(errors.New("mock create error"))
			},
			wantErr: true,
		},
	}

	// Run test cases
	for _, tc := range testCases {
		t.Run(tc.name, func(t *testing.T) {
			tc.setup()

			_, err := service.CreateFileSend(context.Background(), tc.dto)

			if tc.wantErr {
				assert.Error(t, err)
			} else {
				assert.NoError(t, err)
			}
		})
	}
}

func TestFirmwareService_GetFirmwareHistoryByDeviceId(t *testing.T) {
	ctrl := gomock.NewController(t)
	defer ctrl.Finish()

	mockRepo := NewMockIRepository(ctrl)
	service := NewFirmwareService(mockRepo)

	// Define test cases
	testCases := []struct {
		name       string
		deviceId   int
		mockSetup  func()
		wantErr    bool
		wantLength int // To check the length of the returned slice
	}{
		{
			name:     "Success with non-empty result",
			deviceId: 1,
			mockSetup: func() {
				// Mock the expected repository call with a non-empty result
				var mockedFileSends = []*FileSend{
					{
						BaseEntity: BaseEntity{
							ID:        1,
							CreatedAt: time.Now(),
							UpdatedAt: time.Now(),
						},
						UpdateStatus: func(s string) *string { return &s }("Status1"),
						DeviceId:     1,
						File:         func(s string) *string { return &s }("File1"),
						CurrPart:     1,
						TotParts:     3,
					},
					{
						BaseEntity: BaseEntity{
							ID:        2,
							CreatedAt: time.Now(),
							UpdatedAt: time.Now(),
						},
						UpdateStatus: func(s string) *string { return &s }("Status2"),
						DeviceId:     1,
						File:         func(s string) *string { return &s }("File2"),
						CurrPart:     1,
						TotParts:     3,
					},
				}
				mockRepo.EXPECT().GetCollectionByCondition(gomock.Any(), gomock.Any(), gomock.Any()).Return(mockedFileSends, nil)
			},
			wantErr:    false,
			wantLength: 2,
		},
		{
			name:     "Success with empty result",
			deviceId: 2,
			mockSetup: func() {
				mockRepo.EXPECT().GetCollectionByCondition(gomock.Any(), gomock.Any(), gomock.Any()).Return([]*FileSend{}, nil)
			},
			wantErr:    false,
			wantLength: 0,
		},
		{
			name:     "Repository returns error",
			deviceId: 1,
			mockSetup: func() {
				// Mock the repository call to return an error
				mockRepo.EXPECT().GetCollectionByCondition(gomock.Any(), gomock.Any(), gomock.Any()).Return(nil, errors.New("mock error"))
			},
			wantErr: true,
		},
	}

	// Run test cases
	for _, tc := range testCases {
		t.Run(tc.name, func(t *testing.T) {
			tc.mockSetup()

			result, err := service.GetFirmwareHistoryByDeviceId(context.Background(), tc.deviceId)

			if tc.wantErr {
				assert.Error(t, err)
			} else {
				assert.NoError(t, err)
				assert.Len(t, result, tc.wantLength)
			}
		})
	}
}
