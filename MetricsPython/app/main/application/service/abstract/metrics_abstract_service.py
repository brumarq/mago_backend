from typing import List
from abc import ABC, abstractmethod
#from app.main.domain.entities.aggregated_log import AggregatedLog
from app.main.domain.entities.log_value import LogValue
from app.main.application.dtos.export_aggregated_logs_csv_dto import ExportAggregatedLogsCsvDto

class MetricsAbstractService(ABC):
    # @abstractmethod
    # def get_aggregated_logs(aggregated_log_date_type: str) -> List[AggregatedLog]:
    #     pass
    
    @abstractmethod
    def get_device_metrics_by_device(device_id: int) -> List[LogValue]:
        pass

    # @abstractmethod
    # def export_aggregated_logs_csv(export_aggregated_logs_csv_dto: ExportAggregatedLogsCsvDto) -> None:
    #     pass
