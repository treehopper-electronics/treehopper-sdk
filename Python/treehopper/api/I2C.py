from abc import ABC, abstractmethod


class I2C(ABC):

    @property
    @abstractmethod
    def speed(self) -> float:
        pass

    @speed.setter
    @abstractmethod
    def speed(self, value: float):
        pass

    @property
    @abstractmethod
    def enabled(self) -> bool:
        pass

    @enabled.setter
    @abstractmethod
    def enabled(self, value: bool):
        pass

    @abstractmethod
    def send_receive(self, address: int, write_data: bytearray, num_bytes_to_read: int):
        pass
