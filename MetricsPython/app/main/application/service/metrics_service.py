from app.main.domain.entities.log_value import LogValue
from typing import List
from app.main.application.service.abstract.base_metrics_service import BaseMetricsService
from app.main.infrastructure.repositories.metrics_repository import MetricsRepository
from flask import abort
from app.main.webapp.middleware.authentication import has_required_permission

class MetricsService(BaseMetricsService):

    def __init__(self, metrics_repository: (MetricsRepository)):
        self.metrics_repository = metrics_repository

    def get_latest_device_metrics_by_device_id(self, device_id) -> List[LogValue]:   
        
        if not (has_required_permission("client") or has_required_permission("admin")):
            abort(401, "This user does not have sufficient permissions")

        if device_id == 0 or device_id < 0:
            abort(400, "Device id cannot be 0 or negative!")

        device_metrics = self.metrics_repository.get_latest_log_values_by_device_id(device_id)

        return device_metrics       
