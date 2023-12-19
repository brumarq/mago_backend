class ExportAggregatedLogsCsvDto:
    def __init__(self, file_name: str, aggregated_log_date_type: str, device_id: int, field_id: int):
        self.file_name = file_name
        self.aggregated_log_date_type = aggregated_log_date_type
        self.device_id = device_id
        self.field_id = field_id