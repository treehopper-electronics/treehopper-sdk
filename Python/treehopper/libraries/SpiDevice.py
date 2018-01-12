from treehopper.api import Spi
from treehopper.api.Spi import ChipSelectMode, SpiMode, SpiBurstMode
from treehopper.api.Pin import SpiChipSelectPin


class SpiDevice:
    def __init__(self, spi_module: Spi, chip_select: SpiChipSelectPin, chip_select_mode=ChipSelectMode.SpiActiveLow, speed_mhz=1, spi_mode=SpiMode.Mode00):
        self.chip_select_mode = chip_select_mode
        self.chip_select = chip_select
        self._spi = spi_module  # type: Spi
        self.frequency = speed_mhz
        self.mode = spi_mode
        self._spi.enabled = True
        chip_select.make_digital_push_pull_out()

        if chip_select_mode == ChipSelectMode.PulseLowAtBeginning or chip_select_mode == ChipSelectMode.PulseLowAtEnd or chip_select_mode == ChipSelectMode.SpiActiveLow:
            chip_select.digital_value = True
        else:
            chip_select.digital_value = False

    def send_receive(self, data_to_send, burst=SpiBurstMode.NoBurst):
        self._spi.send_receive(data_to_send, self.chip_select, self.chip_select_mode, self.frequency, burst, self.mode)
