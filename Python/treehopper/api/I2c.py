from abc import ABC, abstractmethod


class I2c(ABC):

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
        """
        Send and receive data between Treehopper and an I2C peripheral.

        This function writes write_data to the I2C peripheral at the specified address, and reads back num_bytes_to_read.

        Args:
            address: the 7-bit I2C address of the peripheral devices.
            write_data: a list of bytes to send to the peripheral device.
            num_bytes_to_read: the number of bytes to read back.

        Returns:
            A list of bytes read from the peripheral device.
        
        """
        pass
