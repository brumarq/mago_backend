from flask_restx import Namespace, fields

class MetricsNamespace:
    api = Namespace('Metrics', description="Metrics related operations")

    # The objects below are the actual DTOs.

    field_dto = api.model('Field', {
        'id': fields.Integer(required=True, description="Field identifier (int)", attribute="id"),
        'createdAt': fields.DateTime(required=True, description="Field created at (datetime)", attribute="created_at"),
        'updatedAt': fields.DateTime(required=True, description="Field updated at (datime)", attribute="updated_at"),
        'name': fields.String(required=True, description="Field", attribute="name"),
        'unitId': fields.Integer(required=True, description='Unit identifier (int)', attribute="unit_id"),
        'deviceTypeId': fields.Integer(required=True, description='Device Type identifier (int)', attribute="device_type_id"),
        'loggable': fields.Boolean(required=True, description='Field loggable (boolean)', attribute="loggable"),
    })

    log_collection_type_dto = api.model('LogCollectionType', {
        'id': fields.Integer(required=True, description='Log Collection Type identifier (int)', attribute="id"),
        'createdAt': fields.DateTime(required=True, description="Log Collection Type created at (datetime)", attribute="created_at"),
        'updatedAt': fields.DateTime(required=True, description="Log Collection Type updated at (datetime)", attribute="updated_at")
    })

    log_collection_dto = api.model('LogCollection', {
        'id': fields.Integer(required=True, description="Log Collection identifier (int)", attribute="id"),
        'createdAt': fields.DateTime(required=True, description="Log Collection created at (datetime)", attribute="created_at"),
        'updatedAt': fields.DateTime(required=True, description="Log Collection updated at (datetime)", attribute="updated_at"),
        'deviceId': fields.Integer(required=True, description="Device identifier (int)", attribute="device_id"),
        'logCollectionType': fields.Nested(log_collection_type_dto, required=True, description="Log Collection Type object", attribute="log_collection_type"),
    })

    metrics_response_dto = api.model('MetricsResponseDto', {
        'id': fields.Integer(required=True, description="Log Value identifier", attribute="id"),
        'createdAt': fields.DateTime(required=True, description="LogValue creation datetime", attribute="created_at"),
        'updatedAt': fields.DateTime(required=True, description="LogValue updatation datetime", attribute="updated_at"),
        'value': fields.Float(required=True, description="Log Value current value", attribute="value"),
        'field': fields.Nested(field_dto, required=True, description="Field object", attribute="field"),
        'logCollection': fields.Nested(log_collection_dto, required=True, description="Log Collection object", attribute="log_collection"),
    })