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

    def get_all(self, page_number=None, page_size=None):
        try:
            query = self.model.query

            page_number = page_number or 1
            page_size = page_size or 50

            query = query.order_by(self.model.created_at).offset((page_number - 1) * page_size).limit(page_size)

            return query.all()
        except SQLAlchemyError as e:
            db.session.rollback()
            raise e

    def get_by_condition(self, condition):
        try:
            return self.model.query.filter(condition).first()
        except SQLAlchemyError as e:
            db.session.rollback()
            raise e
    
    def get_all_by_condition(self, condition, page_number=None, page_size=None):
        try:
            query = self.model.query.filter(condition)

            page_number = page_number or 1
            page_size = page_size or 50

            query = query.order_by(self.model.created_at).offset((page_number - 1) * page_size).limit(page_size)

            return query.all()

        except SQLAlchemyError as e:
            db.session.rollback()
            raise e

    def update(self, entity):
        try:
            existing_entity = db.session.get(self.model, entity.id)
            if existing_entity is None:
                return False
            db.session.merge(entity)
            db.session.commit()
            return True
        except SQLAlchemyError as e:
            db.session.rollback()
            raise e

    def delete(self, entity_id) -> bool:
        try:
            entity = db.session.get(self.model, entity_id)
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
            return db.session.get(self.model, record_id) is not None
        except SQLAlchemyError as e:
            db.session.rollback()
            raise e
