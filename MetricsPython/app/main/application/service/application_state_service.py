from app.main.application.service.abstract.base_application_state_service import ApplicationStateService
from app.main.utils.migration_state import MigrationState
from app.main.utils.database_state import ping_database

class ApplicationStateService(ApplicationStateService):
    
    def is_database_up(self) -> bool:
        try:
            ping_database()
            return True
        except Exception as e:
            return False
        
    def is_migration_successful(self) -> bool:
        return MigrationState()._instance.is_migration_sucessful