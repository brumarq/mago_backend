from app.main.domain.entities.log_value import LogValue
from app.main.domain.entities.log_collection import LogCollection
from typing import List
from app.main.application.service.abstract.metrics_abstract_service import MetricsAbstractService
from app.main.infrastructure.repositories.repository import Repository
from flask import abort


class MetricsService(MetricsAbstractService):

    def __init__(self):
        self.log_value_repository = Repository(LogValue)

    def get_device_metrics_by_device(self, device_id) -> List[LogValue]:   

        if device_id <= 0:
            abort(400, "Device id cannot be 0 or negative!")

        #check if logcollection in logvalue has device_id specified and if so, it returns those entries
        device_metrics = self.log_value_repository.get_all_by_condition(LogValue.log_collection.has(LogCollection.device_id == device_id)) 

        return device_metrics




        
        
