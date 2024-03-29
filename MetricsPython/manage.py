import pytest
from flask_migrate import Migrate, upgrade
from app import blueprint
from app.main import create_app, db
from app.main.domain.entities import field, log_value, log_collection, log_collection_type, weekly_average, monthly_average, yearly_average
from app.main.config import env
from app.main.utils.migration_state import MigrationState
from app.main.utils.database_state import ping_database
from app.main.utils.application_state import track_request_duration_and_count
import logging
import sys
import atexit
from apscheduler.schedulers.background import BackgroundScheduler
import time
from flask import request

env = env or 'prod' # if no env, assume its production

app = create_app(env)

app.register_blueprint(blueprint)
app.app_context().push()

migrate = Migrate(app, db)

# (Start) Tracking for prometheus after every request (track time before and use it in after)
@app.before_request
def before_request():
    request.start_time = time.time()

@app.after_request
def after_request(response):
    track_request_duration_and_count(response)
    return response

# (End tracking)

# Periodic pings to wake up Azure SQL from its idle state (30 min) -> ping happens every 15 min
def thd_ping_database_periodically():
    with app.app_context():
        ping_database()

scheduler = BackgroundScheduler()
scheduler.add_job(id="ping_job", func=thd_ping_database_periodically, trigger='interval', minutes=15)
scheduler.start()

# Migrations on start-up
def run_migrations(): 
    with app.app_context():
        try:
            upgrade()
            MigrationState()._instance.is_migration_sucessful = True
        except Exception as e:
            logging.error(f"Migration failed: {str(e)}")
            MigrationState()._instance.is_migration_sucessful = False

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

atexit.register(lambda: scheduler.shutdown())