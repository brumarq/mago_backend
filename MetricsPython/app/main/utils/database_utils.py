from sqlalchemy import text
from app.main import db
import logging
import time

def ping_database() -> None:
        try:
            db.session.execute(text('SELECT 1')) 
            db.session.commit()
        except Exception as e:
            logging.error(f"Error checking database status: {e}")
            db.session.rollback()
            raise

def ping_database_periodically():
    while True:
        try:
            ping_database()
            logging.info("Database ping successful.")
        except Exception as e:
            logging.error(f"Error during periodic database ping: {e}")
        time.sleep(10)  # ping every 5