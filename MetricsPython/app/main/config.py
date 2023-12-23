import os

class Config:
    SECRET_KEY = os.environ.get('SECRET_KEY')
    DEBUG = False
    # Swagger UI #1
    RESTX_MASK_SWAGGER = False
    # LOCAL CONNECTION STRING
    #SQLALCHEMY_DATABASE_URI=f"mssql://{os.getenv('MSSQL_USER')}/{os.getenv('MSSQL_DB')}?driver={os.getenv('MSSQL_DRIVER')}" 
    # AZURE DB CONNECTION STRING
    SQLALCHEMY_DATABASE_URI = f"mssql+pyodbc://{os.environ.get('AZURE_SQL_METRICS_UID')}:{os.environ.get('AZURE_SQL_METRICS_PWD')}@{os.environ.get('AZURE_SQL_SERVER')}/{os.environ.get('AZURE_SQL_METRICS_DB')}?driver={os.environ.get('AZURE_SQL_METRICS_DRIVER')}"
    FLASK_ENV=os.environ.get('FLASK_ENV')
    RESTX_ERROR_404_HELP = False

class DevelopmentConfig(Config):
    DEBUG = True
    SQLALCHEMY_TRACK_MODIFICATIONS = False
    PYTHONDONTWRITEBYTECODE = True

class TestingConfig(Config):
    DEBUG = True
    TESTING = True
    PRESERVE_CONTEXT_ON_EXCEPTION = False
    SQLALCHEMY_TRACK_MODIFICATIONS = False
    PYTHONDONTWRITEBYTECODE = True

class ProductionConfig(Config):
    DEBUG = False

config_by_name = dict(
    dev=DevelopmentConfig,
    test=TestingConfig,
    prod=ProductionConfig
)

key = Config.SECRET_KEY
env = Config.FLASK_ENV


