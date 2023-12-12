from flask_restx import Api
from flask import Blueprint

from app.main.controller.metrics_controller import api as metrics_ns

blueprint = Blueprint('api', __name__)

api = Api(
    blueprint,
    title='Metrics microservice',
    version='3.0',
    description='Swagger UI and documentation for the metrics microservice',
)

api.add_namespace(metrics_ns, path="/metrics")

