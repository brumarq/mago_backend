from flask_restx import Resource, reqparse
from app.main.application.namespaces.aggregated_logs_namespace import AggregatedLogsNamespace
from app.main.application.service.aggregated_logs_service import AggregatedLogsService
from app.main.domain.enums.aggregated_log_date_type import AggregatedLogDateType
from app.main.infrastructure.repositories.repository import Repository
from app.main.domain.entities.field import Field
from app.main.domain.entities.weekly_average import WeeklyAverage
from app.main.domain.entities.monthly_average import MonthlyAverage
from app.main.domain.entities.yearly_average import YearlyAverage
from app.main.application.service.aggregated_logs_service import AggregatedLogsService
from app.main.webapp.middleware.authentication import requires_auth

api = AggregatedLogsNamespace.api

def initialize_aggregated_logs_service():
    return AggregatedLogsService(
        Repository(Field),
        Repository(WeeklyAverage),
        Repository(MonthlyAverage),
        Repository(YearlyAverage)
    )

aggregated_logs_service = initialize_aggregated_logs_service()
parser = reqparse.RequestParser()
parser.add_argument('start_date', type=str, help='Start date for filtering (optional) | Format: YYYY-MM-DD')
parser.add_argument('end_date', type=str, help='End date for filtering (optional) | Format: YYYY-MM-DD')

@api.route('/<string:aggregated_log_date_type>/<int:device_id>/<int:field_id>')
@api.doc(params={'aggregated_log_date_type': 'Aggregation log date type',
                 'start_date': 'Start date for filtering (optional)',
                 'end_date': 'End date for filtering (optional)'})
@api.response(200, 'Sucessfully retrieved aggregated logs')
@api.response(400, 'Invalid property provided')
class AggregatedLogList(Resource):
    def __init__(self, *args, **kwargs):
        super().__init__(*args, **kwargs)
        self.aggregated_logs_service = aggregated_logs_service
    @api.header("Content-Type", "application/json")
    @api.doc('get_aggregated_logs', description="Get aggregated logs based on a date type")
    @api.doc('list_of_aggregated_logs_by_aggregation_log_date_type', params={
        'aggregated_log_date_type': {
            'description': 'Aggregation log date type',
            'enum': [e.value for e in AggregatedLogDateType],  # Using enum values dynamically
            'default': AggregatedLogDateType.WEEKLY.value  # Default value to be selected
        },
        'device_id': 'The unique identifier of the device',
        'field_id': 'The unique identifier of the field',
        'start_date': 'Start date for filtering (optional) | Format: YYYY-MM-DD',
        'end_date': 'End date for filtering (optional) | Format: YYYY-MM-DD'
    })
    @api.marshal_list_with(AggregatedLogsNamespace.aggregated_logs_response_dto)
    @requires_auth
    def get(self, aggregated_log_date_type: str, device_id: int, field_id: int):
        """Provides list of aggregated logs based on date type"""
        args = parser.parse_args()
        start_date = args.get('start_date')
        end_date = args.get('end_date')
        return self.aggregated_logs_service.get_aggregated_logs(aggregated_log_date_type, device_id, field_id, start_date, end_date)