from flask_restx import Namespace, fields

class FieldNamespace:
    api = Namespace('Fields', description="Field related operations")

    # The objects below are the actual DTOs.

    field_request_dto = api.model('CreateField', {        
        'name': fields.String(required=True, description="Field name", attribute="name"),
        'unitId': fields.Integer(required=True, description='Unit identifier (int)', attribute="unit_id"),
        'deviceTypeId': fields.Integer(required=True, description='Device Type identifier (int)', attribute="device_type_id"), 
        'loggable': fields.Boolean(required=True, description='Loggable (true/false)', attribute="loggable")
    })