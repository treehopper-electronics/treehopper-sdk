from treehopper.libraries import SMBusDevice
from treehopper.libraries.register_manager_adapter import RegisterManagerAdapter


class SMBusRegisterManagerAdapter(RegisterManagerAdapter):
    def __init__(self, dev: SMBusDevice):
        self._dev = dev

    def read(self, address, width):
        return self._dev.read_buffer_data(address, width)

    def write(self, address, data):
        return self._dev.write_buffer_data(address, data)
