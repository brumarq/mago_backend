from app.main import db
from sqlalchemy import text
import logging

class ProbeService:
    
    def is_database_up(self) -> bool:
        try:
            db.session.execute(text('SELECT 1')) 
            db.session.commit()
            return True
        except Exception as e:
            logging.error(f"Error checking database status: {e}")
            return False
  