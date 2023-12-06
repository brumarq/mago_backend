from typing import List
from abc import ABC, abstractmethod
from app.main.domain.entities.aggregated_log import AggregatedLog
from app.main.domain.entities.log_value import LogValue

class IMetricsService(ABC):
    @abstractmethod
    def get_aggregated_logs(aggregated_log_date_type) -> List[AggregatedLog]:
        pass
    
    @abstractmethod
    def get_device_metrics_by_device(device_id) -> List[LogValue]:
        pass
