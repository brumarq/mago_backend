# Singleton class to keep track of whether migration was successful during boot-up of the application.

class MigrationState:
    _instance = None

    def __new__(cls):
        if not cls._instance:
            cls._instance = super(MigrationState, cls).__new__(cls)
            cls._instance.is_migration_sucessful = None 
        return cls._instance