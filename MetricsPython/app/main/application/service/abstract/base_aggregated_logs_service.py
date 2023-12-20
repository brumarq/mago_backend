from abc import ABC, abstractmethod
from app.main.application.dtos.export_aggregated_logs_csv_dto import ExportAggregatedLogsCsvDto

class BaseAggregatedLogsService(ABC):
    @abstractmethod
    def get_aggregated_logs(self, aggregated_log_date_type: str, device_id: int, field_id: int):
        pass

    @abstractmethod
    def export_aggregated_logs_csv(self, export_aggregated_logs_csv_dto: ExportAggregatedLogsCsvDto):
        pass
