from app.main import db
from sqlalchemy import text
import logging
from app.main.application.service.abstract.base_application_state_service import ApplicationStateService
from app.main.domain.migration_status import MigrationStatus

class ApplicationStateService(ApplicationStateService):
    
    def is_database_up(self) -> bool:
        try:
            db.session.execute(text('SELECT 1')) 
            db.session.commit()
            return True
        except Exception as e:
            logging.error(f"Error checking database status: {e}")
            db.session.rollback()
            return False
        
    def is_migration_successful(self) -> bool:
        return MigrationStatus()._instance.is_migration_sucessful