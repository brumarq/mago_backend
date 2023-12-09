from flask_restx import Resource
from app.main.application.dtos.metrics_dto import MetricsDto
from app.main.application.service.metrics_service import MetricsService
from app.main.domain.enums.aggregated_log_date_type import AggregatedLogDateType
from app.main.application.dtos.export_aggregated_logs_csv_dto import ExportAggregatedLogsCsvDto
from typing import Dict, Tuple
from flask import request

api = MetricsDto.api

metrics_service = MetricsService()

@api.route('/devices/<int:device_id>')
@api.doc(params={'device_id': 'The device identifier'})
@api.response(404, 'Device id not found')
class MetricsList(Resource):
    @api.doc('list_of_device_metrics_by_device_id')
    @api.marshal_list_with(MetricsDto.metrics_response_dto)
    def get(self, device_id: int):
        """Provides devices metrics for a specific device"""
        return metrics_service.get_device_metrics_by_device(device_id)


@api.route('/aggregated-logs/<string:aggregated_log_date_type>')
@api.doc(params={'aggregated_log_date_type': 'Aggregation log date type'})
class AggregatedLogList(Resource):
    @api.doc('list_of_aggregated_logs_by_aggregation_log_date_type', params={
        'aggregated_log_date_type': {
            'description': 'Aggregation log date type',
            'enum': [e.value for e in AggregatedLogDateType],  # Using enum values dynamically
            'default': AggregatedLogDateType.WEEKLY.value  # Default value to be selected
        }
    })
    @api.marshal_list_with(MetricsDto.aggregated_logs_response_dto)
    def get(self, aggregated_log_date_type: str):
        """Provides list of aggregated logs based on date type"""
        return metrics_service.get_aggregated_logs(aggregated_log_date_type)

@api.expect(MetricsDto.export_aggregated_logs_csv_dto, validate=True)
@api.route('/aggregated-logs/export-csv')
class ExportAggregatedLogsCsv(Resource):
    @api.response(201, 'CSV file successfully exported')
    def post(self):
        """Export aggregated logs to CSV file"""

        export_csv_dto = ExportAggregatedLogsCsvDto(
            file_name=api.payload["file_name"],
            aggregated_log_date_type=api.payload["aggregated_log_date_type"]
        )     
        return metrics_service.export_aggregated_logs_csv(export_csv_dto)