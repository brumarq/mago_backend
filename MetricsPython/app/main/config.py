import os

class Config:
    SECRET_KEY = os.environ.get('SECRET_KEY')
    DEBUG = False
    RESTX_MASK_SWAGGER = False
    SQLALCHEMY_DATABASE_URI = f"mssql+pyodbc://{os.environ.get('METRICS_DB_CONNECTION_STRING_SQLALCHEMY')}"
    SQLALCHEMY_POOL_RECYCLE = 300  # Set the maximum age (in seconds) of a connection
    SQLALCHEMY_POOL_PRE_PING = True  # Enable pool pre-pinging
    FLASK_ENV=os.environ.get('FLASK_ENV')
    RESTX_ERROR_404_HELP = False
    PYTHONDONTWRITEBYTECODE = True

class DevelopmentConfig(Config):
    DEBUG = True
    SQLALCHEMY_TRACK_MODIFICATIONS = False

class TestingConfig(Config):
    DEBUG = True
    TESTING = True
    PRESERVE_CONTEXT_ON_EXCEPTION = False
    SQLALCHEMY_TRACK_MODIFICATIONS = False
    SQLALCHEMY_DATABASE_URI = "sqlite://"

class ProductionConfig(Config):
    DEBUG = False

config_by_name = dict(
    dev=DevelopmentConfig,
    test=TestingConfig,
    prod=ProductionConfig
)

key = Config.SECRET_KEY
env = Config.FLASK_ENV
