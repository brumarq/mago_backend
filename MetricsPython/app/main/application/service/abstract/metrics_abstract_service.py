from typing import List
from abc import ABC, abstractmethod
from app.main.domain.entities.log_value import LogValue

class MetricsAbstractService(ABC):
    
    @abstractmethod
    def get_device_metrics_by_device(device_id: int) -> List[LogValue]:
        pass
