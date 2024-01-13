import pytest
from flask_migrate import Migrate, upgrade
from app import blueprint
from app.main import create_app, db
from app.main.domain.entities import field, log_value, log_collection, log_collection_type, weekly_average, monthly_average, yearly_average
from app.main.config import env
from app.main.domain.migration_status import MigrationStatus
import logging

env = env or 'prod' # if no env, assume its production

app = create_app(env)

app.register_blueprint(blueprint)
app.app_context().push()

migrate = Migrate(app, db)

def run_migrations(): # migration on start-up
    with app.app_context():
        try:
            upgrade()
            MigrationStatus()._instance.is_migration_sucessful = True
        except Exception as e:
            logging.error(f"Migration failed: {str(e)}")
            MigrationStatus()._instance.is_migration_sucessful = False

if not any("pytest" in arg.lower() for arg in sys.argv): #and env == 'prod': #if its a pytest or not production, then ignore the migrations (bc it uses a diff database)
    run_migrations()

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