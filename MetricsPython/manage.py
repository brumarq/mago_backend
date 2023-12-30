import sys
sys.dont_write_bytecode = True
import pytest
from flask_migrate import Migrate
from app import blueprint
from app.main import create_app, db
from app.main.domain.entities import field, log_value, log_collection, log_collection_type, weekly_average, monthly_average, yearly_average
from app.main.config import env

app = create_app(env or 'dev')

app.register_blueprint(blueprint)
app.app_context().push()

migrate = Migrate(app, db)

@app.shell_context_processor
def make_shell_context():
    return dict(db=db, Field=field, LogValue=log_value, LogCollection=log_collection, 
                LogCollectionType=log_collection_type, WeeklyAverage=weekly_average, 
                MonthlyAverage=monthly_average, YearlyAverage=yearly_average
    )

@app.cli.command()
def test():
    """Runs the unit tests."""
    result_code = pytest.main(['-v', 'app/test'])
    return result_code