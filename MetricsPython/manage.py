import sys
sys.dont_write_bytecode = True

from flask_migrate import Migrate

from app import blueprint
from app.main import create_app, db
from app.main.domain.entities import user, field, aggregated_log, log_value, log_collection, log_collection_type

app = create_app('dev')
app.register_blueprint(blueprint)

app.app_context().push()

migrate = Migrate(app, db)

@app.shell_context_processor
def make_shell_context():
    return dict(db=db, User=user, Field=field, AggregatedLog=aggregated_log, LogValue=log_value, LogCollection=log_collection, LogCollectionType=log_collection_type)