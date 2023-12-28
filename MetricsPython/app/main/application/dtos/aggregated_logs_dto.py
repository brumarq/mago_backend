from flask_restx import Namespace, fields

class AggregatedLogsDto:
    api = Namespace('AggregationLogs', description="Aggregation logs related operations")

    field = api.model('Field', {
        'id': fields.Integer(required=True, description="Field identifier (int)", attribute="id"),
        'createdAt': fields.DateTime(required=True, description="Field created at (datetime)", attribute="created_at"),
        'updatedAt': fields.DateTime(required=True, description="Field updated at (datime)", attribute="updated_at"),
        'name': fields.String(required=True, description="Field", attribute="name"),
        'unitId': fields.Integer(required=True, description='Unit identifier (int)', attribute="unit_id"), #id for now, need object later
        'deviceTypeId': fields.Integer(required=True, description='Device Type identifier (int)', attribute="device_type_id"), #id for now, need object later
        'loggable': fields.Boolean(required=True, description='Field loggable (boolean)', attribute="loggable"),
    })

    aggregated_logs_response_dto = api.model('AggregatedResponseDto', {
        'id': fields.Integer(required=True, description="AggregatedLog id", attribute="id"),
        'createdAt': fields.DateTime(required=True, description="AggregatedLog created at", attribute="created_at"),
        'updatedAt': fields.DateTime(required=True, description="AggregatedLog updated at", attribute="updated_at"),
        'averageValue': fields.Float(required=True, description="AggregatedLog Average value (float)", attribute="average_value"),
        'minValue': fields.Float(required=True, description="AggregatedLog Min value (float)", attribute="min_value"),
        'maxValue': fields.Float(required=True, description="AggregatedLog Max value (float)", attribute="max_value"),
        'deviceId': fields.Integer(required=True, description="Device ifentifier", attribute="device_id"),
        'field': fields.Nested(field, required=True, description="Field object (object)", attribute="field"),
    })