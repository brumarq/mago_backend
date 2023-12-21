import os

class Config:
    SECRET_KEY = os.environ.get('SECRET_KEY')
    DEBUG = False
    # Swagger
    RESTX_MASK_SWAGGER = False
    # LOCAL CONNECTION STRING
    #SQLALCHEMY_DATABASE_URI=f"mssql://{os.getenv('MSSQL_USER')}/{os.getenv('MSSQL_DB')}?driver={os.getenv('MSSQL_DRIVER')}" 
    # AZURE DB CONNECTION STRING
    SQLALCHEMY_DATABASE_URI = f"mssql+pyodbc://{os.environ.get('AZURE_SQL_UID')}:{os.environ.get('AZURE_SQL_PWD')}@{os.environ.get('AZURE_SQL_SERVER')}:{os.environ.get('AZURE_SQL_HOST')}/{os.environ.get('AZURE_SQL_DATABASE')}?driver={os.environ.get('AZURE_SQL_DRIVER')}"
    FLASK_ENV=os.environ.get('FLASK_ENV')
    ERROR_404_HELP = False

class DevelopmentConfig(Config):
    DEBUG = True
    SQLALCHEMY_TRACK_MODIFICATIONS = False
    PYTHONDONTWRITEBYTECODE = True
    ERROR_404_HELP = False


class TestingConfig(Config):
    DEBUG = True
    TESTING = True
    PRESERVE_CONTEXT_ON_EXCEPTION = False
    SQLALCHEMY_TRACK_MODIFICATIONS = False
    PYTHONDONTWRITEBYTECODE = True
    ERROR_404_HELP = False


class ProductionConfig(Config):
    DEBUG = False
    ERROR_404_HELP = False


config_by_name = dict(
    dev=DevelopmentConfig,
    test=TestingConfig,
    prod=ProductionConfig
)

key = Config.SECRET_KEY
env = Config.FLASK_ENV


