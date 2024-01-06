from typing import List
from abc import ABC, abstractmethod
from typing import Tuple, Dict

class BaseFieldService(ABC):
    
    @abstractmethod
    def create_field(self, name, unit_id, device_type_id, loggable) -> Tuple[Dict[str, str], int]:
        pass
