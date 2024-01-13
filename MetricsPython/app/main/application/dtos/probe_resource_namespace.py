from flask_restx import Namespace

class ReadyResourceNameSpace: 
    api = Namespace('Ready', description="Ready related operations")

class HealthResourceNameSpace: 
    api = Namespace('Health', description="Health related operations")