from typing import List

from treehopper.api import spi
from treehopper.api.pin import SpiChipSelectPin
from treehopper.api.spi import SpiMode, ChipSelectMode, SpiBurstMode
from treehopper.libraries import SpiDevice
from treehopper.libraries.flushable import Flushable


class ChainableShiftRegisterOutput(Flushable):
    def __init__(self,
                 spi: spi = None,
                 latch_pin: SpiChipSelectPin = None,
                 parent: 'ChainableShiftRegisterOutput' = None,
                 num_bytes=1,
                 speed_mhz=6,
                 spi_mode=SpiMode.Mode00,
                 chip_select_mode=ChipSelectMode.PulseHighAtEnd):
        super().__init__()
        self.num_bytes = num_bytes
        self.children = []
        self._current_value = []
        self._last_value = []
        self.parent = None  # type:ChainableShiftRegisterOutput
        self.dev = None  # type:SpiDevice

        if spi is not None:
            self.dev = SpiDevice(spi, latch_pin, chip_select_mode, speed_mhz, spi_mode)

        elif parent is not None:
            if parent.parent is None:
                self.parent = parent  # the parent is the most upstream node
            else:
                self.parent = parent.parent  # use the parent's parent

            self.parent.children.append(self)

    @property
    def current_value(self):
        return self._current_value

    @current_value.setter
    def current_value(self, value):
        self._current_value = value

        if self.auto_flush:
            self.flush()

    def flush(self, force=False):
        if self._current_value != self._last_value or force:
            if self.parent is not None:
                self.parent.flush()
            else:
                dev_list = list()
                if len(self.children) > 0:
                    dev_list += self.children[::-1]
                    dev_list.append(self)

                bytes = []
                for item in dev_list:
                    bytes += item.current_value[::-1]
                self.dev.send_receive(bytes, SpiBurstMode.BurstTx)
