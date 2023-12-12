import sys
sys.dont_write_bytecode = True

import unittest
from flask_migrate import Migrate
from app import blueprint
from app.main import create_app, db
from app.main.domain.entities import field, aggregated_log, log_value, log_collection, log_collection_type

app = create_app('dev')
app.register_blueprint(blueprint)

#app.app_context().push()

migrate = Migrate(app, db)

@app.shell_context_processor
def make_shell_context():
    return dict(db=db, Field=field, AggregatedLog=aggregated_log, LogValue=log_value, LogCollection=log_collection, LogCollectionType=log_collection_type)

@app.cli.command()
def test():
    """Runs the unit tests."""
    tests = unittest.TestLoader().discover('app/test', pattern='test*.py')
    result = unittest.TextTestRunner(verbosity=2).run(tests)
    if result.wasSuccessful():
        return 0
    return 1


if __name__ == '__main__':
    # Run the app in the '__main__' block
    app.run(port=app.config['PORT'], host='0.0.0.0')