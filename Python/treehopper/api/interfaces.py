from abc import ABC, abstractmethod
from typing import List

from treehopper.utils.event_handler import EventHandler


class Pwm(ABC):
    """Represents a generic PWM pin"""

    @property
    @abstractmethod
    def duty_cycle(self):
        """Gets or sets the duty cycle of the PWM pin, from 0.0-1.0."""
        pass

    @duty_cycle.setter
    @abstractmethod
    def duty_cycle(self, value):
        pass

    @property
    @abstractmethod
    def pulse_width(self):
        """Gets or sets the pulse width, in ms, of the pin."""
        pass

    @pulse_width.setter
    @abstractmethod
    def pulse_width(self, value):
        pass

    @abstractmethod
    def enable_pwm(self):
        """Enable the PWM functionality of this pin."""
        pass


class DigitalBase:
    def __init__(self):
        self._digital_value = False


class DigitalIn(DigitalBase):
    def __init__(self):
        super().__init__()
        self.digital_value_changed = EventHandler(self)

    @property
    def digital_value(self) -> bool:
        return self._digital_value

    @abstractmethod
    def make_digital_in(self):
        pass

    def _update_value(self, value: bool):
        if value == self._digital_value:
            return

        self._digital_value = value
        self.digital_value_changed(value)


class DigitalOut(DigitalBase):
    def __init__(self):
        super().__init__()

    @property
    def digital_value(self) -> bool:
        return self._digital_value

    @digital_value.setter
    @abstractmethod
    def digital_value(self, value):
        pass

    @abstractmethod
    def make_digital_push_pull_out(self):
        pass


class AdcPin:
    """Represents an ADC pin

    Attributes:
        analog_voltage_changed: (\link treehopper.utils.event_handler.EventHandler EventHandler\endlink) an event that fires with new voltages received
        analog_value_changed: (\link treehopper.utils.event_handler.EventHandler EventHandler\endlink) an event that fires with new value received
        adc_value_changed: (\link treehopper.utils.event_handler.EventHandler EventHandler\endlink) an event that fires with new ADC value received
        analog_voltage_threshold: (float) the threshold for analog_voltage_changed events
        analog_value_threshold: (float) the threshold for analog_value_changed events
        adc_value_threshold: (float) the threshold for adc_value_changed events
    """

    def __init__(self, bit_depth, max_voltage):
        self._bit_depth = bit_depth
        self._adc_value = 0
        self._old_adc_value = 0
        self._old_analog_value = 0
        self._old_analog_voltage = 0
        self.max_voltage = max_voltage  # type: float
        self.analog_voltage_changed = EventHandler(self)  # type: EventHandler
        self.analog_value_changed = EventHandler(self)  # type: EventHandler
        self.adc_value_changed = EventHandler(self)  # type: EventHandler
        self.adc_value_threshold = 4  # type: int
        self.analog_value_threshold = 0.05  # type: float
        self.analog_voltage_threshold = 0.1  # type: float

    @property
    def adc_value(self) -> int:
        """Immediately returns the last ADC value (0-4095) captured from the pin

        Returns:
            (int) the last ADC value
        """
        return self._adc_value

    @property
    def analog_voltage(self) -> float:
        """Immediately returns the last analog voltage (0.0 - 3.3V) captured from the pin

        Returns:
            (float) the last analog voltage value

        """
        return self.analog_value * self.max_voltage

    @property
    def analog_value(self) -> float:
        """Immediately returns the uniform last analog value (0.0 - 1.0) captured from the pin

        Returns:
            (float) the last analog value value
        """
        return self._adc_value / ((1 << self._bit_depth) - 1)

    def _update_value(self, adc_value):
        # self._old_analog_voltage = self.analog_voltage
        # self._old_analog_value = self.analog_value
        self._adc_value = adc_value

        if abs(self.analog_voltage - self._old_analog_voltage) > self.analog_voltage_threshold:
            self._old_analog_voltage = self.analog_voltage
            self.analog_voltage_changed(self.analog_voltage)

        if abs(self.analog_value - self._old_analog_value) > self.analog_value_threshold:
            self._old_analog_value = self.analog_value
            self.analog_value_changed(self.analog_value)

        if abs(adc_value - self._old_adc_value) > self.adc_value_threshold:
            self._old_adc_value = self.adc_value
            self.adc_value_changed(adc_value)


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
    def send_receive(self, data_to_write: List[int], chip_select=None, chip_select_mode=ChipSelectMode.SpiActiveLow,
                     speed_mhz=6, burst_mode=SpiBurstMode.NoBurst, spi_mode=SpiMode.Mode00) -> List[int]:
        """
        Send and receive data through SPI

        Args:
            data_to_write: a list of data to send. The length of the transaction is determined
            by the length of this list (list[int]).
            chip_select: The chip select pin, if any, to use during this transaction (\link treehopper.api.pin.Pin
            Pin\endlink).
            chip_select_mode: if a CS pin is selected, this sets the chip select mode to use during this transaction
            (select from \link interfaces.ChipSelectMode ChipSelectMode\endlink).
            speed_mhz: The speed to perform this transaction at (float, default: 6.0).
            burst_mode: The burst mode to use (select from \link interfaces.SpiBurstMode SpiBurstMode\endlink
            values, default: \link interfaces.SpiBurstMode.NoBurst SpiBurstMode.NoBurst\endlink).
            spi_mode: The SPI mode to use during this transaction (select from \link interfaces.SpiMode
            SpiMode\endlink values, default: \link interfaces.SpiMode.Mode00 SpiMode.Mode00\endlink).

        Returns:
            A byte array with the received data (list[int])
        """
        pass


class I2C(ABC):
    @property
    @abstractmethod
    def speed(self) -> float:
        """Gets or sets the speed, in kHz, of the i2C peripheral"""
        pass

    @speed.setter
    @abstractmethod
    def speed(self, value: float):
        pass

    @property
    @abstractmethod
    def enabled(self) -> bool:
        """Gets or sets whether the i2c peripheral is enabled"""
        pass

    @enabled.setter
    @abstractmethod
    def enabled(self, value: bool):
        pass

    @abstractmethod
    def send_receive(self, address: int, write_data: List[int], num_bytes_to_read: int) -> List[int]:
        """
        Send and receive data between Treehopper and an I2C peripheral.

        This function writes write_data to the I2C peripheral at the specified address,
        and reads back num_bytes_to_read.

        Args:
            address: The 7-bit slave address to address (int).
            write_data: A list of data to write to the slave (list[int], default: None).
            num_bytes_to_read: The number of bytes to read after the write operation (int, default: 0).

        Returns:
            A list of data received.

        """
        pass


class Uart(ABC):
    def __init__(self):
        super().__init__()

    @abstractmethod
    def receive(self):
        """
        Receive characters from the UART

        Returns:
             the received characters.

        Calling this function will return the current UART receive FIFO and clear it. The FIFO supports a maximum of 32
        characters.
        """
        pass

    @abstractmethod
    def send(self, data_to_send: List[int]):
        """
        Send data out of the UART

        Args:
            data_to_send: (list[int]) the data to send

        """
        pass


class OneWire(ABC):
    """
    Abstract base class representing a 1-Wire interface.
    """

    def __init__(self):
        super().__init__()

    @abstractmethod
    def start_one_wire(self):
        """
        Place the UART into One-Wire Mode and enable the peripheral.
        """
        pass

    @abstractmethod
    def one_wire_reset_and_match_address(self, address: int):
        """
        Reset the bus and address the device specified

        Args:
            address: (int) the device to address

        """
        pass

    @abstractmethod
    def one_wire_search(self) -> List[int]:
        """
        Search the One-Wire Bus for devices.

        Returns:
            (list[int]) a list of addresses of devices attached to the bus.

        """
        pass

    @abstractmethod
    def one_wire_reset(self) -> bool:
        """
        Reset the 1-Wire bus.

        Returns:
            whether a device was found on the bus or not.

        """
        pass

    @abstractmethod
    def receive(self, one_wire_num_bytes) -> List[int]:
        """
        Receive characters from the UART

        Calling this function will return the current UART receive FIFO and clear it. The FIFO supports a maximum of 32
        characters.

        Args:
            one_wire_num_bytes: (int) in OneWire mode, specifies the number of characters to read. Ignored in standard
        UART mode.

        Returns:
            the received characters.
        """
        pass

    @abstractmethod
    def send(self, data_to_send: List[int]):
        """
        Send data out of the UART

        Args:
            data_to_send: (list[int]) the data to send
        """
        pass
