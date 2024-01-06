from app.main.domain.entities.field import Field
from app.main.infrastructure.repositories.repository import Repository
from app.main.application.service.abstract.base_field_service import BaseFieldService
from flask import abort
from app.main.webapp.middleware.authentication import has_required_permission
from typing import Tuple, Dict

class FieldService(BaseFieldService):

    def __init__(self, field_repository: Repository(Field)):
        self.field_repository = field_repository

    def create_field(self, name, unit_id, device_type_id, loggable) -> Tuple[Dict[str, str], int]:
        
        self.__validate_field_parameters(name, unit_id, device_type_id)

        new_field = Field(
            name=name,
            unit_id=unit_id,
            device_type_id=device_type_id,
            loggable=loggable
        )

        self.field_repository.create(new_field) 

        response_object = {
            'status': 'Success',
            'message': 'Field has been successfully created.',
        }
        return response_object, 201
  

    def __validate_field_parameters(self, name, unit_id, device_type_id):

        if not (has_required_permission("admin")):
            abort(401, "This user does not have sufficient permissions. Only admins allowed.")

        if not name:
            abort(400, "The field name is required.")
        
        if unit_id <= 0:
            abort(400, "Unit id cannot be 0 or negative.")

        if device_type_id <= 0:
            abort(400, "Device type id cannot be 0 or negative.")