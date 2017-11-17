import logging
import math
from treehopper.api.I2C import I2C
from treehopper.utils.utils import *
from treehopper.api.DeviceCommands import DeviceCommands
from treehopper.api.PinMode import PinMode

class I2CTransferError:
    ArbitrationLostError, NackError, UnknownError, TxunderError = range(4)
    Success = 255

    def ErrorString(val):
        if val == I2CTransferError.ArbitrationLostError:
            return "Arbitration lost"
        elif val == I2CTransferError.NackError:
            return "Nack error"
        elif val == I2CTransferError.UnknownError:
            return "Unknown error"
        elif val == I2CTransferError.TxunderError:
            return "TX underrun"
        elif val == I2CTransferError.Success:
            return "Success"


class HardwareI2C(I2C):
    def __init__(self, board):
        self._board = board
        self._enabled = False
        self._speed = 100
        self._logger = logging.getLogger(__name__)

    @property
    def speed(self) -> float:
        return self._speed

    @speed.setter
    def speed(self, value: float):
        if math.isclose(value, self._speed):
            return

        self._speed = value
        self._send_config()

    @property
    def enabled(self) -> bool:
        return self._enabled

    @enabled.setter
    def enabled(self, value: bool):
        if value == self._enabled:
            return

        self._enabled = value
        if self._enabled:
            self._board.pins[3].mode = PinMode.Reserved
            self._board.pins[4].mode = PinMode.Reserved
        else:
            self._board.pins[3].mode = PinMode.Unassigned
            self._board.pins[4].mode = PinMode.Unassigned

        self._send_config()

    def send_receive(self, address: int, write_data=None, num_bytes_to_read=0) -> bytearray:
        if not self._enabled:
            self._logger.error("I2c.send_receive() called before enabling the peripheral. This call will be ignored.")
            return
        if not write_data:
            tx_size = 0
        else:
            tx_size = len(write_data)
        with self._board._comms_lock:
            send_data = [DeviceCommands.I2cTransaction, address, tx_size, num_bytes_to_read]
            if write_data:
                send_data += write_data
            data_chunks = chunks(send_data, 64)
            for chunk in data_chunks:
                self._board._send_peripheral_config_packet(chunk)

            read_data = []
            bytes_remaining = num_bytes_to_read + 1

            while bytes_remaining > 0:
                if bytes_remaining > 64:
                    num_bytes_to_read = 64
                else:
                    num_bytes_to_read = bytes_remaining

                read_data += self._board._receive_comms_response_packet(num_bytes_to_read)
                bytes_remaining -= num_bytes_to_read
            if read_data[0] != 255:
                raise RuntimeError("I2C transaction resulted in an error: {}".format(I2CTransferError.ErrorString(read_data[0])))

            return read_data[1:]

    def _send_config(self):
        th0 = 256.0 - 4000.0 / (3.0 * self._speed)
        if th0 < 0 or th0 > 255.0:
            self._logger.warning("Rate out of limits. Valid rate is 62.5 kHz - 16000 kHz (16 MHz). Clipping.")
            th0 = constrain(th0, 255, 0)
        data_to_send = [DeviceCommands.I2cConfig, self._enabled, round(th0)]
        self._board._send_peripheral_config_packet(data_to_send)

    def __str__(self):
        if self.enabled:
            return "Not enabled"
        else:
            return "Enabled, {:0.2f} kHz".format(self._speed)