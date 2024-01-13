from flask_restx import Resource
from flask import Response
from app.main.application.dtos.probe_resource_namespace import ReadyResourceNameSpace
from app.main.application.dtos.probe_resource_namespace import HealthResourceNameSpace
from app.main.application.service.probe_service import ProbeService
from app.main.webapp.custommetrics.application_state import set_readiness_status, set_health_status

health_api = HealthResourceNameSpace.api
ready_api = ReadyResourceNameSpace.api

def initialize_probe_service():
    return ProbeService()

probe_service = initialize_probe_service()

@ready_api.route('', doc=False)
@ready_api.response(200, 'Successful ping to the database')
class ReadyResource(Resource):
    @ready_api.doc('ready', doc=False)
    def get(self):
        is_database_up = probe_service.is_database_up()
        is_migration_successful = probe_service.is_migration_successful()

        if (is_database_up and is_migration_successful):
            set_readiness_status(True)
            return {'status': 'ready'}, 200
        else:
            set_readiness_status(False)
            return {'status': 'failed to apply pending database migrations'}, 503
            

@health_api.route('', doc=False)
@health_api.response(200, 'Successful ping to the endpoint')
class HealthResource(Resource):
    @health_api.doc('health', doc=False)
    def get(self):
        set_health_status(True)
        return {'status': 'up'}, 200