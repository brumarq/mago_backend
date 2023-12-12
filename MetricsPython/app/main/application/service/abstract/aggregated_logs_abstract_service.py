from typing import List
from abc import ABC, abstractmethod
from app.main.application.dtos.export_aggregated_logs_csv_dto import ExportAggregatedLogsCsvDto

class AggregatedLogsAbstractService(ABC):
    @abstractmethod
    def get_aggregated_logs(device_id: int, field_id: int, aggregated_log_date_type: str):
        pass

    @abstractmethod
    def export_aggregated_logs_csv(export_aggregated_logs_csv_dto: ExportAggregatedLogsCsvDto):
        pass
