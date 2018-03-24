import logging
import math
from typing import List, TYPE_CHECKING, Sequence

if TYPE_CHECKING:
    from treehopper.api.TreehopperUsb import TreehopperUsb
from treehopper.api.Spi import Spi, SpiBurstMode, ChipSelectMode, SpiMode
from treehopper.api.Pin import PinMode
from treehopper.utils.utils import *
from treehopper.api.DeviceCommands import DeviceCommands


class HardwareSpi(Spi):
    """
    Hardware SPI module
    """
    def __init__(self, board: 'TreehopperUsb'):
        self._board = board
        self._enabled = False
        self._logger = logging.getLogger(__name__)
        self.sck = self._board.pins[0]
        self.miso = self._board.pins[1]
        self.mosi = self._board.pins[2]

    @property
    def enabled(self) -> bool:
        return self._enabled

    @enabled.setter
    def enabled(self, value: bool):
        if value == self._enabled:
            return

        self._enabled = value
        if self._enabled:
            self.sck.mode  = PinMode.Reserved
            self.miso.mode = PinMode.Reserved
            self.mosi.mode = PinMode.Reserved
        else:
            self.sck.mode  = PinMode.Unassigned
            self.miso.mode = PinMode.Unassigned
            self.mosi.mode = PinMode.Unassigned

        self._send_config()

    def send_receive(self, data_to_write: List[int], chip_select=None, chip_select_mode=ChipSelectMode.SpiActiveLow,
                     speed_mhz=6, burst_mode=SpiBurstMode.NoBurst, spi_mode=SpiMode.Mode00) -> List[int]:
        if not self._enabled:
            self._logger.error("spi.send_receive() called before enabling the peripheral. This call will be ignored.")
            return

        if chip_select is not None and chip_select.spi_module != self:
            self._logger.error("Chip select pin must belong to this SPI module. This call will be ignored.")

        if chip_select is not None:
            chip_select_pin_number = chip_select.number
        else:
            chip_select_pin_number = 255

        if not data_to_write:
            tx_size = 0
        else:
            tx_size = len(data_to_write)

        if 0.8 < speed_mhz < 6:
            self._logger.info("NOTICE: automatically rounding up SPI speed to 6 MHz, due to a possible silicon bug. "
                              "This bug affects SPI speeds between 0.8 and 6 MHz, so if you need a speed lower than 6 "
                              "MHz, please set to 0.8 MHz or lower.")
            speed_mhz = 6

        spi0_ckr = round(24.0 / speed_mhz - 1)
        if spi0_ckr > 255.0:
            spi0_ckr = constrain(spi0_ckr, 0, 255)
            self._logger.error("NOTICE: Requested SPI frequency of {} MHz is below the minimum frequency, and will be "
                               "clipped to 0.09375 MHz (93.75 kHz).".format(speed_mhz))
        elif spi0_ckr < 0:
            spi0_ckr = constrain(spi0_ckr, 0, 255)
            self._logger.error(
                "NOTICE: Requested SPI frequency of {} MHz is above the maximum frequency, and will be clipped to 24 "
                "MHz.".format(speed_mhz))

        actual_frequency = 48.0 / (2.0 * (spi0_ckr + 1.0))

        if math.isclose(actual_frequency, speed_mhz, abs_tol=1):
            self._logger.info("NOTICE: SPI module actual frequency of {} MHz is more than 1 MHz away from the "
                              "requested frequency of {} MHz".format(actual_frequency, speed_mhz))

        if tx_size > 255:
            self._logger.error("Maximum packet length is 255 bytes")

        header = [DeviceCommands.SpiTransaction, chip_select_pin_number, chip_select_mode, spi0_ckr, spi_mode,
                  burst_mode, tx_size]

        with self._board._comms_lock:
            if burst_mode == SpiBurstMode.BurstRx:
                self._board._send_peripheral_config_packet(header)
            else:
                data_to_send = header + data_to_write
                data_chunks = chunks(data_to_send, 64)
                for chunk in data_chunks:
                    self._board._send_peripheral_config_packet(chunk)

                read_data = []
                if burst_mode != SpiBurstMode.BurstTx:
                    bytes_remaining = tx_size

                    while bytes_remaining > 0:
                        if bytes_remaining > 64:
                            num_bytes_to_read = 64
                        else:
                            num_bytes_to_read = bytes_remaining

                        read_data += self._board._receive_comms_response_packet(num_bytes_to_read)
                        bytes_remaining -= num_bytes_to_read

                return read_data

    def _send_config(self):
        data_to_send = [DeviceCommands.SpiConfig, self._enabled]
        self._board._send_peripheral_config_packet(data_to_send)

    def __str__(self):
        if self.enabled:
            return "Not enabled"
        else:
            return "Enabled"