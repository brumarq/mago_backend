from flask_restx import Namespace

class ReadyNamespace: 
    api = Namespace('Ready', description="Ready related operations")

class HealthNamespace: 
    api = Namespace('Health', description="Health related operations")