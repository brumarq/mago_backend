from app.main.webapp.middleware.authentication import get_token_from_auth_header, has_required_permission, get_user_id
import pytest
from unittest.mock import patch
from jose import jwt

# Header and scheme tests
def test_get_token_from_auth_header_missing_header(app):
    with app.test_request_context():
        with pytest.raises(Exception) as excinfo:
            get_token_from_auth_header()
        assert '401 Unauthorized: Authorization header is expected' in str(excinfo.value)

def test_get_token_from_auth_header_invalid_scheme(app):
    with app.test_request_context(headers={'Authorization': 'InvalidScheme INVALID_ACCESS_TOKEN'}):
        with pytest.raises(Exception) as excinfo:
            get_token_from_auth_header()
        assert '401 Unauthorized: Authorization header must start with Bearer' in str(excinfo.value)

def test_get_token_from_auth_header_missing_token(app):
    with app.test_request_context(headers={'Authorization': 'Bearer'}):
        with pytest.raises(Exception) as excinfo:
            get_token_from_auth_header()
        assert '401 Unauthorized: Invalid header: Token not found' in str(excinfo.value)

def test_get_token_from_auth_header_multiple_parts(app):
    with app.test_request_context(headers={'Authorization': 'Bearer Token Part'}):
        with pytest.raises(Exception) as excinfo:
            get_token_from_auth_header()
        assert '401 Unauthorized: Authorization header must be Bearer token' in str(excinfo.value)

def test_get_token_from_auth_header_valid_token(app):
    valid_token = 'VALID_ACCESS_TOKEN'
    with app.test_request_context(headers={'Authorization': f'Bearer {valid_token}'}):
        token = get_token_from_auth_header()
        assert token == valid_token

# Permission tests

def test_has_required_permission_valid_permission(app):
    with app.test_request_context(headers={'Authorization': 'Bearer VALID_ACCESS_TOKEN'}):
        with patch('jose.jwt.get_unverified_claims') as mock_get_unverified_claims:
            mock_get_unverified_claims.return_value = {'permissions': ['client', 'admin']}
            assert has_required_permission('client') is True
            assert has_required_permission('admin') is True

def test_has_required_permission_invalid_permission(app):
    with app.test_request_context(headers={'Authorization': 'Bearer VALID_ACCESS_TOKEN'}):
        with patch('jose.jwt.get_unverified_claims') as mock_get_unverified_claims:
            mock_get_unverified_claims.return_value = {'permissions': ['admin']}
            assert has_required_permission('client') is False

def test_has_required_permission_expired_token(app):
    with app.test_request_context(headers={'Authorization': 'Bearer EXPIRED_ACCESS_TOKEN'}):
        with patch('jose.jwt.get_unverified_claims', side_effect=jwt.ExpiredSignatureError):
            with pytest.raises(jwt.ExpiredSignatureError):
                assert has_required_permission('client')

# User ID extraction tests
def test_get_user_id_valid_user_(app):
    valid_token = 'VALID_TOKEN'
    user_id = 'auth0|656a13f60300966ad3b88e23'

    with patch('app.main.webapp.middleware.authentication.get_token_from_auth_header') as mock_get_token:
        mock_get_token.return_value = valid_token

        with patch('jose.jwt.get_unverified_claims') as mock_get_unverified_claims:

            mock_get_unverified_claims.return_value = {'sub': 'auth0|656a13f60300966ad3b88e23', 'permissions': ['admin']}

            with app.test_request_context(headers={'Authorization': f'Bearer {valid_token}'}):
                user_id = get_user_id()
                assert user_id is 'auth0|656a13f60300966ad3b88e23'

def test_get_user_id_invalid_user(app):
    valid_token = 'VALID_TOKEN'

    with patch('app.main.webapp.middleware.authentication.get_token_from_auth_header') as mock_get_token:
        mock_get_token.return_value = valid_token

        with patch('jose.jwt.get_unverified_claims') as mock_get_unverified_claims:
            mock_get_unverified_claims.return_value = {'sub': 'auth0|656a13f60300966ad3b88f91', 'permissions': ['client']}

            with app.test_request_context(headers={'Authorization': f'Bearer {valid_token}'}):
                user_id = get_user_id()
                assert user_id is not 'auth0|656a13f60300966ad3b88e23'


