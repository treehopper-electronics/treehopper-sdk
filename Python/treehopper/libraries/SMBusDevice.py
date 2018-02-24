from treehopper.api import I2c


class SMBusDevice:
    def __init__(self, address: int, i2c_module: I2c, rate_khz=100.0):
        if address > 0x7f:
            raise ValueError("The address parameter expects a 7-bit address that doesn't include a Read/Write bit. The maximum address is 0x7F")
        self._address = address
        self._rate = rate_khz
        self._i2c = i2c_module
        self._i2c.enabled = True

    def read_byte(self):
        self._i2c.speed = self._rate
        return self._i2c.send_receive(self._address, [], 1)[0]

    def write_byte(self, data):
        self._i2c.speed = self._rate
        self._i2c.send_receive(self._address, [data], 0)

    def write_data(self, data):
        self._i2c.speed = self._rate
        self._i2c.send_receive(self._address, data, 0)

    def read_data(self, num_bytes):
        self._i2c.speed = self._rate
        return self._i2c.send_receive(self._address, None, num_bytes)

    def read_byte_data(self, register):
        self._i2c.speed = self._rate
        return self._i2c.send_receive(self._address, [register], 1)[0]

    def read_word_data(self, register):
        self._i2c.speed = self._rate
        result = self._i2c.send_receive(self._address, [register], 2)
        return result[1] << 8 | result[0]

    def read_word_data_be(self, register):
        self._i2c.speed = self._rate
        result = self._i2c.send_receive(self._address, [register], 2)
        return result[0] << 8 | result[1]

    def read_word(self):
        self._i2c.speed = self._rate
        result = self._i2c.send_receive(self._address, None, 2)
        return result[1] << 8 | result[0]

    def read_word_be(self):
        self._i2c.speed = self._rate
        result = self._i2c.send_receive(self._address, None, 2)
        return result[0] << 8 | result[1]

    def write_byte_data(self, register, data):
        self._i2c.speed = self._rate
        self._i2c.send_receive(self._address, [register, data], 0)

    def write_word_data(self, register, data):
        self._i2c.speed = self._rate
        self._i2c.send_receive(self._address, [register, data & 0xFF, data >> 8], 0)

    def write_word_data_be(self, register, data):
        self._i2c.speed = self._rate
        self._i2c.send_receive(self._address, [register, data >> 8, data & 0xFF], 0)

    def read_buffer_data(self, register, num_bytes):
        self._i2c.speed = self._rate
        return self._i2c.send_receive(self._address, [register], num_bytes)

    def write_buffer_data(self, register, data):
        self._i2c.speed = self._rate
        self._i2c.send_receive(self._address, [register]+data)
