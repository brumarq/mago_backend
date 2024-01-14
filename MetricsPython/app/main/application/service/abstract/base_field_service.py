from abc import ABC, abstractmethod
from typing import Tuple, Dict

class BaseFieldService(ABC):
    
    @abstractmethod
    def create_field(self, data) -> Tuple[Dict[str, str]]:
        pass
