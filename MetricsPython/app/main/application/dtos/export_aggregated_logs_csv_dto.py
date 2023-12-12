class ExportAggregatedLogsCsvDto:
    def __init__(self, file_name: str, aggregated_log_date_type: str):
        self.file_name = file_name
        self.aggregated_log_date_type = aggregated_log_date_type