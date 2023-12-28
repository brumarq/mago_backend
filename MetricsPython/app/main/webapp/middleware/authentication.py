from flask import request, _request_ctx_stack, abort
from flask.globals import request_ctx
from six.moves.urllib.request import urlopen
from functools import wraps
from jose import jwt
import json
import os

def get_token_from_auth_header():
    """Obtains the Access Token from the Authorization Header
    """
    auth = request.headers.get("Authorization", None)
    if not auth:
        abort(401, "Authorization header is expected")

    parts = auth.split()

    if parts[0].lower() != "bearer":
        abort(401, "Authorization header must start with" " Bearer")
    elif len(parts) == 1:
        abort(401, "Invalid header: Token not found")
    elif len(parts) > 2:
        abort(401, "Authorization header must be" " Bearer token")

    token = parts[1]
    return token

def requires_auth(f):
    """Determines if the Access Token is valid
    """
    @wraps(f)
    def decorated(*args, **kwargs):
        token = get_token_from_auth_header()
        jsonurl = urlopen(f"https://{os.environ.get('AUTH0_DOMAIN')}/.well-known/jwks.json")
        jwks = json.loads(jsonurl.read())
        unverified_header = jwt.get_unverified_header(token)
        rsa_key = {}
        for key in jwks["keys"]:
            if key["kid"] == unverified_header["kid"]:
                rsa_key = {
                    "kty": key["kty"],
                    "kid": key["kid"],
                    "use": key["use"],
                    "n": key["n"],
                    "e": key["e"]
                }
        if rsa_key:
            try:
                payload = jwt.decode(
                    token,
                    rsa_key,
                    algorithms=['RS256'],
                    audience=os.environ.get('AUTH0_AUDIENCE'),
                    issuer=f"https://{os.environ.get('AUTH0_DOMAIN')}/"
                )
            except jwt.ExpiredSignatureError:
                abort(401, "Token has expired")
            except jwt.JWTClaimsError:
                abort(401, "Incorrect claims: please check the audience and issuer")
            except Exception:
                abort(401, "Unable to parse authentication token")

            _request_ctx_stack.top.current_user = payload
            return f(*args, **kwargs)
        abort(401, "Unable to find appropriate key")
    return decorated


def has_required_permission(required_permission):
    """Determines if the required permission is present in the "permissions" claim of the Access Token
    Args:
        required_permission (str): The permission required to access the resource
    Avaialble permissions: client, admin
    """
    token = get_token_from_auth_header()
    unverified_claims = jwt.get_unverified_claims(token)
    
    # Assuming "permissions" is a list in the claims
    if unverified_claims.get("permissions"):
        token_permissions = unverified_claims["permissions"]
        
        # Check if the required permission is in the list
        if required_permission in token_permissions:
            return True
    
    return False