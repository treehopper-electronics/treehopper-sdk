import threading
from time import sleep
from typing import List

import usb.core
import usb.util

from treehopper.api.device_commands import DeviceCommands
from treehopper.api.i2c import HardwareI2C
from treehopper.api.pin import Pin, SoftPwmManager
from treehopper.api.pwm import HardwarePwm
from treehopper.api.pwm import HardwarePwmManager
from treehopper.api.spi import HardwareSpi
from treehopper.api.uart import HardwareUart
from treehopper.utils.event_handler import EventHandler


class TreehopperUsb:
    """The core class for communicating with Treehopper USB boards.

    ![Treehopper pinout](images/treehopper-hardware.svg)

    Core hardware
    =============
    %Treehopper is a USB 2.0 Full Speed device with 20 \link pin.Pin Pins\endlink — each of which can be used as an
    analog input, digital input, digital output, or soft-PWM output. Many of these pins also have dedicated peripheral
    functions for \link spi.HardwareSpi SPI\endlink, \link i2c.HardwareI2C I2C\endlink,\link uart.HardwareUart
    UART\endlink, and \link pwm.HardwarePwm PWM\endlink.

    You'll access all the pins, peripherals, and board functions through this class, which will automatically create
    all peripheral instances for you.

    Example:
        >>> board = find_boards()[0]        # Get the first board.
        >>> board.connect()                 # Be sure to connect before doing anything else.
        >>> board.led = True                # Turn on the board's LED.
        >>> imu = Mpu9250.probe(board.i2c)  # Connect an MPU9250 9-DoF IMU to the I2C interface on the board.
        >>> print(imu.accelerometer)        # Print the acceleration reading from the sensor.
        '[0.95, 0.01, 0.03]'
        >>> board.disconnect()              # Disconnect from the board when finished.

    Getting a board reference
    -------------------------
    To obtain a reference to the board, use the \link treehopper.api.find_boards.find_boards() find_boards()\endlink
    method:

        >>> board = find_boards()[0]        # Get the first board.

    \warning While you're free to create TreehopperUsb variables that reference boards, do not attempt to create a
    TreehopperUsb instance manually; always obtain references to boards from the \link
    treehopper.api.find_boards.find_boards() find_boards()\endlink function.

    Connect to the board
    --------------------
    Before you use the board, you must explicitly connect to it by calling the connect() method

        >>> board = find_boards()[0]        # Get the first board.
        >>> board.connect()                 # Be sure to connect before
        doing anything else.

    \note Once a board is connected, other applications won't be able to use it.

    On-board LED
    ------------
    The only peripheral directly exposed by this class is the #led property, which will control the LED's state once
    the board is connected. This demo will blink the LED until the board is unplugged:

        >>> board = find_boards()[0]
        >>> board.connect()
        >>> while board.connected:
        >>>     board.led = not board.led  # toggle the LED
        >>>     sleep(0.1)

    Next steps
    ==========
    To learn about accessing different %Treehopper peripherals, visit the doc links to the relevant classes:
    \li \link pin.Pin Pin\endlink
    \li \link spi.HardwareSpi HardwareSpi\endlink
    \li \link i2c.HardwareI2C HardwareI2C\endlink
    \li \link uart.HardwareUart HardwareUart\endlink
    \li \link pwm.HardwarePwm HardwarePwm\endlink
    """

    ## \cond PRIVATE
    def __init__(self, dev: usb.core.Device):
        self._pin_listener_thread = threading.Thread(target=self._pin_listener)
        self._dev = dev
        self._comms_lock = threading.Lock()

        self._pin_update_flag = threading.Event()

        self._pin_report_endpoint = 0x81
        self._peripheral_response_endpoint = 0x82
        self._pin_config_endpoint = 0x01
        self._peripheral_config_endpoint = 0x02

        self._pins = []  # type: List[Pin]

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

        self._spi = HardwareSpi(self)
        self._i2c = HardwareI2C(self)
        self._uart = HardwareUart(self)

        self._pwm1 = HardwarePwm(self.pins[7])
        self._pwm2 = HardwarePwm(self.pins[8])
        self._pwm3 = HardwarePwm(self.pins[9])

        self._hardware_pwm_manager = HardwarePwmManager(self)
        self._soft_pwm_manager = SoftPwmManager(self)

        self._led = False
        self._connected = False
        self._pin_report_received = EventHandler(self)

    ## \endcond
    ## @name Main components
    # @{
    def connect(self):
        """
        Connect to the board.

        Calling this method will connect to the board and start the pin listener update thread. Repeated calls to
        this method are ignored.

        Warning:
            You must connect to the board before performing any operations (other than querying the name() or
            serial_number() of the board).

        Returns:
            None

        """
        if self._connected:
            return

        self._dev.set_configuration()
        self._connected = True
        self._pin_listener_thread.start()

    @property
    def pins(self):
        """Gets a list of \link pin.Pin pins\endlink that belong to this board"""
        return self._pins

    @property
    def spi(self):
        """Gets the \link spi.HardwareSpi SPI\endlink peripheral that belongs to this board"""
        return self._spi

    @property
    def i2c(self):
        """Gets the \link i2c.HardwareI2C I2C\endlink peripheral that belongs to this board"""
        return self._i2c

    @property
    def uart(self):
        """Gets the \link uart.HardwareUart UART\endlink peripheral that belongs to this board"""
        return self._uart

    @property
    def pwm1(self):
        """Gets the \link pwm.HardwarePwm PWM1\endlink module that belongs to this board"""
        return self._pwm1

    @property
    def pwm2(self):
        """Gets the \link pwm.HardwarePwm PWM2\endlink module that belongs to this board"""
        return self._pwm3

    @property
    def pwm3(self):
        """Gets the \link pwm.HardwarePwm PWM3\endlink module that belongs to this board"""
        return self._pwm3

    @property
    def led(self) -> bool:
        """
        Gets or sets the state of the LED.

        Example:
            >>> while board.connected:
            >>>     board.led = not board.led  # toggle the LED
            >>>     sleep(0.1)

        Returns:
            bool: The current state of the LED.

        """
        return self._led

    @led.setter
    def led(self, val: bool) -> None:
        self._led = val
        data = [DeviceCommands.LedConfig, self._led]
        self._send_peripheral_config_packet(data)

    def disconnect(self):
        """Disconnect from the board.

        This method disconnects from the board and stops the pin listener update thread. Repeated calls to this
        method are ignored."""
        if not self._connected:
            return

        self._connected = False
        self._pin_listener_thread.join()
        usb.util.dispose_resources(self._dev)

    @property
    def connected(self):
        """
        Gets whether the board is connected.

        Returns:
            bool: Whether the board is connected

        Note:
            The board will automatically disconnect if an unrecoverable error is encountered (which includes a
            disconnect), so this is a useful property to consult to determine if a board is physically attached to a
            computer.
        """
        return self._connected

    ## @}
    ## @name Board identity & firmware management
    # @{
    @property
    def serial_number(self):
        """
        Gets the serial number of the board.

        This property is available to read even without connecting to the board. If you wish to change the serial
        number,
        use update_serial_number().

        Note:
            While you do not need to connect() to the board before querying its serial number, you will not be able to
            retrieve the serial number of a board to which another application is connected.

            Treehopper's Python API doesn't currently support directly querying the OS for device info, so while
            executing \link find_boards.find_boards() find_boards()\endlink, the API implicitly connects to the board,
            queries the string descriptor, and disconnects. Since concurrent operation isn't supported, this operation
            will error if the board is already open.

        Returns:
            str: The serial number.

        """
        return self._dev.serial_number

    @property
    def name(self):
        """
        Gets the device name of the board.

        This property is available to read even without connecting to the board. If you wish to change the device name,
        use update_device_name().

        Note:
            While you do not need to connect() to the board before querying its device name, you will not be able to
            retrieve the device name of a board to which another application is connected.

            Treehopper's Python API doesn't currently support directly querying the OS for device info, so while
            executing \link find_boards.find_boards() find_boards()\endlink, the API implicitly connects to the board,
            queries the string descriptor, and disconnects. Since concurrent operation isn't supported, this operation
            will error if the board is already open.

        Returns:
            str: The device name.

        """
        return self._dev.product

    def update_serial_number(self, serial_number: str):
        """
        Update the serial number on the device.

        While the new serial number is immediately available from the SerialNumber property, the changes will not take
        effect in other applications until the device is reset. This can be done by calling reboot().

        Args:
            serial_number: a 60-character (or fewer) string containing the new serial number (str).

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

        While the new serial number is immediately available from the SerialNumber property, the changes will not take
        effect in other applications until the device is reset. This can be done by calling reboot().

        Note:
            Microsoft Windows caches the device name in the registry when it is first attached to the board, so even if
            you change the device name with this function, you won't see the new device name --- even after replugging
            the board. To force Windows to update theregistry with the new name, make sure to change the serial number,
            too (see update_serial_number()).

        Args:
            device_name: a 60-character (or fewer) string containing the new device name (str).

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

    ## @}
    ## @name Other components
    # @{
    def reboot(self):
        """
        Reboots the board.

        Calling this method will automatically call the disconnect() method, and no further communication will be
        possible until the board is reopened.

        Returns:
            None
        """
        self._send_peripheral_config_packet([DeviceCommands.Reboot])
        self.disconnect()

    def reboot_into_bootloader(self):
        """
        Reboots the board into bootloader mode.

        Calling this method will automatically call the disconnect() method, and no further communication will be
        possible. You can load new firmware onto the board once in bootloader mode, or if you wish to return to normal
        operation, replug the board to reset it.

        Returns:
            None
        """
        self._send_peripheral_config_packet([DeviceCommands.EnterBootloader])
        self.disconnect()

    def await_pin_update(self):
        """ Returns when the board has received a new pin update """
        self._pin_update_flag.clear()
        self._pin_update_flag.wait()

    @property
    def hardware_pwm_manager(self):
        """Gets the hardware PWM manager for this board"""
        return self._hardware_pwm_manager

    ## @}
    def _pin_listener(self):
        while self._connected:
            try:
                data = self._dev.read(self._pin_report_endpoint, 41, 1000)
                i = 1
                if data[0] == 0x02:
                    for pin in self.pins:
                        pin._update_value(data[i], data[i + 1])
                        i += 2
                    self._pin_update_flag.set()
            except usb.USBError as e:
                if e.errno == 10060:  # timeout win32
                    pass
                elif e.errno == 110:  # timeout libusb
                    pass
                elif e.errno == 60:  # timeout macos
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
