from flask_restx import Namespace, fields

class MetricsDto:
    api = Namespace('metrics', description='Metrics related operations')

    field = api.model('Field', {
        'id': fields.Integer(required=True, description='Log Value id'),
        'created_at': fields.Date(required=True, description="Log Collection created at"),
        'updated_at': fields.Date(required=True, description="Log Collection updated at"),
        'name': fields.String(required=True, description='Log Value'),
        'unit_id': fields.Integer(required=True, description='Log Value id'), #id for not, need object later
        'device_type_id': fields.Integer(required=True, description='Log Value id'), #id for not, need object later
        'loggable': fields.Boolean(required=True, description='Log Value id'),
    })

    log_collection_type = api.model('LogCollectionType', {
        'id': fields.Integer(required=True, description='Log Collection Type id'),
        'created_at': fields.Date(required=True, description="Log Collection created at"),
        'updated_at': fields.Date(required=True, description="Log Collection updated at")
    })

    log_collection = api.model('LogCollection', {
        'id': fields.Integer(required=True, description='Log Value id'),
        'created_at': fields.Date(required=True, description="Log Collection created at"),
        'updated_at': fields.Date(required=True, description="Log Collection updated at"),
        'device_id': fields.Integer(required=True, description='Log Value id'),
        'log_collection_type': fields.Nested(log_collection_type, required=True, description='Log Collection Type'),
    })

    log_value = api.model('LogValue', {
        'id': fields.Integer(required=True, description='Log Value id'),
        'created_at': fields.Date(required=True, description="LogValue created_at"),
        'updated_at': fields.Date(required=True, description="LogValue updated_at"),
        'value': fields.Float(required=True, description='Log Value'),
        'field': fields.Nested(field, required=True, description='Log Collection Type'),
        'log_collection': fields.Nested(log_collection, required=True, description='Log Collection Type'),
    })


    metrics_response_dto = api.model('MetricsResponseDto', { #log collection
        'id': fields.Integer(required=True, description='Log Value id'),
        'created_at': fields.Date(required=True, description="LogValue created_at"),
        'updated_at': fields.Date(required=True, description="LogValue updated_at"),
        'value': fields.Float(required=True, description='Log Value'),
        'field': fields.Nested(field, required=True, description='Log Collection Type'),
        'log_collection': fields.Nested(log_collection, required=True, description='Log Collection Type'),
    })

    aggregated_logs_response_dto = api.model('AggregatedResponseDto', {
        'id': fields.Integer(required=True, description='Aggregated log id'),
        'created_at': fields.Date(required=True, description="Log Collection created at"),
        'updated_at': fields.Date(required=True, description="Log Collection updated at"),
        'field': fields.Nested(field, required=True, description='Log Collection Type'),
        'type': fields.String(required=True, description='Type'),
        'average_value': fields.Float(required=True, description='Average value'),
        'min_value': fields.Float(required=True, description='Min value'),
        'max_value': fields.Float(required=True, description='Max value')
    })