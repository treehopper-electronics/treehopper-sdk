import logging
from typing import List, TYPE_CHECKING

if TYPE_CHECKING:
    from treehopper.api.treehopper_usb import TreehopperUsb
from treehopper.api.device_commands import DeviceCommands
from treehopper.api.interfaces import OneWire, Uart
from treehopper.utils.error import error


class UartMode:
    Uart = 0
    """The UART is in standard UART mode."""

    OneWire = 1
    """The UART is in OneWire mode"""


class UartConfig:
    Disabled, Standard, OneWire = range(3)


class UartCommand:
    Transmit, Receive, OneWireReset, OneWireScan = range(4)


class HardwareUart(Uart, OneWire):
    """
    Built-in UART peripheral

    Background
    ==========
    The UART peripheral allows you to send and receive standard-format RS-232-style asynchronous serial communications.

    Pins
    ----
    When the UART is enabled, the following pins will be unavailable for other use:
    - <b>TX</b> <i>(Transmit)</i>: This pin carries data from Treehopper to the device you've attached to the UART.
    - <b>RX</b> <i>(Receive)</i>: This pin carries data from the device to Treehopper.

    Note that UART cross-over is a common problem when people are attaching devices together; always consult the
    documentation for the device you're attaching to Treehopper to ensure that the TX signal from Treehopper is
    flowing into the receive input (RX, DIN, etc) of the device, and vice-versa. Since you are unlikely to damage
    either device by incorrectly connecting the TX and RX pins, it is a common troubleshooting practice to simply swap
    TX and RX if the system doesn't appear to be functioning properly.

    One-wire mode
    -------------
    Treehopper's UART has built-in support for One-Wire mode with few external circuitry requirements. When you use
    the UART in One-Wire mode, the TX pin will switch to an open-drain mode. You must physically tie the RX and TX
    pins together --- this is the data pin for the One-Wire bus. Most One-Wire sensors and devices you use will
    require an external pull-up resistor on the bus.

    Implementation details
    ----------------------
    Treehopper's UART is designed for average baud rates; the range of supported rates is 7813 baud to 2.4 Mbaud,
    though communication will be less reliable above 1-2 Mbaud.

    Transmitting data is straightforward: simply pass a list of data --- up to 63 elements long --- to the Send()
    function once the UART is enabled.

    Receiving data is more challenging, since incoming data can appear on the RX pin at any moment when the UART is
    enabled. Since all actions on Treehopper are initiated on the host, to get around UART's inherent asynchronicity,
    a 32-byte buffer holds any received data that comes in while the UART is enabled. Then, when the host wants to
    access this data, it can Receive() it from the board to obtain the buffer.

    Whenever Receive() is called, the entire buffer is sent to the host, and the buffer's pointer is reset to 0 (
    i.e., the buffer is reset). This can be useful for clearing out any gibberish and returning the UART to a known
    state before you expect to receive data --- for example, if you're addressing a device that you send commands to,
    and read responses back from, you may wish to call Receive() before sending the command; that way, parsing the
    received data will be simpler.

    Other considerations
    --------------------
    This ping-pong short-packet-oriented back-and-forth scenario is what Treehopper's UART is built for,
    as it's what's most commonly needed when interfacing with embedded devices that use a UART.

    There is a tight window of possible baud rates where it is plausible to receive data continuously without
    interruption. For example, at 9600 baud, the Receive() function only need to finish execution every 33
    milliseconds, which can easily be accomplished in most operating systems. However, because data is not
    double-buffered on the board, under improbable circumstances, continuously-transmitted data may inadvertently be
    discarded.

    Treehopper's UART is not designed to replace a high-quality CDC-class USB-to-serial converter, especially for
    high data-rate applications. In addition to streaming large volumes of data continuously, USB CDC-class UARTs
    should also offer lower latency for receiving data. Treehopper also has no way of exposing its UART to the
    operating system as a COM port, so it's most certainly not a suitable replacement for a USB-to-serial converter
    in most applications.

    """

    ## \cond PRIVATE
    def __init__(self, board: 'TreehopperUsb'):
        super().__init__()
        self._board = board
        self._baud = 9600
        self._enabled = False
        self._mode = UartMode.Uart
        self._use_open_drain_tx = False
        self._logging = logging.getLogger(__name__)

    ## \endcond
    def start_one_wire(self):
        """
        Place the UART into One-Wire Mode and enable the peripheral.
        """
        self.mode = UartMode.OneWire
        self.enabled = True

    def one_wire_reset_and_match_address(self, address: int):
        """
        Reset the bus and address the device specified
        @param address: the device to address
        """
        self.one_wire_reset()
        data = [0x55] + list(address.to_bytes(8, 'big'))
        self.send(data)

    def one_wire_search(self) -> List[int]:
        """
        Search the 1-Wire Bus for devices.
        @return: a list of addresses of devices attached to the bus.
        """
        self.mode = UartMode.OneWire
        self.enabled = True

        ret_val = []  # list[int]

        data = [DeviceCommands.UartTransaction, UartCommand.OneWireScan]
        with self._board._comms_lock:
            self._board._send_peripheral_config_packet(data)
            while True:
                received_data = self._board._receive_comms_response_packet(9)
                if received_data[0] == 0xff:
                    break

                ret_val.append(int.from_bytes(received_data[1:], 'big'))
        return ret_val

    def one_wire_reset(self) -> bool:
        """
        Reset the 1-Wire bus.
        @return: whether a device was found on the bus or not.
        """
        self.mode = UartMode.OneWire
        self.enabled = True
        ret_val = False
        data = [DeviceCommands.UartTransaction, UartCommand.OneWireReset]
        with self._board._comms_lock:
            self._board._send_peripheral_config_packet(data)
            received_data = self._board._receive_comms_response_packet(1)
            ret_val = received_data[0] > 0

        return ret_val

    def receive(self, one_wire_num_bytes=0) -> List[int]:
        """
        Receive characters from the UART
        @param one_wire_num_bytes: in OneWire mode, specifies the number of characters to read. Ignored in standard
        UART mode.
        @return: the received characters.

        Calling this function will return the current UART receive FIFO and clear it. The FIFO supports a maximum of 32
        characters.
        """
        ret_val = []  # List[int]
        if self._mode == UartMode.Uart:
            if one_wire_num_bytes != 0:
                self._logging.info("Since the UART is not in One-Wire Mode, the oneWireNumBytes parameter is ignored")

            data = [DeviceCommands.UartTransaction, UartCommand.Receive]

            with self._board._comms_lock:
                self._board._send_peripheral_config_packet(data)
                received_data = self._board._receive_comms_response_packet(33)
                len = received_data[32]
                ret_val = received_data[0:len]

        else:
            if one_wire_num_bytes == 0:
                error(logging, "You must specify the number of bytes to receive in One-Wire Mode", False)
                return ret_val

            data = [DeviceCommands.UartTransaction, UartCommand.Receive, one_wire_num_bytes]

            with self._board._comms_lock:
                self._board._send_peripheral_config_packet(data)
                received_data = self._board._receive_comms_response_packet(33)
                len = received_data[32]
                ret_val = received_data[0:len]
        return ret_val

    def send(self, data_to_send: List[int]):
        """
        Send data out of the UART
        @param data_to_send: the data to send
        """
        if len(data_to_send) > 63:
            error(logging, "The maximum UART length for one transaction is 63 bytes", True)
        data = [DeviceCommands.UartTransaction, UartCommand.Transmit, len(data_to_send)] + data_to_send
        with self._board._comms_lock:
            self._board._send_peripheral_config_packet(data)
            self._board._receive_comms_response_packet(1)

    @property
    def mode(self):
        """Gets or sets the mode of the device."""
        return self._mode

    @mode.setter
    def mode(self, value):
        """Gets or sets the mode of the device."""
        if self._mode == value:
            return

        self._mode = value
        self._update_config()

    @property
    def enabled(self):
        """[Gets or sets whether the device is enabled."""
        return self._enabled

    @enabled.setter
    def enabled(self, value):
        """Gets or sets whether the device is enabled."""
        if self._enabled == value:
            return

        self._enabled = value
        self._update_config()

    @property
    def baud(self):
        """Gets or sets the baud of the UART."""
        return self._baud

    @baud.setter
    def baud(self, value):
        """Gets or sets the baud of the UART."""
        if self._baud == value:
            self._baud = value

        self._update_config()

    def _update_config(self):
        if not self._enabled:
            self._board._send_peripheral_config_packet([DeviceCommands.UartConfig, UartConfig.Disabled])
        elif self._mode == UartMode.Uart:
            timer_val = 0
            use_prescaler = False

            timer_val_prescaler = round(256.0 - 2000000.0 / self._baud)
            timer_val_no_prescaler = round(256.0 - 24000000.0 / self._baud)

            prescaler_out_of_bounds = (timer_val_prescaler > 255) or timer_val_prescaler < 0
            no_prescaler_out_of_bounds = (timer_val_no_prescaler > 255) or (timer_val_no_prescaler < 0)

            prescaler_error = round(self._baud - 2000000 / (256 - timer_val_prescaler))
            no_prescaler_error = round(self._baud - 24000000 / (256 - timer_val_no_prescaler))

            if prescaler_out_of_bounds and no_prescaler_out_of_bounds:
                error(self._logging, "The specified baud rate is out of bounds.", False)

            if prescaler_out_of_bounds:
                use_prescaler = False
                timer_val = timer_val_no_prescaler
            elif no_prescaler_out_of_bounds:
                use_prescaler = True
                timer_val = timer_val_prescaler
            elif prescaler_error > no_prescaler_error:
                use_prescaler = False
                timer_val = timer_val_no_prescaler
            else:
                use_prescaler = True
                timer_val = timer_val_no_prescaler

            data = [DeviceCommands.UartConfig, UartConfig.Standard, round(timer_val), use_prescaler,
                    self._use_open_drain_tx]
            self._board._send_peripheral_config_packet(data)
        else:
            data = [DeviceCommands.UartConfig, UartConfig.OneWire]
            self._board._send_peripheral_config_packet(data)

    def __str__(self):
        """
        Get a string representation of the UART's state
        :return: a string representation of the state
        """
        if self._enabled:
            if self._mode == UartMode.Uart:
                return "Uart, running at {} baud".format(self._baud)
            else:
                return "1-Wire"
        else:
            return "Disabled"
