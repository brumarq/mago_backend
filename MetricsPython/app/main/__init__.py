from flask import Flask
from flask_sqlalchemy import SQLAlchemy
from flask_bcrypt import Bcrypt

from app.main.config import config_by_name
from flask.app import Flask
from authlib.integrations.flask_client import OAuth

import os
from dotenv import load_dotenv

load_dotenv()

db = SQLAlchemy()
flask_bcrypt = Bcrypt()
oauth = OAuth()

def create_app(config_name: str) -> Flask:
    app = Flask(__name__)
    app.config.from_object(config_by_name[config_name])
    db.init_app(app)
    flask_bcrypt.init_app(app)
    oauth.init_app(app)

    configure_oauth()

    return app

def configure_oauth():
    oauth.register(
        "auth0",
        client_id=os.getenv('AUTH0_CLIENT_ID'),
        client_secret=os.getenv('AUTH0_CLIENT_SECRET'),
        client_kwargs={
            "scope": "openid profile email",         
        },
        server_metadata_url=f"https://{os.getenv('AUTH0_DOMAIN')}/.well-known/openid-configuration"
    )  

