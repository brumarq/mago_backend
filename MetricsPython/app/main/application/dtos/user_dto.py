from flask_restx import Namespace, fields

class UserDto:
    api = Namespace('users', description='User related operations')

    create_user_dto = api.model('create_user', {
        'email': fields.String(required=True, description='User email address'),
        'username': fields.String(required=True, description='User username'),
        'password': fields.String(required=True, description='User password'),
    })

    user_response_dto = api.model('user_retrieval', {
        'id': fields.Integer(required=True, description='User id'),
        'email': fields.String(required=True, description='User email address'),
        'username': fields.String(required=True, description='User username'),
        'admin': fields.Boolean(required=True, description='User admin'),
        'created_at': fields.DateTime(required=True, description='User creation'),
        'updated_at': fields.DateTime(required=True, description='User updatation')
    })

