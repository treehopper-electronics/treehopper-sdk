#pragma once

#include <vector>
#include "Treehopper.h"
#include "Spi.h"

namespace Treehopper {
    class TreehopperUsb;

    /** Built-in SPI peripheral

# Basic Usage
Once enabled(), you can use the hardware SPI module on %Treehopper through the sendReceive() method, which is used to simultaneously transmit and/or receive data.

# Background
SPI is a full-duplex synchronous serial interface useful for interfacing with both complex, high-speed peripherals, as well as simple LED drivers, output ports, and any other general-purpose input or output shift register.

Compared to I<sup>2</sup>, SPI is a simpler protocol, generally much faster, and less popular for modern peripheral ICs.

![Basic SPI interfacing](images/spi-overview.svg)

## Pins
%Treehopper supports SPI master mode with the following pins:
 - <b>MISO</b> <i>(Master In, Slave Out)</i>: This pin carries data from the slave to the master.
 - <b>MOSI</b> <i>(Master Out, Slave In)</i>: This pin carries data from the master to the peripheral
 - <b>SCK</b> <i>(Serial Clock)</i>: This pin clocks the data into and out of the master and slave device.

Not all devices use all pins, but the SPI peripheral will always reserve the SCK, MISO, and MOSI pin once the peripheral is enabled, so these pins cannot be used for other functions.

## Chip Select
Almost all SPI peripherals also use some sort of chip select (CS) pin, which indicates a valid transaction. Thus, the easiest way to place multiple peripherals on a bus is by using a separate chip select pin for each peripheral (since a peripheral will ignore SPI traffic without a valid chip select signal). %Treehopper supports two different chip-select styles:
 - SPI mode: chip-select is asserted at the beginning of a transaction, and de-asserted at the end; and
 - Shift output mode: chip-select is strobed at the end of a transaction
 - Shift input mode: chip-select is strobed at the beginning of a transaction
These styles support both active-low and active-high signal polarities.

## SPI Mode
SPI does not specify a transaction-level protocol for accessing peripheral functions (unlike, say, SMBus for I2c does); as a result, peripherals that use SPI have wildly different implementations. Even basic aspects -- when data is clocked, and the polarity of the clock signal -- vary by IC. This property is often called the "SPI mode" of the peripheral; %Treehopper supports all four modes:
 - <b>Mode 0 (00):</b> Clock is idle-low. Data is latched in on the clock's rising edge and data is output on the falling edge.
 - <b>Mode 1 (01):</b> Clock is idle-low. Data is latched in on the clock's falling edge and data is output on the rising edge.
 - <b>Mode 2 (10):</b> Clock is idle-high. Data is latched in on the clock's rising edge and data is output on the falling edge.
 - <b>Mode 3 (11):</b> Clock is idle-high. Data is latched in on the clock's falling edge and data is output on the rising edge.

## Clock Speed
%Treehopper supports SPI clock rates as low as 93.75 kHz and as high as 24 MHz, but we recommend a clock speed of 6 MHz for most cases. You will not notice performance gains above 6 MHz, since this is the fastest rate that %Treehopper's MCU can place bytes into the SPI buffer; any faster and the SPI peripheral will have to wait for the CPU before transmitting the next byte.

\note In the current firmware release, clock rates between 800 kHz and 6 MHz are disallowed. There appears to be a silicon bug in the SPI FIFO that can cause lock-ups with heavy USB traffic. We hope to create a workaround for this issue in future firmware updates.

## Burst mode
If you only need to transmit or receive data from the device, %Treehopper supports an \link Treehopper.SpiBurstMode SpiBurstMode\endlink flag, which can improve performance substantially (especially in the case of BurstTx, which eliminates the back-and-forth needed, reducing transaction times down to a few hundred microseconds).

## Chaining Devices & Shift Registers
%Treehopper's SPI module works well for interfacing with many types of shift registers, which typically have a single output state "register" that is updated whenever new SPI data comes in. Because of the nature of SPI, any existing data in this register is sent to the MISO pin (sometimes labeled "DO" --- digital output --- or, confusingly, "SO" --- serial output). Thus, many shift registers (even of different types) can be chained together by connecting the DO pin of each register to the DI pin of the next:

![Many shift registers can share the SPI bus and CS line](images/spi-shift-register.svg)
Please note that most shift registers refer to their "CS" pin as a "latch enable" (LE) signal.

In the example above, if both of these shift registers were 8-bit, sending the byte array {0xff, 0x03} would send "0xff" to the right register, and "0x03" to the left one.

%Treehopper.Libraries has support for many different peripherals you can use with the %SPI peripheral, including shift registers. See the \ref libraries documentation for more details on all the library components. Examples of shift register library components include Treehopper.Libraries.Displays.LedShiftRegister, Treehopper.Libraries.IO.PortExpander.Hc166, Treehopper.Libraries.IO.PortExpander.Hc595.

 ## Further Reading
 Wikipedia has an excellent SPI article: [Serial Peripheral Interface Bus](https://en.wikipedia.org/wiki/Serial_Peripheral_Interface_Bus)
 */
    class TREEHOPPER_API HardwareSpi : public Spi {
    public:
        explicit HardwareSpi(TreehopperUsb &board);

        /** gets whether the SPI module is enabled */
        bool enabled() override;

        /** sets whether the SPI module is enabled */
        void enabled(bool) override;

#if 0
        std::vector<uint8_t> sendReceive(std::vector<uint8_t> dataToWrite,
                                         SpiChipSelectPin *chipSelect = nullptr,
                                         ChipSelectMode chipSelectMode = ChipSelectMode::SpiActiveLow,
                                         double speed = 6,
                                         SpiBurstMode burstMode = SpiBurstMode::NoBurst,
                                         SpiMode spiMode=SpiMode::Mode00) override;
#endif
        /** \cond PRIVATE */
        std::vector<uint8_t> sendReceive(std::vector<uint8_t> dataToWrite,
                                         SpiChipSelectPin *chipSelect,
                                         ChipSelectMode chipSelectMode,
                                         double speed,
                                         SpiBurstMode burstMode,
                                         SpiMode spiMode) override;
         /** \endcond */
    private:
        TreehopperUsb &board;
        bool _enabled;
    };
}
