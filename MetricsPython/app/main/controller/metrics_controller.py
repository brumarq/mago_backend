from flask_restx import Resource
from app.main.application.dtos.metrics_dto import MetricsDto
from app.main.application.service.metrics_service import MetricsService
from app.main.domain.enums.aggregated_log_date_type import AggregatedLogDateType
from app.main.application.dtos.export_aggregated_logs_csv_dto import ExportAggregatedLogsCsvDto

api = MetricsDto.api

metrics_service = MetricsService()

@api.route('/devices/<int:device_id>')
@api.doc(params={'device_id': 'The device identifier'})
@api.response(404, 'Device id not found')
@api.response(200, 'Sucessfully retrieved device metrics')
@api.response(400, 'Invalid device id provided')
class MetricsList(Resource):
    @api.header("Content-Type", "application/json")
    @api.doc('get_device_metrics_by_device', description="Get device metrics for a specific device")
    @api.marshal_list_with(MetricsDto.metrics_response_dto)
    def get(self, device_id: int):
        """Provides devices metrics for a specific device"""
        return metrics_service.get_device_metrics_by_device(device_id)


@api.route('/aggregated-logs/<int:device_id>/<int:field_id>/<string:aggregated_log_date_type>')
@api.doc(params={'aggregated_log_date_type': 'Aggregation log date type'})
@api.response(200, 'Sucessfully retrieved aggregated logs')
@api.response(400, 'Invalid property provided')
class AggregatedLogList(Resource):
    @api.header("Content-Type", "application/json")
    @api.doc('get_aggregated_logs', description="Get aggregated logs based on a date type")
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

# @api.expect(MetricsDto.export_aggregated_logs_csv_dto, validate=True)
# @api.route('/aggregated-logs/export-csv')
# @api.response(201, 'CSV file successfully exported')
# @api.response(400, 'Invalid body provided')
# class ExportAggregatedLogsCsv(Resource):
#     @api.doc('export_aggregated_logs_csv', description="Export aggregated logs to CSV file")
#     def post(self):
#         """Export aggregated logs to CSV file"""

#         export_csv_dto = ExportAggregatedLogsCsvDto(
#             file_name=api.payload["file_name"],
#             aggregated_log_date_type=api.payload["aggregated_log_date_type"]
#         )     
#         return metrics_service.export_aggregated_logs_csv(export_csv_dto)