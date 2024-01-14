from sqlalchemy import text
from app.main import db
import logging

def ping_database() -> None:
        try:
            db.session.execute(text('SELECT 1')) 
            db.session.commit()
            logging.info("Database ping successful.")
        except Exception as e:
            logging.error(f"Error checking database status: {e}")
            db.session.rollback()
            raise