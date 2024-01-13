from abc import ABC, abstractmethod

class ApplicationStateService(ABC):
    
    @abstractmethod
    def is_database_up() -> bool:
        pass

    @abstractmethod
    def is_migration_successful() -> bool:
        pass