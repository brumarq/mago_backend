from flask_restx import Resource
from app.main.application.namespaces.application_state_namespace import ReadyNamespace
from app.main.application.namespaces.application_state_namespace import HealthNamespace
from app.main.application.service.application_state_service import ApplicationStateService
from app.main.utils.application_state import set_readiness_status, set_health_status

health_api = ReadyNamespace.api
ready_api = HealthNamespace.api

def initialize_application_state_service():
    return ApplicationStateService()

application_state_service = initialize_application_state_service()

@ready_api.route('', doc=False)
@ready_api.response(200, 'Successful ping to the database')
class ReadyResource(Resource):
    @ready_api.doc('ready', doc=False)
    def get(self):
        is_database_up = application_state_service.is_database_up()
        is_migration_successful = application_state_service.is_migration_successful()

        if not is_database_up:
            set_readiness_status(False)
            return {"status": "could not connect to database."}, 503

        if not is_migration_successful:
            set_readiness_status(False)
            return {"status": "failed to apply pending database migrations"}, 503

        set_readiness_status(True)
        return {'status': 'ready'}, 200
            
            
@health_api.route('', doc=False)
@health_api.response(200, 'Successful ping to the endpoint')
class HealthResource(Resource):
    @health_api.doc('health', doc=False)
    def get(self):
        set_health_status(True)
        return {'status': 'up'}, 200
