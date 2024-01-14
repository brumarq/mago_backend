from sqlalchemy.exc import SQLAlchemyError
from app.main import db
from app.main.infrastructure.repositories.abstract.base_repository import BaseRepository

class Repository(BaseRepository):
    def __init__(self, model):
        self.model = model

    def create(self, entity):
        try:
            with db.session.begin():
                db.session.add(entity)
            return entity
        except SQLAlchemyError as e:
            db.session.rollback()
            raise e

    def get_all(self):
        try:
            return self.model.query.all()
        except SQLAlchemyError as e:
            db.session.rollback()
            raise e

    def get_by_condition(self, condition):
        try:
            return self.model.query.filter(condition).first()
        except SQLAlchemyError as e:
            db.session.rollback()
            raise e
    
    def get_all_by_condition(self, condition):
        try:
            return self.model.query.filter(condition).all()
        except SQLAlchemyError as e:
            db.session.rollback()
            raise e

    def update(self, entity):
        try:
            with db.session.begin():
                existing_entity = db.session.get(self.model, entity.id)
                if existing_entity is None:
                    return False
                db.session.merge(entity)
            return True
        except SQLAlchemyError as e:
            db.session.rollback()
            raise e

    def delete(self, entity_id):
        try:
            with db.session.begin():
                entity = db.session.get(self.model, entity_id)
                if entity is None:
                    return False
                db.session.delete(entity)
            return True
        except SQLAlchemyError as e:
            db.session.rollback()
            raise e
        
    def exists_by_id(self, record_id):
        try:
            with db.session.begin():
                return db.session.get(self.model, record_id) is not None
        except SQLAlchemyError as e:
            db.session.rollback()
            raise e
