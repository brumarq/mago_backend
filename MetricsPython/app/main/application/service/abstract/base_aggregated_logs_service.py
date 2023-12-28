from abc import ABC, abstractmethod

class BaseAggregatedLogsService(ABC):
    @abstractmethod
    def get_aggregated_logs(self, aggregated_log_date_type: str, device_id: int, field_id: int):
        pass
