import unittest

from flask import current_app
from flask_testing import TestCase
import os

from manage import app

class TestDevelopmentConfig(TestCase):
    def create_app(self):
        app.config.from_object('app.main.config.DevelopmentConfig')
        return app

    def test_app_should_be_development(self):
        self.assertFalse(app.config['SECRET_KEY'] == 'my_precious')
        self.assertTrue(app.config['DEBUG'] is True)
        self.assertFalse(current_app is None)
        self.assertTrue(
            app.config['SQLALCHEMY_DATABASE_URI'] == f"mssql+pyodbc://{os.environ.get('AZURE_SQL_UID')}:{os.environ.get('AZURE_SQL_PWD')}@{os.environ.get('AZURE_SQL_SERVER')}:{os.environ.get('AZURE_SQL_HOST')}/{os.environ.get('AZURE_SQL_DATABASE')}?driver={os.environ.get('AZURE_SQL_DRIVER')}"
        )

class TestTestingConfig(TestCase):
    def create_app(self):
        app.config.from_object('app.main.config.TestingConfig')
        return app

    def test_app_should_be_testing(self):
        self.assertFalse(app.config['SECRET_KEY'] == 'my_precious')
        self.assertTrue(app.config['DEBUG'])
        self.assertTrue(
            app.config['SQLALCHEMY_DATABASE_URI'] == f"mssql+pyodbc://{os.environ.get('AZURE_SQL_UID')}:{os.environ.get('AZURE_SQL_PWD')}@{os.environ.get('AZURE_SQL_SERVER')}:{os.environ.get('AZURE_SQL_HOST')}/{os.environ.get('AZURE_SQL_DATABASE')}?driver={os.environ.get('AZURE_SQL_DRIVER')}"
        )

class TestProductionConfig(TestCase):
    def create_app(self):
        app.config.from_object('app.main.config.ProductionConfig')
        return app

    def test_app_should_be_production(self):
        self.assertTrue(app.config['DEBUG'] is False)


if __name__ == '__main__':
    unittest.main()