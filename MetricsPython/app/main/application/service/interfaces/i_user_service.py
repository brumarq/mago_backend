from typing import List
from typing import Dict, Tuple
from app.main.domain.entities.user import User
from abc import ABC, abstractmethod


class IUserService(ABC):
    @abstractmethod
    def save_new_user(self, data: Dict[str, str]) -> Tuple[Dict[str, str], int]:
        pass

    @abstractmethod
    def get_all_users(self) -> List[User]:
        pass

    @abstractmethod
    def get_a_user(self, user_id: int) -> User:
        pass
