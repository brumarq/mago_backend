from abc import ABC, abstractmethod
from datetime import datetime

class BaseAggregatedLogsService(ABC):
    @abstractmethod
    def get_aggregated_logs(self, aggregated_log_date_type: str, device_id: int, field_id: int, page_number: int, page_size: int, start_date: str, end_date: str):
        pass
