from flask_restx import Resource, reqparse
from app.main.application.namespaces.metrics_namespace import MetricsNamespace
from app.main.application.service.metrics_service import MetricsService
from app.main.infrastructure.repositories.metrics_repository import MetricsRepository
from app.main.domain.entities.log_value import LogValue
from app.main.webapp.middleware.authentication import requires_auth

api = MetricsNamespace.api

def initialize_metrics_service():
    return MetricsService(MetricsRepository(LogValue))

metrics_service = initialize_metrics_service()

parser = reqparse.RequestParser()
parser.add_argument('page_number', type=int, help='The page number (starts at 1) | Defaults to 1')
parser.add_argument('page_size', type=int, help='The page size | Defaults to 50')

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
        args = parser.parse_args()
        page_number = args.get('page_number')
        page_size = args.get('page_size')
        return self.metrics_service.get_latest_device_metrics_by_device_id(device_id, page_number, page_size)