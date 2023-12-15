import os
from dotenv import load_dotenv

load_dotenv()

class Config:
    SECRET_KEY = os.getenv('SECRET_KEY')
    DEBUG = False
    # Swagger
    RESTX_MASK_SWAGGER = False
    MSSQL_USER=os.getenv('MSSQL_USER')
    MSSQL_DB=os.getenv('MSSQL_DB')
    MSSQL_DRIVER=os.getenv('MSSQL_DRIVER')
    SQLALCHEMY_DATABASE_URI=f"mssql://{MSSQL_USER}/{MSSQL_DB}?driver={MSSQL_DRIVER}"
    FLASK_ENV=os.getenv('FLASK_ENV')

class DevelopmentConfig(Config):
    DEBUG = True
    SQLALCHEMY_TRACK_MODIFICATIONS = False
    PYTHONDONTWRITEBYTECODE = True
    ERROR_404_HELP = False
    PORT=os.getenv('FLASK_RUN_PORT_DEV')


class TestingConfig(Config):
    DEBUG = True
    TESTING = True
    PRESERVE_CONTEXT_ON_EXCEPTION = False
    SQLALCHEMY_TRACK_MODIFICATIONS = False
    PYTHONDONTWRITEBYTECODE = True
    ERROR_404_HELP = False
    PORT=os.getenv('FLASK_RUN_PORT_TEST')


class ProductionConfig(Config):
    DEBUG = False
    PORT = os.getenv('FLASK_RUN_PORT_PROD')


config_by_name = dict(
    dev=DevelopmentConfig,
    test=TestingConfig,
    prod=ProductionConfig
)

key = Config.SECRET_KEY
env = Config.FLASK_ENV


