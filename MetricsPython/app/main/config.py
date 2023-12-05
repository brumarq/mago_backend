import os


basedir = os.path.abspath(os.path.dirname(__file__))

class Config:
    SECRET_KEY = os.getenv('SECRET_KEY', 'my_precious_secret_key')
    DEBUG = False
    # Swagger
    RESTX_MASK_SWAGGER = False

class DevelopmentConfig(Config):
    # uncomment the line below to use postgres
    # SQLALCHEMY_DATABASE_URI = postgres_local_base
    DEBUG = True
    SQLALCHEMY_DATABASE_URI = 'mssql://ALEX/MetricsDB?driver=ODBC+Driver+17+for+SQL+Server'
    SQLALCHEMY_TRACK_MODIFICATIONS = False
    PYTHONDONTWRITEBYTECODE = True
    ERROR_404_HELP = False


class TestingConfig(Config):
    DEBUG = True
    TESTING = True
    SQLALCHEMY_DATABASE_URI = 'mssql://ALEX/MetricsDB?driver=ODBC+Driver+17+for+SQL+Server'
    PRESERVE_CONTEXT_ON_EXCEPTION = False
    SQLALCHEMY_TRACK_MODIFICATIONS = False
    PYTHONDONTWRITEBYTECODE = True
    ERROR_404_HELP = False


class ProductionConfig(Config):
    DEBUG = False
    SQLALCHEMY_DATABASE_URI = 'mssql://ALEX/MetricsDB?driver=ODBC+Driver+17+for+SQL+Server'
    # uncomment the line below to use postgres
    # SQLALCHEMY_DATABASE_URI = postgres_local_base


config_by_name = dict(
    dev=DevelopmentConfig,
    test=TestingConfig,
    prod=ProductionConfig
)

key = Config.SECRET_KEY
