from flask_restx import Namespace, fields

class MetricsDto:
    api = Namespace('Metrics', description="Metrics related operations")

    field = api.model('Field', {
        'id': fields.Integer(required=True, description="Field identifier (int)"),
        'created_at': fields.Date(required=True, description="Field created at (datetime)"),
        'updated_at': fields.Date(required=True, description="Field updated at (datime)"),
        'name': fields.String(required=True, description="Field"),
        'unit_id': fields.Integer(required=True, description='Unit identifier (int)'), #id for now, need object later
        'device_type_id': fields.Integer(required=True, description='Device Type identifier (int)'), #id for now, need object later
        'loggable': fields.Boolean(required=True, description='Field loggable (boolean)'),
    })

    log_collection_type = api.model('LogCollectionType', {
        'id': fields.Integer(required=True, description='Log Collection Type identifier (int)'),
        'created_at': fields.Date(required=True, description="Log Collection created at (datetime)"),
        'updated_at': fields.Date(required=True, description="Log Collection updated at (datetime)")
    })

    log_collection = api.model('LogCollection', {
        'id': fields.Integer(required=True, description="Log Collection identifier (int)"),
        'created_at': fields.Date(required=True, description="Log Collection created at (datetime)"),
        'updated_at': fields.Date(required=True, description="Log Collection updated at (datetime)"),
        'device_id': fields.Integer(required=True, description="Device identifier (int)"), #id for now, need object later
        'log_collection_type': fields.Nested(log_collection_type, required=True, description="Log Collection Type object"),
    })

    log_value = api.model('LogValue', {
        'id': fields.Integer(required=True, description="Log Value identifier"),
        'created_at': fields.Date(required=True, description="LogValue created_at"),
        'updated_at': fields.Date(required=True, description="LogValue updated_at"),
        'value': fields.Float(required=True, description="Log Value"),
        'field': fields.Nested(field, required=True, description="Field object"),
        'log_collection': fields.Nested(log_collection, required=True, description="Log Collection Type"),
    })

    # The objects below are the actual DTOs.

    metrics_response_dto = api.model('MetricsResponseDto', { #log collection
        'id': fields.Integer(required=True, description="Log Value identifier"),
        'created_at': fields.Date(required=True, description="LogValue creation datetime"),
        'updated_at': fields.Date(required=True, description="LogValue updatation datetime"),
        'value': fields.Float(required=True, description="Log Value current value"),
        'field': fields.Nested(field, required=True, description="Field object"),
        'log_collection': fields.Nested(log_collection, required=True, description="Log Collection object"),
    })

    aggregated_logs_response_dto = api.model('AggregatedResponseDto', {
        'id': fields.Integer(required=True, description="AggregatedLog id"),
        'created_at': fields.Date(required=True, description="AggregatedLog created at"),
        'updated_at': fields.Date(required=True, description="AggregatedLog updated at"),
        'field': fields.Nested(field, required=True, description="Field object (object)"),
        'type': fields.String(required=True, description="AggregatedLog Type (string)"),
        'average_value': fields.Float(required=True, description="AggregatedLog Average value (float)"),
        'min_value': fields.Float(required=True, description="AggregatedLog Min value (float)"),
        'max_value': fields.Float(required=True, description="AggregatedLog Max value (float)")
    })

    export_aggregated_logs_csv_dto = api.model('ExportAggregatedLogsCsvDto', {
        'file_name': fields.String(required=True, description="File name (without extension)"),
        'aggregated_log_date_type': fields.String(required=True, description="Aggregated log date type (Weekly, Monthly, Yearly)")
    })