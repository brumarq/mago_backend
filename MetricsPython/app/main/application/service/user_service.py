from app.main.domain.entities.user import User
from typing import Dict, Tuple, List
from app.main.application.service.interfaces.i_user_service import IUserService
from app.main.infrastructure.repositories.repository import Repository
from flask import abort

class UserService(IUserService):
    
    def __init__(self):
        self.user_repository = Repository(User)

    def save_new_user(self, data: Dict[str, str]) -> Tuple[Dict[str, str], int]:

        user = self.user_repository.get_by_condition(User.email == data['email'])

        if user:
            abort(409, f"User with the email '{data['email']}' already exists!")

        new_user = User(
            email=data['email'],
            username=data['username'],
            password=data['password']
        )

        self.user_repository.create(new_user)

        response_object = {
            'status': 'Success',
            'message': 'User has been successfully created.',
        }
        return response_object, 201

    def get_all_users(self) -> List[User]:
        try:
            return self.user_repository.get_all()
        except Exception as e:
            raise Exception(f"Error getting all users: {str(e)}")

    def get_a_user(self, user_id: int) -> Tuple[Dict[str, str], int]:

        user = self.user_repository.get_by_condition(User.id == user_id)

        if not user:
            abort(404, f"The user with id {user_id} was not found!")
        
        return user
        

    def delete_a_user(self, user_id: int) -> str:
        user = self.user_repository.get_by_condition(User.id == user_id)

        if not user:
            abort(404, f"The user with id {user_id} was not found!")

        self.user_repository.delete(user.id)

        return f"User with id {user_id} has been successfully removed"
