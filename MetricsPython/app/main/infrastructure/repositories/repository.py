from sqlalchemy.exc import SQLAlchemyError
from app.main import db
from app.main.infrastructure.repositories.abstract.base_repository import BaseRepository

class Repository(BaseRepository):
    def __init__(self, model):
        self.model = model

    def create(self, entity):
        try:
            db.session.add(entity)
            db.session.commit()
            return entity
        except SQLAlchemyError as e:
            db.session.rollback()
            raise e

    def get_all(self):
        return self.model.query.all()

    def get_by_condition(self, condition):
        return self.model.query.filter(condition).first()
    
    def get_all_by_condition(self, condition):
        return self.model.query.filter(condition).all()

    def update(self, entity):
        try:
            existing_entity = self.model.query.get(entity.id)
            if existing_entity is None:
                return False
            db.session.merge(entity)
            db.session.commit()
            return True
        except SQLAlchemyError as e:
            db.session.rollback()
            raise e

    def delete(self, entity_id):
        try:
            entity = self.model.query.get(entity_id)
            if entity is None:
                return False
            db.session.delete(entity)
            db.session.commit()
            return True
        except SQLAlchemyError as e:
            db.session.rollback()
            raise e
        
    def exists_by_id(self, record_id):
        try:
            return self.model.query.get(record_id) is not None
        except SQLAlchemyError as e:
            # Handle the exception according to your application's requirements
            # For now, re-raise the exception
            raise e