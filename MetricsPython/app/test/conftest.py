import pytest
from manage import create_app
from app.main import db
from app.main.domain.entities.field import Field
from app.main.infrastructure.repositories.repository import Repository
from unittest.mock import MagicMock


@pytest.fixture()
def app():
    app = create_app('test')
    
    with app.app_context():
        db.create_all()

    print("CREATING DATABASE")

    yield app

@pytest.fixture()
def client(app):
    return app.test_client()

@pytest.fixture
def repository(request, app):
    model_class = request.param

    with app.app_context():
        repository = Repository(model_class)
        yield repository
