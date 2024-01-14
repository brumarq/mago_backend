from flask import Flask
from flask_sqlalchemy import SQLAlchemy
from app.main.config import config_by_name
from flask.app import Flask
import logging
import sys

db = SQLAlchemy()

def create_app(config_name: str) -> Flask:
    app = Flask(__name__)
    app.config.from_object(config_by_name[config_name])
    app.logger.addHandler(logging.StreamHandler(sys.stdout))
    app.logger.setLevel(logging.INFO)
    db.init_app(app)
    
    return app

