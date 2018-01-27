import logging
from typing import List, TYPE_CHECKING
if TYPE_CHECKING:
    from treehopper.api import TreehopperUsb
from treehopper.api import DeviceCommands
from treehopper.api.IOneWire import IOneWire
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


class HardwareUart(IOneWire):
    """
    Hardware UART module
    """

    def __init__(self, board: 'TreehopperUsb'):
        """
        Construct a new Hardware UART connected to the specified board.
        @param board: the board the hardware UART is attached to

        This constructor should not be called by the user; you can obtain a reference to the hardware UART
        on your board by accessing the board's "uart" property. See the documentation for TreehopperUsb for more info.
        """
        self._board = board
        self._baud = 9600
        self._enabled = False
        self._mode = UartMode.Uart
        self._use_open_drain_tx = False
        self._logging = logging.getLogger(__name__)

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
        """[Property] Gets or sets the mode of the device."""
        return self._mode

    @mode.setter
    def mode(self, value):
        """[Property] Gets or sets the mode of the device."""
        if self._mode == value:
            return

        self._mode = value
        self._update_config()

    @property
    def enabled(self):
        """[Property] Gets or sets whether the device is enabled."""
        return self._enabled

    @enabled.setter
    def enabled(self, value):
        """[Property] Gets or sets whether the device is enabled."""
        if self._enabled == value:
            return

        self._enabled = value
        self._update_config()

    @property
    def baud(self):
        """[Property] Gets or sets the baud of the UART."""
        return self._baud

    @baud.setter
    def baud(self, value):
        """[Property] Gets or sets the baud of the UART."""
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
            
            data = [DeviceCommands.UartConfig, UartConfig.Standard, round(timer_val), use_prescaler, self._use_open_drain_tx]
            self._board._send_peripheral_config_packet(data)
        else:
            data =[DeviceCommands.UartConfig, UartConfig.OneWire]
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
