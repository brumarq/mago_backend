import pytest
from manage import create_app
from app.main import db
from app.main.domain.entities.field import Field
from app.main.infrastructure.repositories.repository import Repository
from app.main.infrastructure.repositories.metrics_repository import MetricsRepository
from app.main.domain.entities.weekly_average import WeeklyAverage
from app.main.domain.entities.monthly_average import MonthlyAverage
from app.main.domain.entities.yearly_average import YearlyAverage
from app.main.application.service.aggregated_logs_service import AggregatedLogsService
from app.main.application.service.metrics_service import MetricsService
from app.main.application.service.field_service import FieldService
from app.main.domain.entities.log_value import LogValue
from app import blueprint

@pytest.fixture()
def app():
    app = create_app('test')

    app.register_blueprint(blueprint)

    with app.app_context():
        db.create_all()

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

@pytest.fixture
def aggregated_logs_service(app):
    with app.app_context():
        field_repository = Repository(Field)
        weekly_average_repository = Repository(WeeklyAverage)
        monthly_average_repository = Repository(MonthlyAverage)
        yearly_average_repository = Repository(YearlyAverage)
        yield  AggregatedLogsService(
            field_repository, 
            weekly_average_repository, 
            monthly_average_repository, 
            yearly_average_repository
        )

@pytest.fixture
def metrics_service(app):
    with app.app_context():
        yield MetricsService(MetricsRepository(LogValue))


@pytest.fixture
def field_service(app):
    with app.app_context():
        yield FieldService(Repository(Field))