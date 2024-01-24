from flask import Flask
from flask_sqlalchemy import SQLAlchemy
from app.main.config import config_by_name
import logging

db = SQLAlchemy()

def create_app(config_name: str) -> Flask:
    app = Flask(__name__)
    app.config.from_object(config_by_name[config_name])

    __add_log_configuration(app)

    db.init_app(app)
    
    return app

def __add_log_configuration(app: Flask):
    # Configure Werkzeug logger
    werkzeug_logger = logging.getLogger('werkzeug')
    werkzeug_logger.setLevel(logging.DEBUG)

    # Configure a StreamHandler to capture logs to stdout
    stdout_handler = logging.StreamHandler()
    stdout_handler.setLevel(logging.DEBUG)

    # Create a formatter for the log messages
    formatter = logging.Formatter('%(asctime)s - %(name)s - %(levelname)s - %(message)s')
    stdout_handler.setFormatter(formatter)

    # Add the StreamHandler to the Werkzeug logger
    werkzeug_logger.addHandler(stdout_handler)

    # Configure Flask application logger
    app_logger = app.logger
    app_logger.setLevel(logging.DEBUG)  # Adjust the level as needed

    # Add the StreamHandler to the Flask application logger
    app_logger.addHandler(stdout_handler)