package services

import (
	. "FirmwareGO/domain/entities"
	. "FirmwareGO/infrastructure/repositories/interfaces"
)

type ApplicationStateService struct {
	Repo IRepository[*BaseEntity]
}

func NewApplicationStateService(repo IRepository[*BaseEntity]) *ApplicationStateService {
	return &ApplicationStateService{Repo: repo}
}

func (s *ApplicationStateService) DbIsConnected() bool {
	return s.Repo.IsDatabaseConnected()
}
