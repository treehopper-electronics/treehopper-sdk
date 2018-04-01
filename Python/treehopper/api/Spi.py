import logging
import math
from typing import List, TYPE_CHECKING

from treehopper.api.interfaces import Spi, SpiBurstMode, SpiMode, ChipSelectMode
from treehopper.api.pin import PinMode
from treehopper.utils.utils import *
from treehopper.api.device_commands import DeviceCommands
if TYPE_CHECKING:
    from treehopper.api.treehopper_usb import TreehopperUsb


class HardwareSpi(Spi):
    """
    Built-in SPI peripheral

    Attributes:
        sck: The SCK pin on the board
        miso: The MISO pin on the board
        mosi: The MOSI pin on the board

    \section basic Basic Usage
    Once \link #enable enabled\endlink, you can use the hardware SPI module on %Treehopper through the #send_receive() method, which is used to simultaneously transmit and/or receive data.

    \section back Background
    SPI is a full-duplex synchronous serial interface useful for interfacing with both complex, high-speed peripherals, as well as simple LED drivers, output ports, and any other general-purpose input or output shift register.

    Compared to I<sup>2</sup>, SPI is a simpler protocol, generally much faster, and less popular for modern peripheral ICs.

    ![Basic SPI interfacing](images/spi-overview.svg)

    \subsection pins Pins
    %Treehopper supports SPI master mode with the following pins:
     - <b>MISO</b> <i>(Master In, Slave Out)</i>: This pin carries data from the slave to the master.
     - <b>MOSI</b> <i>(Master Out, Slave In)</i>: This pin carries data from the master to the peripheral
     - <b>SCK</b> <i>(Serial Clock)</i>: This pin clocks the data into and out of the master and slave device.

    Not all devices use all pins, but the SPI peripheral will always reserve the SCK, MISO, and MOSI pin once the peripheral is enabled, so these pins cannot be used for other functions.

    \subsection chipselect Chip Select
    Almost all SPI peripherals also use some sort of chip select (CS) pin, which indicates a valid transaction. Thus, the easiest way to place multiple peripherals on a bus is by using a separate chip select pin for each peripheral (since a peripheral will ignore SPI traffic without a valid chip select signal). %Treehopper supports two different chip-select styles:
     - SPI mode: chip-select is asserted at the beginning of a transaction, and de-asserted at the end; and
     - Shift output mode: chip-select is strobed at the end of a transaction
     - Shift input mode: chip-select is strobed at the beginning of a transaction
    These styles support both active-low and active-high signal polarities.

    \subsection mode SPI Mode
    SPI does not specify a transaction-level protocol for accessing peripheral functions (unlike, say, SMBus for I2c does); as a result, peripherals that use SPI have wildly different implementations. Even basic aspects -- when data is clocked, and the polarity of the clock signal -- vary by IC. This property is often called the "SPI mode" of the peripheral; %Treehopper supports all four modes:
     - <b>Mode 0 (00):</b> Clock is idle-low. Data is latched in on the clock's rising edge and data is output on the falling edge.
     - <b>Mode 1 (01):</b> Clock is idle-low. Data is latched in on the clock's falling edge and data is output on the rising edge.
     - <b>Mode 2 (10):</b> Clock is idle-high. Data is latched in on the clock's rising edge and data is output on the falling edge.
     - <b>Mode 3 (11):</b> Clock is idle-high. Data is latched in on the clock's falling edge and data is output on the rising edge.

    \subsection cs Clock Speed
    %Treehopper supports SPI clock rates as low as 93.75 kHz and as high as 24 MHz, but we recommend a clock speed of 6 MHz for most cases. You will not notice performance gains above 6 MHz, since this is the fastest rate that %Treehopper's MCU can place bytes into the SPI buffer; any faster and the SPI peripheral will have to wait for the CPU before transmitting the next byte.

    \note In the current firmware release, clock rates between 800 kHz and 6 MHz are disallowed. There appears to be a silicon bug in the SPI FIFO that can cause lock-ups with heavy USB traffic. We hope to create a workaround for this issue in future firmware updates.

    \subsection burst Burst mode
    If you only need to transmit or receive data from the device, %Treehopper supports an \link treehopper.api.interfaces.SpiBurstMode SpiBurstMode\endlink flag, which can improve performance substantially (especially in the case of BurstTx, which eliminates the back-and-forth needed, reducing transaction times down to a few hundred microseconds).

    \subsection chaining Chaining Devices & Shift Registers
    %Treehopper's SPI module works well for interfacing with many types of shift registers, which typically have a single output state "register" that is updated whenever new SPI data comes in. Because of the nature of SPI, any existing data in this register is sent to the MISO pin (sometimes labeled "DO" --- digital output --- or, confusingly, "SO" --- serial output). Thus, many shift registers (even of different types) can be chained together by connecting the DO pin of each register to the DI pin of the next:

    ![Many shift registers can share the SPI bus and CS line](images/spi-shift-register.svg)
    Please note that most shift registers refer to their "CS" pin as a "latch enable" (LE) signal.

    In the example above, if both of these shift registers were 8-bit, sending the byte array {0xff, 0x03} would send "0xff" to the right register, and "0x03" to the left one.

    %Treehopper.Libraries has support for many different peripherals you can use with the %SPI peripheral, including shift registers. See the \ref libraries documentation for more details on all the library components.

     \subsection further Further Reading
     Wikipedia has an excellent SPI article: [Serial Peripheral Interface Bus](https://en.wikipedia.org/wiki/Serial_Peripheral_Interface_Bus)
    """

    ## \cond PRIVATE
    def __init__(self, board: 'TreehopperUsb'):
        self._board = board
        self._enabled = False
        self._logger = logging.getLogger(__name__)
        self.sck = self._board.pins[0]
        self.miso = self._board.pins[1]
        self.mosi = self._board.pins[2]
    ## \endcond

    @property
    def enabled(self) -> bool:
        """
        Gets or sets whether the module is enabled

        Returns:
            (bool) whether the module is enabled
        """
        return self._enabled

    @enabled.setter
    def enabled(self, value: bool):
        if value == self._enabled:
            return

        self._enabled = value
        if self._enabled:
            self.sck.mode  = PinMode.Reserved
            self.miso.mode = PinMode.Reserved
            self.mosi.mode = PinMode.Reserved
        else:
            self.sck.mode  = PinMode.Unassigned
            self.miso.mode = PinMode.Unassigned
            self.mosi.mode = PinMode.Unassigned

        self._send_config()

    def send_receive(self, data_to_write: List[int], chip_select=None, chip_select_mode=ChipSelectMode.SpiActiveLow,
                     speed_mhz=6, burst_mode=SpiBurstMode.NoBurst, spi_mode=SpiMode.Mode00) -> List[int]:
        """
        Send/receive data
        Args:
            data_to_write (list[int]): a byte array of the data to send. The length of the transaction is determined by the length of this array.
            chip_select (Pin): The chip select pin, if any, to use during this transaction.
            chip_select_mode (ChipSelectMode): The chip select mode to use during this transaction (if a CS pin is selected)
            speed_mhz (float): The speed to perform this transaction at.
            burst_mode (SpiBurstMode): The burst mode (if any) to use.
            spi_mode (SpiMode): The SPI mode to use during this transaction.

        Returns:
            (List[int]) A byte array with the received data
        """
        if not self._enabled:
            self._logger.error("spi.send_receive() called before enabling the peripheral. This call will be ignored.")
            return

        if chip_select is not None and chip_select.spi_module != self:
            self._logger.error("Chip select pin must belong to this SPI module. This call will be ignored.")

        if chip_select is not None:
            chip_select_pin_number = chip_select.number
        else:
            chip_select_pin_number = 255

        if not data_to_write:
            tx_size = 0
        else:
            tx_size = len(data_to_write)

        if 0.8 < speed_mhz < 6:
            self._logger.info("NOTICE: automatically rounding up SPI speed to 6 MHz, due to a possible silicon bug. "
                              "This bug affects SPI speeds between 0.8 and 6 MHz, so if you need a speed lower than 6 "
                              "MHz, please set to 0.8 MHz or lower.")
            speed_mhz = 6

        spi0_ckr = round(24.0 / speed_mhz - 1)
        if spi0_ckr > 255.0:
            spi0_ckr = constrain(spi0_ckr, 0, 255)
            self._logger.error("NOTICE: Requested SPI frequency of {} MHz is below the minimum frequency, and will be "
                               "clipped to 0.09375 MHz (93.75 kHz).".format(speed_mhz))
        elif spi0_ckr < 0:
            spi0_ckr = constrain(spi0_ckr, 0, 255)
            self._logger.error(
                "NOTICE: Requested SPI frequency of {} MHz is above the maximum frequency, and will be clipped to 24 "
                "MHz.".format(speed_mhz))

        actual_frequency = 48.0 / (2.0 * (spi0_ckr + 1.0))

        if math.isclose(actual_frequency, speed_mhz, abs_tol=1):
            self._logger.info("NOTICE: SPI module actual frequency of {} MHz is more than 1 MHz away from the "
                              "requested frequency of {} MHz".format(actual_frequency, speed_mhz))

        if tx_size > 255:
            self._logger.error("Maximum packet length is 255 bytes")

        header = [DeviceCommands.SpiTransaction, chip_select_pin_number, chip_select_mode, spi0_ckr, spi_mode,
                  burst_mode, tx_size]

        with self._board._comms_lock:
            if burst_mode == SpiBurstMode.BurstRx:
                self._board._send_peripheral_config_packet(header)
            else:
                data_to_send = header + data_to_write
                data_chunks = chunks(data_to_send, 64)
                for chunk in data_chunks:
                    self._board._send_peripheral_config_packet(chunk)

                read_data = []
                if burst_mode != SpiBurstMode.BurstTx:
                    bytes_remaining = tx_size

                    while bytes_remaining > 0:
                        if bytes_remaining > 64:
                            num_bytes_to_read = 64
                        else:
                            num_bytes_to_read = bytes_remaining

                        read_data += self._board._receive_comms_response_packet(num_bytes_to_read)
                        bytes_remaining -= num_bytes_to_read

                return read_data

    def _send_config(self):
        data_to_send = [DeviceCommands.SpiConfig, self._enabled]
        self._board._send_peripheral_config_packet(data_to_send)

    def __str__(self):
        if self.enabled:
            return "Not enabled"
        else:
            return "Enabled"