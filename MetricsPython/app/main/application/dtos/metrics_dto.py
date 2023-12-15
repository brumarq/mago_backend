from flask_restx import Namespace, fields

class MetricsDto:
    api = Namespace('Metrics', description="Metrics related operations")

    field = api.model('Field', {
        'id': fields.Integer(required=True, description="Field identifier (int)", attribute="id"),
        'createdAt': fields.DateTime(required=True, description="Field created at (datetime)", attribute="created_at"),
        'updatedAt': fields.DateTime(required=True, description="Field updated at (datime)", attribute="updated_at"),
        'name': fields.String(required=True, description="Field", attribute="name"),
        'unitId': fields.Integer(required=True, description='Unit identifier (int)', attribute="unit_id"), #id for now, need object later
        'deviceTypeId': fields.Integer(required=True, description='Device Type identifier (int)', attribute="device_type_id"), #id for now, need object later
        'loggable': fields.Boolean(required=True, description='Field loggable (boolean)', attribute="loggable"),
    })

    log_collection_type = api.model('LogCollectionType', {
        'id': fields.Integer(required=True, description='Log Collection Type identifier (int)', attribute="id"),
        'createdAt': fields.DateTime(required=True, description="Log Collection created at (datetime)", attribute="created_at"),
        'updatedAt': fields.DateTime(required=True, description="Log Collection updated at (datetime)", attribute="updated_at")
    })

    log_collection = api.model('LogCollection', {
        'id': fields.Integer(required=True, description="Log Collection identifier (int)", attribute="id"),
        'createdAt': fields.DateTime(required=True, description="Log Collection created at (datetime)", attribute="created_at"),
        'updatedAt': fields.DateTime(required=True, description="Log Collection updated at (datetime)", attribute="updated_at"),
        'deviceId': fields.Integer(required=True, description="Device identifier (int)", attribute="device_id"), #id for now, need object later
        'logCollectionType': fields.Nested(log_collection_type, required=True, description="Log Collection Type object", attribute="log_collection_type"),
    })

    # The objects below are the actual DTOs.

    metrics_response_dto = api.model('MetricsResponseDto', { #log collection
        'id': fields.Integer(required=True, description="Log Value identifier", attribute="id"),
        'createdAt': fields.DateTime(required=True, description="LogValue creation datetime", attribute="created_at"),
        'updatedAt': fields.DateTime(required=True, description="LogValue updatation datetime", attribute="updated_at"),
        'value': fields.Float(required=True, description="Log Value current value", attribute="value"),
        'field': fields.Nested(field, required=True, description="Field object", attribute="field"),
        'logCollection': fields.Nested(log_collection, required=True, description="Log Collection object", attribute="log_collection"),
    })