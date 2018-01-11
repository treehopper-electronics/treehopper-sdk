from treehopper.libraries import RegisterManager
from abc import abstractmethod


def sign_extend(value, bits):
    sign_bit = 1 << (bits - 1)
    return (value & (sign_bit - 1)) - (value & sign_bit)


class Register:
    def __init__(self, reg_manager: RegisterManager, address: int, width: int, is_big_endian: bool):
        self._manager = reg_manager
        self.width = width
        self.address = address
        self._is_big_endian = is_big_endian

    def write(self):
        self._manager.write(self)

    def get_bytes(self)->bytes:
        if self._is_big_endian:
            return list(self.get_value().to_bytes(length=self.width, byteorder='big'))
        else:
            return list(self.get_value().to_bytes(length=self.width, byteorder='little'))

    def set_bytes(self, byte_array):
        if self._is_big_endian:
            reg_val = int.from_bytes(bytes=byte_array, byteorder='big')
        else:
            reg_val = int.from_bytes(bytes=byte_array, byteorder='little')
        self.set_value(reg_val)

    @abstractmethod
    def get_value(self)->int:
        pass

    @abstractmethod
    def set_value(self, value: int):
        pass