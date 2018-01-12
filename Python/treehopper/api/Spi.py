from abc import ABC, abstractmethod


class SpiBurstMode:
    """The burst mode to use with an SPI interface"""

    NoBurst = 0
    """No burst -- always read the same number of bytes as transmitted"""

    BurstTx = 1
    """Transmit burst --- don't return any data read from the bus"""

    BurstRx = 2
    """Receive burst -- ignore transmitted data above 53 bytes long, but receive the full number of bytes specfied"""


class SpiMode:
    """The SPI mode to use"""

    Mode00 = 0x00
    """Clock is initially low; data is valid on the rising edge of the clock"""

    Mode01 = 0x20
    """Clock is initially low; data is valid on the falling edge of the clock"""

    Mode10 = 0x10
    """Clock is initially high; data is valid on the rising edge of the clock"""

    Mode11 = 0x30
    """Clock is initially high; data is valid on the falling edge of the clock"""


class ChipSelectMode:
    """The chip select mode to use"""

    SpiActiveLow = 0x00
    """CS is asserted low, the SPI transaction takes place, and then the signal is returned high"""

    SpiActiveHigh = 0x01
    """CS is asserted high, the SPI transaction takes place, and then the signal is returned low"""

    PulseHighAtBeginning = 0x02
    """CS is pulsed high, and then the SPI transaction takes place"""

    PulseHighAtEnd = 0x03
    """The SPI transaction takes place, and once finished, CS is pulsed high"""

    PulseLowAtBeginning = 0x04
    """CS is pulsed low, and then the SPI transaction takes place"""

    PulseLowAtEnd = 0x05
    """The SPI transaction takes place, and once finished, CS is pulsed low"""


class Spi(ABC):
    """An SPI interface
    
    """

    @property
    @abstractmethod
    def enabled(self):
        """Gets whether this SPI module is enabled"""
        pass

    @enabled.setter
    @abstractmethod
    def enabled(self, value):
        """Sets whether this SPI module is enabled"""
        pass

    @abstractmethod
    def send_receive(self, data_to_write, chip_select=None, chip_select_mode=ChipSelectMode.SpiActiveLow, speed_mhz=1, burst_mode=SpiBurstMode.NoBurst, spi_mode=SpiMode.Mode00):
        """
        Send and receive data through SPI

        :param data_to_write: a list of bytes to send
        :param chip_select: the chip select pin to use
        :param chip_select_mode: the chip select mode to use
        :param speed_mhz: the speed, in Mhz, to clock the data at
        :param burst_mode: the burst mode to use --- if any
        :param spi_mode:
        :return: the data read from the bus
        """
        pass