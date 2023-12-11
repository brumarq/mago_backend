from flask_restx import Namespace, fields

class MetricsDto:
    api = Namespace('Metrics', description="Metrics related operations")

    field = api.model('Field', {
        'id': fields.Integer(required=True, description="Field identifier (int)", attribute="id"),
        'createdAt': fields.Date(required=True, description="Field created at (datetime)", attribute="created_at"),
        'updatedAt': fields.Date(required=True, description="Field updated at (datime)", attribute="updated_at"),
        'name': fields.String(required=True, description="Field", attribute="name"),
        'unitId': fields.Integer(required=True, description='Unit identifier (int)', attribute="unit_id"), #id for now, need object later
        'deviceTypeId': fields.Integer(required=True, description='Device Type identifier (int)', attribute="device_type_id"), #id for now, need object later
        'loggable': fields.Boolean(required=True, description='Field loggable (boolean)', attribute="loggable"),
    })

    log_collection_type = api.model('LogCollectionType', {
        'id': fields.Integer(required=True, description='Log Collection Type identifier (int)', attribute="id"),
        'createdAt': fields.Date(required=True, description="Log Collection created at (datetime)", attribute="created_at"),
        'updatedAt': fields.Date(required=True, description="Log Collection updated at (datetime)", attribute="updated_at")
    })

    log_collection = api.model('LogCollection', {
        'id': fields.Integer(required=True, description="Log Collection identifier (int)", attribute="id"),
        'createdAt': fields.Date(required=True, description="Log Collection created at (datetime)", attribute="created_at"),
        'updatedAt': fields.Date(required=True, description="Log Collection updated at (datetime)", attribute="updated_at"),
        'deviceId': fields.Integer(required=True, description="Device identifier (int)", attribute="device_id"), #id for now, need object later
        'logCollectionType': fields.Nested(log_collection_type, required=True, description="Log Collection Type object", attribute="log_collection_type"),
    })

    log_value = api.model('LogValue', {
        'id': fields.Integer(required=True, description="Log Value identifier", attribute="id"),
        'createdAt': fields.Date(required=True, description="LogValue created_at", attribute="created_at"),
        'updatedAt': fields.Date(required=True, description="LogValue updated_at", attribute="updated_at"),
        'value': fields.Float(required=True, description="Log Value", attribute="value"),
        'field': fields.Nested(field, required=True, description="Field object", attribute="field"),
        'logCollection': fields.Nested(log_collection, required=True, description="Log Collection Type", attribute="log_collection"),
    })

    # The objects below are the actual DTOs.

    metrics_response_dto = api.model('MetricsResponseDto', { #log collection
        'id': fields.Integer(required=True, description="Log Value identifier", attribute="id"),
        'createdAt': fields.Date(required=True, description="LogValue creation datetime", attribute="created_at"),
        'updatedAt': fields.Date(required=True, description="LogValue updatation datetime", attribute="updated_at"),
        'value': fields.Float(required=True, description="Log Value current value", attribute="value"),
        'field': fields.Nested(field, required=True, description="Field object", attribute="field"),
        'logCollection': fields.Nested(log_collection, required=True, description="Log Collection object", attribute="log_collection"),
    })

    aggregated_logs_response_dto = api.model('AggregatedResponseDto', {
        'id': fields.Integer(required=True, description="AggregatedLog id", attribute="id"),
        'createdAt': fields.Date(required=True, description="AggregatedLog created at", attribute="created_at"),
        'updatedAt': fields.Date(required=True, description="AggregatedLog updated at", attribute="updated_at"),
        'field': fields.Nested(field, required=True, description="Field object (object)", attribute="field"),
        'type': fields.String(required=True, description="AggregatedLog Type (string)", attribute="type"),
        'averageValue': fields.Float(required=True, description="AggregatedLog Average value (float)", attribute="average_value"),
        'minValue': fields.Float(required=True, description="AggregatedLog Min value (float)", attribute="min_value"),
        'maxValue': fields.Float(required=True, description="AggregatedLog Max value (float)", attribute="max_value")
    })

    export_aggregated_logs_csv_dto = api.model('ExportAggregatedLogsCsvDto', {
        'fileName': fields.String(required=True, description="File name (without extension)", attribute="file_name"),
        'aggregatedLogDateType': fields.String(required=True, description="Aggregated log date type (Weekly, Monthly, Yearly)", attribute="aggregated_log_date_type")
    })