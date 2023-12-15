from flask_restx import Resource
from app.main.application.dtos.metrics_dto import MetricsDto
from app.main.application.service.metrics_service import MetricsService

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