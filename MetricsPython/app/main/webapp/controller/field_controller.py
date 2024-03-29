from flask_restx import Resource
from app.main.application.namespaces.field_namespace import FieldNamespace
from app.main.application.service.field_service import FieldService
from app.main.infrastructure.repositories.repository import Repository
from app.main.domain.entities.field import Field
from app.main.webapp.middleware.authentication import requires_auth
from flask import Response
import json

api = FieldNamespace.api

def initialize_field_service():
    return FieldService(Repository(Field))

field_service = initialize_field_service()

@api.route('')
class FieldCreation(Resource):
    def __init__(self, *args, **kwargs):
        super().__init__(*args, **kwargs)
        self.field_service = field_service

    @api.expect(FieldNamespace.field_request_dto, validate=True)
    @api.response(201, 'Field successfully created')
    @api.doc('Create new field')
    @requires_auth
    def post(self):
        """Creates a field"""
        data = api.payload

        field_json_data = self.field_service.create_field(data=data)

        return Response(field_json_data, status=201, mimetype="application/json")