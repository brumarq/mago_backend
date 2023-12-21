from app.main.domain.entities.log_value import LogValue
from app.main.domain.entities.log_collection import LogCollection
from typing import List
from app.main.application.service.abstract.base_metrics_service import BaseMetricsService
from app.main.infrastructure.repositories.repository import Repository
from flask import abort
from app.main.webapp.middleware.authentication import has_required_permission

class MetricsService(BaseMetricsService):

    def __init__(self, log_value_repository: Repository(LogValue)):
        self.log_value_repository = log_value_repository

    def get_device_metrics_by_device(self, device_id) -> List[LogValue]:   
        
        if not (has_required_permission("client") or has_required_permission("admin")):
            abort(401, "This user does not have sufficient permissions")

        if device_id <= 0:
            abort(400, "Device id cannot be 0 or negative!")

        device_metrics = self.log_value_repository.get_all_by_condition(LogValue.log_collection.has(LogCollection.device_id == device_id)) 

        return device_metrics       
