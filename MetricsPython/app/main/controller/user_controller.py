from flask import request
from flask_restx import Resource, reqparse

from app.main.application.dtos.user_dto import UserDto
from app.main.application.service.interfaces.i_user_service import IUserService
from app.main.application.service.user_service import UserService
from typing import Dict, Tuple

api = UserDto.api  # API namespace has to be the SAME, however, api.domain can be different. (check dto.py)

user_service = UserService()  # Instantiate service

@api.route('/')
class UserList(Resource):

    @api.doc('list_of_registered_users')
    @api.marshal_list_with(UserDto.user_response_dto)
    def get(self):
        """List all registered users"""
        return user_service.get_all_users()

    @api.expect(UserDto.create_user_dto, validate=True)
    @api.response(201, 'User successfully created.')
    @api.doc('create a new user')
    def post(self) -> Tuple[Dict[str, str], int]:
        """Creates a new User """
        data = request.json
        return user_service.save_new_user(data=data)


@api.route('/<int:id>')  # Updated route definition to accept an integer identifier
@api.param('id', 'The User identifier')
class User(Resource):

    @api.doc('get a user')
    @api.marshal_with(UserDto.user_response_dto)
    def get(self, id: int):
        """get a user given its identifier"""
        return user_service.get_a_user(id)

    @api.response(204, 'User successfully deleted.')
    @api.doc('delete a user')
    def delete(self, id):
        """Deletes a user given its identifier"""
        return user_service.delete_a_user(id)