from abc import ABC, abstractmethod
from typing import List


class IOneWire(ABC):
    """
    Abstract base class representing a 1-Wire interface.
    """
    def __init__(self):
        super().__init__()

    @abstractmethod
    def start_one_wire(self):
        """
        Place the UART into One-Wire Mode and enable the peripheral.
        """
        pass

    @abstractmethod
    def one_wire_reset_and_match_address(self, address: int):
        """
        Reset the bus and address the device specified
        @param address: the device to address
        """
        pass

    @abstractmethod
    def one_wire_search(self) -> List[int]:
        """
        Search the One-Wire Bus for devices.
        :return: a list of addresses of devices attached to the bus.
        """
        pass

    @abstractmethod
    def one_wire_reset(self) -> bool:
        """
        Reset the 1-Wire bus.
        @return: whether a device was found on the bus or not.
        """
        pass

    @abstractmethod
    def receive(self, one_wire_num_bytes=0) -> List[int]:
        """
        Receive characters from the UART
        @param one_wire_num_bytes: in OneWire mode, specifies the number of characters to read. Ignored in standard
        UART mode.
        @return: the received characters.

        Calling this function will return the current UART receive FIFO and clear it. The FIFO supports a maximum of 32
        characters.
        """
        pass

    @abstractmethod
    def send(self, data_to_send: List[int]):
        """
        Send data out of the UART
        @param data_to_send: the data to send
        """
        pass
