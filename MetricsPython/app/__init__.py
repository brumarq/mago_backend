from flask_restx import Api
from flask import Blueprint
from app.main.webapp.controller.metrics_controller import api as metrics_ns
from app.main.webapp.controller.aggregated_logs_controller import api as aggregated_logs_ns

blueprint = Blueprint('api', __name__)
authorizations = {
    'apikey': {
        'type': 'apiKey',
        'in': 'header',
        'name': 'Authorization'
    }
}

api = Api(
    blueprint,
    title='Metrics microservice',
    version='1.0',
    description='Swagger UI and documentation for the metrics microservice',
    authorizations=authorizations,
    security='apikey'
)

api.add_namespace(metrics_ns, path="/metrics")
api.add_namespace(aggregated_logs_ns, path="/aggregated-logs")