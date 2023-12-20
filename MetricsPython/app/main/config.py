import os
from dotenv import load_dotenv

load_dotenv()

class Config:
    SECRET_KEY = os.getenv('SECRET_KEY')
    DEBUG = False
    # Swagger
    RESTX_MASK_SWAGGER = False
    # LOCAL CONNECTION STRING
    #SQLALCHEMY_DATABASE_URI=f"mssql://{os.getenv('MSSQL_USER')}/{os.getenv('MSSQL_DB')}?driver={os.getenv('MSSQL_DRIVER')}" 
    # AZURE DB CONNECTION STRING
    SQLALCHEMY_DATABASE_URI = f"mssql+pyodbc://{os.getenv('AZURE_SQL_UID')}:{os.getenv('AZURE_SQL_PWD')}@{os.getenv('AZURE_SQL_SERVER')}:{os.getenv('AZURE_SQL_HOST')}/{os.getenv('AZURE_SQL_DATABASE')}?driver={os.getenv('AZURE_SQL_DRIVER')}"
    FLASK_ENV=os.getenv('FLASK_ENV')
    ERROR_404_HELP = False

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
    ERROR_404_HELP = False


config_by_name = dict(
    dev=DevelopmentConfig,
    test=TestingConfig,
    prod=ProductionConfig
)

key = Config.SECRET_KEY
env = Config.FLASK_ENV


