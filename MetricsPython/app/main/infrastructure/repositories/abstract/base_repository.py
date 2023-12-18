from abc import ABC, abstractmethod

class BaseRepository(ABC):
    @abstractmethod
    def create(self, entity):
        pass

    @abstractmethod
    def get_all(self):
        pass

    @abstractmethod
    def get_by_condition(self, condition):
        pass
    
    @abstractmethod
    def get_all_by_condition(self, condition):
        pass

    @abstractmethod
    def update(self, entity):
        pass

    @abstractmethod
    def delete(self, entity_id):
        pass

    @abstractmethod
    def exists_by_id(self, record_id):
        pass
