from typing import List

from treehopper.libraries.register_manager_adapter import RegisterManagerAdapter
from treehopper.libraries.smbus_device import SMBusDevice
from abc import abstractmethod


def sign_extend(value, bits):
    sign_bit = 1 << (bits - 1)
    return (value & (sign_bit - 1)) - (value & sign_bit)


class Register:
    def __init__(self, reg_manager: 'RegisterManager', address: int, width: int, is_big_endian: bool):
        self._manager = reg_manager
        self.width = width
        self.address = address
        self._is_big_endian = is_big_endian

    def write(self):
        self._manager.write(self)

    def get_bytes(self)->bytes:
        if self._is_big_endian:
            return list(self.getValue().to_bytes(length=self.width, byteorder='big'))
        else:
            return list(self.getValue().to_bytes(length=self.width, byteorder='little'))

    def set_bytes(self, byte_array):
        if self._is_big_endian:
            reg_val = int.from_bytes(bytes=byte_array, byteorder='big')
        else:
            reg_val = int.from_bytes(bytes=byte_array, byteorder='little')
        self.setValue(reg_val)

    @abstractmethod
    def getValue(self)->int:
        pass

    @abstractmethod
    def setValue(self, value: int):
        pass


class RegisterManager:
    def __init__(self, manager: RegisterManagerAdapter, multi_register_access: bool):
        self.manager = manager
        self.registers = []  # type: List[Register]
        self.multi_access = multi_register_access

    def read(self, reg: Register):
        reg.set_bytes(self.manager.read(reg.address, reg.width))

    def readRange(self, start: Register, end: Register):
        start_address = start.address
        count = end.address - start.address + end.width
        regs = [x for x in self.registers if start.address <= x.address <= end.address]
        if self.multi_access:
            data = self.manager.read(start_address, count)
            i = 0
            for reg in regs:
                reg.set_bytes(data[i:i+reg.width])
                i += reg.width
        else:
            for reg in regs:
                self.read(reg)

    def write(self, reg: Register):
        self.manager.write(reg.address, reg.get_bytes())

    def writeRange(self, start: Register, end: Register):
        start_address = start.address
        count = end.address - start.address + end.address
        regs = [x for x in self.registers if start.address <= x.address <= end.address]
        if self.multi_access:
            data = []
            for reg in regs:
                data += reg.get_bytes()
            self.manager.write(start.address, data)
        else:
            for reg in regs:
                self.write(reg)
