from flask_testing import TestCase

from manage import app
from app.main import db

class BaseTest(TestCase):
    def create_app(self):
        app.config.from_object('app.main.config.TestingConfig')
        return app
    
    def set_up(self):
        db.create_all()
        db.session.commit()

    def tear_down(self):
        db.session.remove()
        db.drop_all()
