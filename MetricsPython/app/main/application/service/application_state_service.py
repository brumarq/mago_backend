from app.main.application.service.abstract.base_application_state_service import ApplicationStateService
from app.main.domain.migration_status import MigrationStatus
from app.main.webapp.custommetrics.database_utils import ping_database

class ApplicationStateService(ApplicationStateService):
    
    def is_database_up(self) -> bool:
        try:
            ping_database()
            return True
        except Exception as e:
            return False
        
    def is_migration_successful(self) -> bool:
        return MigrationStatus()._instance.is_migration_sucessful