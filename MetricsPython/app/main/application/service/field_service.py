from app.main.domain.entities.field import Field
from app.main.infrastructure.repositories.repository import Repository
from app.main.application.service.abstract.base_field_service import BaseFieldService
from flask import abort
from app.main.webapp.middleware.authentication import has_required_permission
from typing import Tuple, Dict
import json

class FieldService(BaseFieldService):

    def __init__(self, field_repository: Repository(Field)):
        self.field_repository = field_repository

    def create_field(self, data) -> Tuple[Dict[str, str]]:
        
        if not (has_required_permission("admin")):
            abort(401, "This user does not have sufficient permissions. Only admins allowed.")

        self.__validate_field_parameters(data)

        new_field = Field(
            name=data["name"],
            unit_id=data["unitId"],
            device_type_id=data["deviceTypeId"],
            loggable=data["loggable"]
        )

        self.field_repository.create(new_field)

        return json.dumps(data)


    def __validate_field_parameters(self, data):
       
        if not data["name"]:
            abort(400, "The field name is required.")
        
        if data["unitId"] <= 0:
            abort(400, "Unit id cannot be 0 or negative.")

        if data["deviceTypeId"] <= 0:
            abort(400, "Device type id cannot be 0 or negative.")