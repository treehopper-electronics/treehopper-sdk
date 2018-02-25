from treehopper.api import Spi
from treehopper.api.Spi import ChipSelectMode, SpiMode, SpiBurstMode
from treehopper.api.Pin import SpiChipSelectPin


class SpiDevice:
    """
    SPI device

    This class provides a simple wrapper around Spi.send_receive() that preserves the chip-select, mode, and frequency
    configurations.
    """
    def __init__(self, spi_module: Spi, chip_select: SpiChipSelectPin, chip_select_mode=ChipSelectMode.SpiActiveLow, speed_mhz=6, spi_mode=SpiMode.Mode00):
        """
        Construct a new SPI device.
        :param spi_module: The SPI module to use
        :param chip_select: The chip select pin to use
        :param chip_select_mode: The chip select mode to use (e.g., ChipSelectMode.SpiActiveLow)
        :param speed_mhz: The speed, in MHz, to use.
        :param spi_mode: The SPI mode (e.g., SpiMode.Mode00) to use.

        Due to an open firmware issue, we recommend avoiding frequencies between 0.8 and 6 MHz, as these can cause ISR glitches under certain bus constraints.
        """
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
        """
        Send and/or receive data using the settings specified by the constructor.
        :param data_to_send: A list-like object of data to send
        :param burst: the SPI burst mode (choose from SpiBurstMode members).
        :return: Data received during the transaction.
        """
        self._spi.send_receive(data_to_send, self.chip_select, self.chip_select_mode, self.frequency, burst, self.mode)
