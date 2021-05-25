import logging
import math

from treehopper.api.device_commands import DeviceCommands
from treehopper.api.interfaces import I2C
from treehopper.api.pin import PinMode
from treehopper.utils.utils import *


class I2CTransferError:
    """An enumeration of possible I2C transfer errors"""
    ArbitrationLostError, NackError, UnknownError, TxunderError = range(4)
    Success = 255

    def ErrorString(val):
        """Gets a string representation of an I2CTransferError value"""
        if val == I2CTransferError.ArbitrationLostError:
            return "Arbitration lost"
        elif val == I2CTransferError.NackError:
            return "Nack error"
        elif val == I2CTransferError.UnknownError:
            return "Unknown error"
        elif val == I2CTransferError.TxunderError:
            return "TX underrun"
        elif val == I2CTransferError.Success:
            return "Success"


class HardwareI2C(I2C):
    """
    Built-in I<sup>2</sup>C module

    Quick Start
    ===========
    Once you've connected to your board, you can enable the %I2C peripheral by settings its \link HardwareI2C.enabled
    enabled\endlink property to true, and then send/receive data.


        >>> board = find_boards()
        >>> board.connect()

        >>> board.i2c.enabled = True
        >>> # send 0x31 to device with address 0x17, and read two bytes back
        >>> write_data = 0x31  # the register to read from
        >>> address = 0x17  # 7-bit address
        >>> read_data = board.i2c.send_receive(address, write_data, 2)


    If you want to change the communication rate from the default 100 kHz, consult the #speed property.

    Note that Treehopper.Libraries contains \link treehopper.libraries.smbus_device.SMBusDevice SMBusDevice\endlink,
    a useful class for reading and writing device registers. Almost all %I2C drivers in %Treehopper.Libraries use it.

    Speaking of which, before writing a driver yourself, check to make it's not already in Treehopper.Libraries. You
    may save yourself some time!

    Background
    ==========
    I<sup>2</sup>C (%I2C, or IIC) is a low-speed synchronous serial protocol that allows up to 127 ICs to communicate
    with a master over a shared two-wire open-drain bus. It has largely replaced \link treehopper.api.spi.HardwareSpi
    Spi\endlink for
    many sensors and peripherals.

    The %Treehopper.Libraries distribution for your language/platform has support for many different peripherals you
    can use with the I<sup>2</sup>C peripheral; see the \ref libraries documentation for more details.

    Here's an example of a typical I<sup>2</sup>C arrangement:
    ![Typical I2C interfacing with Treehopper](images/i2c-overview.svg)

    Addressing
    ----------
    Each <i>I2C</i> peripheral on the bus must have a unique 7-bit address. This is almost always specified in the
    datasheet of the peripheral, and might also include the states of one or more address pins — input pins on the
    chip that can either be permanently tied low or high to control the address. This allows multiple instances of
    the same IC to be placed on the same bus, so long as the address pins are tied in a unique combination.

    SMBus
    -----
    System Management Bus (SMBus) is a protocol definition that sits on top of I<sup>2</sup>C, and is implemented by
    almost all modern I<sup>2</sup>C peripherals. Peripherals expose all functionality through <i>registers</i> (
    which are similar to the registers of an MCU). SMBus uses an 8-bit value to specify the register, thus supporting
    255 addresses. By manipulating these registers, the peripheral can be commanded to perform its functions,
    and data can also be read back from it.

    Implementation
    ==============
    %Treehopper implements an SMBus-compliant I<sup>2</sup>C master role that is compatible with almost all
    I<sup>2</sup>C peripherals on the market. %Treehopper does not support multi-master scenarios or I<sup>2</sup>C
    slave functionality.

    %send_receive() Function
    -----------------------
    It would be impractical for %Treehopper to directly expose low-level I<sup>2</sup>C functions (start bit,
    stop bit, ack/nack); instead, %Treehopper's I<sup>2</sup>C module supports a single high-level send_receive()
    function that is used to exchange data.

    This function can be used to either write data to the device (if `numBytesToRead` is `0`), read data from the
    device (if `writeData` is `null`), or both write data to the device and then read from it.

    This function is well-suited to reading and writing registers on an I2C SMBus-compatible peripheral.

    For example, to read a 16-bit register at address 0x31 from an I2C device with address 0x17, one can call:

        >>> write_data = 0x31  # the register to read from
        >>> address = 0x17  # 7-bit address
        >>> read_data = board.i2c.send_receive(address, write_data, 2)

    This translates to this transaction:
    ![ReadWrite function maps to a standard SMBus-compatible transaction](images/i2c-function-mapping.svg)

    Note that %Treehopper correctly implements the Restart condition when it requests data from the device after
    writing `writeData` to it.permission. While unusual, if your peripheral required a STOP condition to be sent
    before requesting data from it, simply break up the transaction into two SendReceiveAsync() calls:

        >>> write_data = 0x31  # the register to read from
        >>> address = 0x17  # 7-bit address
        >>> board.i2c.send_receive(address, write_data, 0)
        >>> read_data = board.I2c.send_receive(address, None, 2)


    Errors
    ------
    %Treehopper correctly detects and forwards %I2C errors to your application.

    Frequent issues
    ===============
    It can be difficult to diagnose I<sup>2</sup>C problems without a logic analyzer, but there are several common
    issues that arise that can be easily diagnosed without specialized tools.

    Pull-Up resistors
    -----------------
    %Treehopper does not have on-board I<sup>2</sup>C pull-up resistors on the SCL and SDA pins, as this would
    interfere with analog inputs on these pins. There are methodologies for selecting these resistors, but there's
    quite a bit of latitude -- we've found 4.7-10k resistors seem to work almost all the time, with normal numbers of
    slaves (say, fewer than 10) on a bus. If you have fewer slaves, you may need to decrease these resistor values.

    Note that many off-the-shelf modules you might buy from [Adafruit](https://www.adafruit.com), [SparkFun](
    https://www.sparkfun.com), [Amazon](https://www.amazon.com/)</a> or an [eBay](https://www.ebay.com/) vendor
    probably already have I2C pull-up resistors on them. It is usually not an issue if you have more than one of
    these modules on the bus, but depending on the pull-up resistor values use, the ICs may struggle to drive the bus
    with a large number of pull-up resistors on it.

    Addressing
    ----------
    At the protocol level, the device's 7-bit address is shifted to the left by 1, leaving the least-significant bit
    to be used to indicate a 1 for <i>Input</i> (read), and a 0 for <i>Output</i> (write) transactions. The
    %Treehopper API (and all %Treehopper libraries) use this 7-bit address. Unfortunately, the datasheets for some
    peripherals specify the peripheral's address in this shifted 8-bit format. To add further confusion,
    many peripherals have external address pins that can be tied high or low to set or clear the respective address
    bits. For example, Figure 1-4 from the MCP23017 datasheet gives
    ![MCP23017 address](images/mcp23017.png)
    To determine what address to use with %Treehopper, ignore the R/W bit completely, thus the 7-bit address is
    0b0100(a2)(a1)(a0). If we were to tie A0 high while leaving A1 and A2 low, the address would be 0b0100001,
    which is 0x21.

    Address conflicts
    -----------------
    With only 127 different I<sup>2</sup>C addresses available, it's actually quite common for ICs to have
    conflicting addresses. And some ICs --- especially low pin-count sensors --- lack external address pins that can
    be used to set the address. While many of these devices have a programmable address, this is an annoying
    chicken-and-the-egg problem that requires you to individually program the addresses of the ICs before they're
    installed together on your board.

    Some language APIs have <b>I2cMux</b>-inherited components in the Treehopper.Libraries.Interface.Mux namespace
    that might be useful for handling address conflicts. For example, the Treehopper.Libraries.Interface.Mux.I2cMux
    class allows you to use low-cost analog muxes (such as jellybean 405x-type parts that are often just a few cents
    each) as a transparent mux to share one Treehopper I<sup>2</sup>C bus with multiple slaves with conflicting
    addresses.

    Logic-Level Conversion
    ----------------------
    %Treehopper is a 3.3V device, which almost all modern peripheral ICs use as their recommended operating (or at
    least I/O) voltage. Furthermore, because I<sup>2</sup>C is an open-drain interface, logic-level conversion is
    usually not necessary when dealing with peripherals that operate anywhere between 2.8 and 5V. This range covers
    the vast majority of ICs in use today.

    If your 5V device has TTL-compatible logic (i.e., a V<sub>IH</sub> of 2V), no logic-level conversion is needed --
    you can simply wire these devices directly to %Treehopper's SCL and SDA pins, making sure to pull them up to
    3.3V. Since TTL specifies a minimum high voltage of 2V, the 3.3V signals generated by the pull-ups is sufficient.
    If the 5V device has a CMOS-compatible input, you should consider pulling up the SCL and SDA lines to 5V instead.

    On the opposite end of the spectrum, if you're dealing with 2.8V devices, make sure to pull up the bus to 2.8 ---
    not 3.3 --- volts. If you have lower-voltage devices, you'll need to build or buy a bidirectional logic level
    converter (which can be [as simple as a transistor and some pull-ups](
    http://www.nxp.com/documents/application_note/AN10441.pdf)).

    """

    ## \cond PRIVATE
    def __init__(self, board):
        self._board = board
        self._enabled = False
        self._speed = 100
        self._logger = logging.getLogger(__name__)

    ## \endcond
    @property
    def speed(self) -> float:
        """Gets or sets the speed, in kHz, of the I2C peripheral."""
        return self._speed

    @speed.setter
    def speed(self, value: float):
        if math.isclose(value, self._speed):
            return

        self._speed = value
        self._send_config()

    @property
    def enabled(self) -> bool:
        """Gets or sets whether the I2C peripheral is enabled"""
        return self._enabled

    @enabled.setter
    def enabled(self, value: bool):
        if value == self._enabled:
            return

        self._enabled = value
        if self._enabled:
            self._board.pins[3].mode = PinMode.Reserved
            self._board.pins[4].mode = PinMode.Reserved
        else:
            self._board.pins[3].mode = PinMode.Unassigned
            self._board.pins[4].mode = PinMode.Unassigned

        self._send_config()

    def send_receive(self, address: int, write_data=None, num_bytes_to_read=0) -> bytearray:
        """
        Send and receive bytes using the I2C peripheral.

        Args:
            address: The 7-bit slave address to address (int).
            write_data: A list of data to write to the slave (list[int], default: None).
            num_bytes_to_read: The number of bytes to read after the write operation (int, default: 0).

        Returns:
            A list of data received.

        To reduce USB communication chattiness, Treehopper has no API for primitive I2C operations (start condition,
        ACK, etc). Rather, Treehopper supports a single send_receive() function that sends a "start" condition, followed
        by the 7-bit slave address. Reading and writing data occurs according to write_data and num_bytes_to_read.

        If write_data is set, the "read" bit is cleared, and Treehopper will write write_data
        to the board. Then, if num_bytes_to_read is not 0, a restart condition will be sent, followed by the device
        address and "read" bit. Treehopper will then read num_bytes_to_read bytes from the device.

        If write_data is None, the "read" bit is set, and Treehopper will simply read
        num_bytes_to_read bytes.

        By supporting both None write_data and num_bytes_to_read=0 conditions, this function can be used for all
        standard I2C/SMBus transactions.

        Most I2C devices use a register-based scheme for exchanging data; consider using \link
        treehopper.libraries.smbus_device.SMBusDevice SMBusDevice\endlink for interacting with these devices.
        """
        if not self._enabled:
            self._logger.error("I2c.send_receive() called before enabling the peripheral. This call will be ignored.")
            return
        if not write_data:
            tx_size = 0
        else:
            tx_size = len(write_data)
        with self._board._comms_lock:
            send_data = [DeviceCommands.I2cTransaction, address, tx_size, num_bytes_to_read]
            if write_data:
                send_data += write_data
            data_chunks = chunks(send_data, 64)
            for chunk in data_chunks:
                self._board._send_peripheral_config_packet(chunk)

            read_data = []
            bytes_remaining = num_bytes_to_read + 1

            while bytes_remaining > 0:
                if bytes_remaining > 64:
                    num_bytes_to_read = 64
                else:
                    num_bytes_to_read = bytes_remaining

                read_data += self._board._receive_comms_response_packet(num_bytes_to_read)
                bytes_remaining -= num_bytes_to_read
            if read_data[0] != 255:
                raise RuntimeError(
                    "I2C transaction resulted in an error: {}".format(I2CTransferError.ErrorString(read_data[0])))

            return read_data[1:]

    def _send_config(self):
        th0 = 256.0 - 4000.0 / (3.0 * self._speed)
        if th0 < 0 or th0 > 255.0:
            self._logger.warning("Rate out of limits. Valid rate is 62.5 kHz - 16000 kHz (16 MHz). Clipping.")
            th0 = constrain(th0, 255, 0)
        data_to_send = [DeviceCommands.I2cConfig, self._enabled, round(th0)]
        self._board._send_peripheral_config_packet(data_to_send)

    def __str__(self):
        if self.enabled:
            return "Not enabled"
        else:
            return "Enabled, {:0.2f} kHz".format(self._speed)
