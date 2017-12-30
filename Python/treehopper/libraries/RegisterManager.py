from treehopper.libraries import SMBusDevice
from treehopper.libraries import Register

class RegisterManager:
    def __init__(self, dev: SMBusDevice, multi_register_access: bool):
        self.dev = dev
        self.registers = [] # type: Register
        self.multi_access = multi_register_access

    def read(self, reg: Register):
        reg.set_bytes(self.dev.read_buffer_data(register=reg.address, num_bytes=reg.width))

    def read_range(self, start: Register, end: Register):
        start_address = start.address
        count = end.address - start.address + end.width
        regs = [x for x in self.registers if start.address <= x.address <= end.address]
        if self.multi_access:
            data = self.dev.read_buffer_data(register=start_address, num_bytes=count)
            i = 0
            for reg in regs:
                reg.set_bytes(data[i:i+reg.width])
                i += reg.width
        else:
            for reg in regs:
                self.read(reg)

    def write(self, reg: Register):
        self.dev.write_buffer_data(reg.address, reg.get_bytes())

    def write_range(self, start: Register, end: Register):
        start_address = start.address
        count = end.address - start.address + end.address
        regs = [x for x in self.registers if start.address <= x.address <= end.address]
        if self.multi_access:
            data = []
            for reg in regs:
                data += reg.get_bytes()
            self.dev.write_buffer_data(start.address, data)
        else:
            for reg in regs:
                self.write(reg)
