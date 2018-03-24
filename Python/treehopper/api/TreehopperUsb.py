from time import sleep
from typing import List

import usb.core
import usb.util
import threading

from treehopper.api.HardwarePwmManager import HardwarePwmManager
from treehopper.api.HardwareSpi import HardwareSpi
from treehopper.api.HardwareUart import HardwareUart
from treehopper.utils.EventHandler import EventHandler
from treehopper.api.Pin import Pin
from treehopper.api.HardwarePwm import HardwarePwm
from treehopper.api.HardwareI2c import HardwareI2c
from treehopper.api.DeviceCommands import DeviceCommands
from treehopper.api.SoftPwmManager import SoftPwmManager


class TreehopperUsb:
    """
    The core class for communicating with Treehopper USB boards.

    This class represents a Treehopper board. You'll access all the pins, peripherals, and board functions through
    this object, which will automatically create all peripheral instances for you.

    Warning:
        Do not attempt to create a TreehopperUsb instance manually; always obtain references to boards from the
        treehopper.api.find_boards() function.

    Examples:
        >>> board = find_boards()[0]        # Get the first board.
        >>> board.connect()                 # Be sure to connect before doing anything else.
        >>> board.led = True                # Turn on the board's LED.
        >>> imu = Mpu9250.probe(board.i2c)  # Connect an MPU9250 9-DoF IMU to the I2C interface on the board.
        >>> print(imu.accelerometer)        # Print the acceleration reading from the sensor.
        '[0.95, 0.01, 0.03]'
        >>> board.disconnect()              # Disconnect from the board when finished.

    Attributes:
        pins (list[DigitalPin]): Collection of pins that belong to this board (List[Pin.Pin]).
        spi (HardwareSpi): The SPI peripheral (HardwareSpi.HardwareSpi).
        i2c (HardwareI2c): The I2C peripheral (HardwareI2c.HardwareI2c).
        uart (HardwareUart): The UART peripheral (HardwareUart.HardwareUart).
        pwm1 (HardwarePwm): The PWM1 peripheral (HardwarePwm.HardwarePwm).
        pwm2 (HardwarePwm): The PWM2 peripheral (HardwarePwm.HardwarePwm).
        pwm3 (HardwarePwm): The PWM3 peripheral (HardwarePwm.HardwarePwm).
        hardware_pwm_manager (HardwarePwmManager): The hardware PWM manager (HardwarePwmManager.HardwarePwmManager).

    """
    def __init__(self, dev: usb.core.Device):
        self._pin_listener_thread = threading.Thread(target=self._pin_listener)
        self._dev = dev
        self._comms_lock = threading.Lock()

        self._pin_report_endpoint = 0x81
        self._peripheral_response_endpoint = 0x82
        self._pin_config_endpoint = 0x01
        self._peripheral_config_endpoint = 0x02

        self.pins = []  # type: List[Pin]


        for i in range(20):
            self.pins.append(Pin(self, i))

        self.pins[0].name = "Pin 0 (SCK)"
        self.pins[1].name = "Pin 1 (MISO)"
        self.pins[2].name = "Pin 2 (MOSI)"
        self.pins[3].name = "Pin 3 (SDA)"
        self.pins[4].name = "Pin 4 (SCL)"
        self.pins[5].name = "Pin 5 (TX)"
        self.pins[6].name = "Pin 6 (RX)"
        self.pins[7].name = "Pin 7 (PWM1)"
        self.pins[8].name = "Pin 8 (PWM2)"
        self.pins[9].name = "Pin 9 (PWM3)"

        self.spi = HardwareSpi(self)
        self.i2c = HardwareI2c(self)
        self.uart = HardwareUart(self)

        self.pwm1 = HardwarePwm(self.pins[7])
        self.pwm2 = HardwarePwm(self.pins[8])
        self.pwm3 = HardwarePwm(self.pins[9])

        self.hardware_pwm_manager = HardwarePwmManager(self)
        self._soft_pwm_manager = SoftPwmManager(self)


        self._led = False
        self._connected = False
        self._pin_report_received = EventHandler(self)

    def connect(self):
        """
        Connect to the board.

        Calling this method will connect to the board and start the pin listener update thread. Repeated calls
        to this method are ignored.

        Warning:
            You must connect to the board before performing any operations (other than querying the name()
            or serial_number() of the board).

        Returns:
            None

        """
        if self._connected:
            return

        self._dev.set_configuration()
        self._connected = True
        self._pin_listener_thread.start()

    def disconnect(self):
        """Disconnect from the board.

        This method disconnects from the board and stops the pin listener update thread. Repeated calls to this
        method are ignored."""
        if not self._connected:
            return

        self._connected = False
        self._pin_listener_thread.join()
        usb.util.dispose_resources(self._dev)

    def reboot(self):
        """
        Reboots the board.

        Calling this method will automatically call the disconnect() method, and no further communication will
        be possible until the board is reopened.

        Returns:
            None
        """
        self._send_peripheral_config_packet([DeviceCommands.Reboot])
        self.disconnect()

    def reboot_into_bootloader(self):
        """
        Reboots the board into bootloader mode.

        Calling this method will automatically call the disconnect() method, and no further communication will
        be possible. You can load new firmware onto the board once in bootloader mode, or if you wish to
        return to normal operation, replug the board to reset it.

        Returns:
            None
        """
        self._send_peripheral_config_packet([DeviceCommands.EnterBootloader])
        self.disconnect()

    def update_serial_number(self, serial_number: str):
        """
        Update the serial number on the device.

        While the new serial number is immediately available from the SerialNumber property, the changes
        will not take effect in other applications until the device is reset. This can be done by calling
        reboot().

        Args:
            serial_number (str): a 60-character (or fewer) string containing the new serial number

        Returns:
            None

        Examples:
        >>> board = find_boards()[0]                 # Get the first board.
        >>> board.connect()                          # Be sure to connect before doing anything else.
        >>> board.update_serial_number("a3bY392")    # Change the serial number.
        >>> board.reboot()                           # Reboot the board so the OS picks up the changes.
        """
        length = len(serial_number)
        if length > 60:
            raise RuntimeError("String must be 60 characters or less")

        data_to_send = [DeviceCommands.FirmwareUpdateSerial, length] + list(serial_number.encode())
        self._send_peripheral_config_packet(data_to_send)
        sleep(0.1)

    def update_device_name(self, device_name: str):
        """
        Update the device name on the device.

        While the new serial number is immediately available from the SerialNumber property, the changes
        will not take effect in other applications until the device is reset. This can be done by calling
        reboot().

        Note:
            Microsoft Windows caches the device name in the registry when it is first attached to the board, so even
            if you change the device name with this function, you won't see the new device name --- even after
            replugging the board. To force Windows to update the registry with the new name, make sure to change the
            serial number, too (see update_serial_number()).

        Args:
            device_name (str): a 60-character (or fewer) string containing the new device name.

        Returns:
            None

        Examples:
        >>> board = find_boards()[0]                 # Get the first board.
        >>> board.connect()                          # Be sure to connect before doing anything else.
        >>> board.update_device_name("Acme Widget")  # Update the device name.
        >>> board.update_serial_number("a3bY392")    # Change the serial number to force Windows refresh.
        >>> board.reboot()                           # Reboot the board so the OS picks up the changes.

        """
        length = len(device_name)
        if length > 60:
            raise RuntimeError("String must be 60 characters or less")

        data_to_send = [DeviceCommands.FirmwareUpdateName, length] + list(device_name.encode())
        self._send_peripheral_config_packet(data_to_send)
        sleep(0.1)

    @property
    def serial_number(self):
        """
        [Property] Gets the serial number of the board.

        This property is available to read even without connecting to the board. If you wish to change the serial number,
        use update_serial_number().

        Note:
            While you do not need to connect() to the board before querying its serial number, you will not be able
            to retrieve the serial number of a board to which another application is connected.

            Treehopper's Python USB backend calls into LibUSB, which doesn't support querying the OS for device info,
            so LibUSB implicitly connects to the board, queries the string descriptor, and disconnects. Since
            concurrent operation isn't supported by LibUSB, this operation will error if the board is already open.

        Returns:
            str: The serial number.

        """
        return self._dev.serial_number

    @property
    def name(self):
        """
        [Property] Gets the device name of the board.

        This property is available to read even without connecting to the board. If you wish to change the device name,
        use update_device_name().

        Note:
            While you do not need to connect() to the board before querying its device name, you will not be able
            to retrieve the device name of a board to which another application is connected.

            Treehopper's Python USB backend calls into LibUSB, which doesn't support querying the OS for device info,
            so LibUSB implicitly connects to the board, queries the string descriptor, and disconnects. Since
            concurrent operation isn't supported by LibUSB, this operation will error if the board is already open.

        Returns:
            str: The device name.

        """
        return self._dev.product

    def _pin_listener(self):
        while self._connected:
            try:
                data = self._dev.read(self._pin_report_endpoint, 41, 1000)
                i = 1
                if data[0] == 0x02:
                    for pin in self.pins:
                        pin._update_value(data[i], data[i+1])
                        i += 2
            except usb.USBError as e:
                if e.errno == 10060:  # timeout win32
                    pass
                elif e.errno == 110:  # timeout libusb
                    pass
                elif e.errno == 60:   # timeout macos
                    pass
                else:
                    self._connected = False
        return

    def _send_pin_config(self, data: List[int]):
        try:
            self._dev.write(self._pin_config_endpoint, data)
        except usb.USBError:
            self._connected = False

    def _send_peripheral_config_packet(self, data: List[int]):
        try:
            self._dev.write(self._peripheral_config_endpoint, data)
        except usb.USBError:
            self._connected = False

    def _receive_comms_response_packet(self, num_bytes_to_read: int) -> List[int]:
        try:
            return self._dev.read(self._peripheral_response_endpoint, num_bytes_to_read)
        except usb.USBError:
            self._connected = False
            return [0] * num_bytes_to_read

    @property
    def led(self) -> bool:
        """
        [Property] Gets the state of the LED.

        Returns:
            bool: The current state of the LED.

        """
        return self._led

    @led.setter
    def led(self, val: bool) -> None:
        """
        [Property] Sets the state of the LED.

        Args:
            val (bool): The new state of the LED.

        Returns:
            None
        """
        self._led = val
        data = [DeviceCommands.LedConfig, self._led]
        self._send_peripheral_config_packet(data)

    @property
    def connected(self):
        """
        [Property] Gets whether the board is connected.

        Returns:
            bool: Whether the board is connected

        Note:
            The board will automatically disconnect if an unrecoverable error is encountered (which includes a disconnect),
            so this is a useful property to consult to determine if a board is physically attached to a computer.
        """
        return self._connected
