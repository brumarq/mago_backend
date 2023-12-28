from flask import current_app
import os
from manage import create_app

def test_app_should_be_development():
    app = create_app('dev')

    assert app.config['SECRET_KEY'] != 'my_precious'
    assert app.config['DEBUG'] is True
    assert current_app is not None
    assert app.config['SQLALCHEMY_DATABASE_URI'] == f"mssql+pyodbc://{os.environ.get('METRICS_DB_CONNECTION_STRING_SQLALCHEMY')}"

def test_app_should_be_testing():
    app = create_app('test')

    assert app.config['SECRET_KEY'] != 'my_precious'
    assert app.config['DEBUG'] is True
    assert app.config['SQLALCHEMY_DATABASE_URI'] == "sqlite://"

def test_app_should_be_production():
    app = create_app('prod')
    assert app.config['DEBUG'] is False
