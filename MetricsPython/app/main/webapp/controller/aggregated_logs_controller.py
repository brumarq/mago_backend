from flask_restx import Resource
from app.main.application.dtos.aggregated_logs_dto import AggregatedLogsDto
from app.main.application.service.aggregated_logs_service import AggregatedLogsService
from app.main.domain.enums.aggregated_log_date_type import AggregatedLogDateType
from app.main.application.dtos.export_aggregated_logs_csv_dto import ExportAggregatedLogsCsvDto
from app.main.infrastructure.repositories.repository import Repository
from app.main.domain.entities.field import Field
from app.main.domain.entities.weekly_average import WeeklyAverage
from app.main.domain.entities.monthly_average import MonthlyAverage
from app.main.domain.entities.yearly_average import YearlyAverage
from app.main.application.service.aggregated_logs_service import AggregatedLogsService
from app.main.webapp.middleware.authentication import requires_auth

api = AggregatedLogsDto.api

def initialize_aggregated_logs_service():
    return AggregatedLogsService(
        Repository(Field),
        Repository(WeeklyAverage),
        Repository(MonthlyAverage),
        Repository(YearlyAverage)
    )

aggregated_logs_service = initialize_aggregated_logs_service()

@api.route('/<string:aggregated_log_date_type>/<int:device_id>/<int:field_id>')
@api.doc(params={'aggregated_log_date_type': 'Aggregation log date type'})
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
        }
    })
    @api.marshal_list_with(AggregatedLogsDto.aggregated_logs_response_dto)
    @requires_auth
    def get(self, aggregated_log_date_type: str, device_id: int, field_id: int):
        """Provides list of aggregated logs based on date type"""
        return self.aggregated_logs_service.get_aggregated_logs(aggregated_log_date_type, device_id, field_id)

# @api.expect(AggregatedLogsDto.export_aggregated_logs_csv_dto, validate=True)
# @api.route('/export-csv')
# @api.response(201, 'CSV file successfully exported')
# @api.response(400, 'Invalid body provided')
# class ExportAggregatedLogsCsv(Resource):
#     def __init__(self, *args, **kwargs):
#         super().__init__(*args, **kwargs)
#         self.aggregated_logs_service = aggregated_logs_service
#     @api.doc('export_aggregated_logs_csv', description="Export aggregated logs to CSV file")
#     def post(self):
#         """Export aggregated logs to CSV file"""

#         export_csv_dto = ExportAggregatedLogsCsvDto(
#             file_name=api.payload["fileName"],
#             aggregated_log_date_type=api.payload["aggregatedLogDateType"],
#             device_id=api.payload["deviceId"],
#             field_id=api.payload["fieldId"]      
#         )     
#         return self.aggregated_logs_service.export_aggregated_logs_csv(export_csv_dto)