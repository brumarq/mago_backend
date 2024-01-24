from flask_restx import Resource
from app.main.application.namespaces.metrics_namespace import MetricsNamespace
from app.main.application.service.metrics_service import MetricsService
from app.main.infrastructure.repositories.metrics_repository import MetricsRepository
from app.main.domain.entities.log_value import LogValue
from app.main.webapp.middleware.authentication import requires_auth
from flask import request

api = MetricsNamespace.api

def initialize_metrics_service():
    return MetricsService(MetricsRepository(LogValue))

metrics_service = initialize_metrics_service()

@api.route('/devices/<int(signed=True):device_id>')
@api.doc(params={'device_id': 'The device identifier'})
@api.response(404, 'Device id not found')
@api.response(200, 'Successfully retrieved device metrics')
@api.response(400, 'Invalid device id provided')
class MetricsList(Resource):
    def __init__(self, *args, **kwargs):
        super().__init__(*args, **kwargs)
        self.metrics_service = metrics_service

    @api.header("Content-Type", "application/json")
    @api.doc('get_device_metrics_by_device', description="Get device metrics for a specific device")
    @api.doc('list_of_aggregated_logs_by_aggregation_log_date_type', params={
        'device_id': 'The unique identifier of the device',
        'page_number': 'The page number (starts at 1) | Defaults to 1',
        'page_size': 'The page size | Defaults to 50',
    })
    @api.marshal_list_with(MetricsNamespace.metrics_response_dto)
    @requires_auth
    def get(self, device_id: int):
        """Provides devices metrics for a specific device"""
        page_number = request.args.get('page_number', default=1, type=int)
        page_size = request.args.get('page_size', default=50, type=int)
        return self.metrics_service.get_latest_device_metrics_by_device_id(device_id, page_number, page_size)