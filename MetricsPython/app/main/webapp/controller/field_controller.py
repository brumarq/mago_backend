from flask_restx import Resource
from app.main.application.dtos.field_dto import FieldDto
from app.main.application.service.field_service import FieldService
from app.main.infrastructure.repositories.repository import Repository
from app.main.domain.entities.field import Field
from app.main.webapp.middleware.authentication import requires_auth

api = FieldDto.api

def initialize_field_service():
    return FieldService(Repository(Field))

field_service = initialize_field_service()

@api.route('')
class FieldCreation(Resource):
    def __init__(self, *args, **kwargs):
        super().__init__(*args, **kwargs)
        self.field_service = field_service

    @api.expect(FieldDto.field_request_dto, validate=True)
    @api.response(201, 'Field successfully created')
    @api.doc('Create new field')
    @requires_auth
    def post(self):
        """Creates a field"""
        data = api.payload

        return self.field_service.create_field(data=data)