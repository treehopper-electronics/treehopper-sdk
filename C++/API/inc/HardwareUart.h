#pragma once

#include "Uart.h"
#include "OneWire.h"

namespace Treehopper {

    enum class UartMode {

        /**
         * The module is operating in UART mode
         */
                Uart,

        /**
         * The module is operating in OneWire mode. Only the TX pin is used.
         */
                OneWire
    };

    class TreehopperUsb;

    /** Built-in UART peripheral

The UART peripheral allows you to send and receive standard-format RS-232-style asynchronous serial communications.

## Pins
When the UART is enabled, the following pins will be unavailable for other use:
- <b>TX</b> <i>(Transmit)</i>: This pin carries data from %Treehopper to the device you've attached to the UART.
- <b>RX</b> <i>(Receive)</i>: This pin carries data from the device to %Treehopper.

Note that UART cross-over is a common problem when people are attaching devices together; always consult the documentation for the device you're attaching to %Treehopper to ensure that the TX signal from %Treehopper is flowing into the receive input (RX, DIN, etc) of the device, and vice-versa. Since you are unlikely to damage either device by incorrectly connecting the TX and RX pins, it is a common troubleshooting practice to simply swap TX and RX if the system doesn't appear to be functioning properly.

## One-Wire Mode
%Treehopper's UART has built-in support for One-Wire mode with few external circuitry requirements. When you use the UART in One-Wire mode, the TX pin will switch to an open-drain mode. You must physically tie the RX and TX pins together --- this is the data pin for the One-Wire bus. Most One-Wire sensors and devices you use will require an external pull-up resistor on the bus.

## Implementation Details
%Treehopper's UART is designed for average baud rates; the range of supported rates is 7813 baud to 2.4 Mbaud, though communication will be less reliable above 1-2 Mbaud.

Transmitting data is straightforward: simply pass a byte array --- up to 63 characters long --- to the send() function once the UART is enabled.

Receiving data is more challenging, since incoming data can appear on the RX pin at any moment when the UART is enabled. Since all actions on %Treehopper are initiated on the host, to get around UART's inherent asynchronicity, a 32-byte buffer holds any received data that comes in while the UART is enabled. Then, when the host wants to access this data, it can receive() it from the board to obtain the buffer.

Whenever receive() is called, the entire buffer is sent to the host, and the buffer's pointer is reset to 0 (i.e., the buffer is reset). This can be useful for clearing out any gibberish and returning the UART to a known state before you expect to receive data --- for example, if you're addressing a device that you send commands to, and read responses back from, you may wish to call receive() before sending the command; that way, parsing the received data will be simpler.

## Other Considerations
This ping-pong short-packet-oriented back-and-forth scenario is what %Treehopper's UART is built for, as it's what's most commonly needed when interfacing with embedded devices that use a UART.

There is a tight window of possible baud rates where it is plausible to receive data continuously without interruption. For example, at 9600 baud, the receive() function only need to finish execution every 33 milliseconds, which can easily be accomplished in most operating systems. However, because data is not double-buffered on the board, under improbable circumstances, continuously-transmitted data may inadvertently be discarded.

%Treehopper's UART is not designed to replace a high-quality CDC-class USB-to-serial converter, especially for high data-rate applications. In addition to streaming large volumes of data continuously, USB CDC-class UARTs should also offer lower latency for receiving data. %Treehopper also has no way of exposing its UART to the operating system as a COM port, so it's most certainly not a suitable replacement for a USB-to-serial converter in most applications.
*/
    class HardwareUart : public Uart, OneWire {
    public:
        void startOneWire() override;

        void oneWireResetAndMatchAddress(uint64_t address) override;

        std::vector<uint64_t> oneWireSearch() override;

        bool oneWireReset() override;

        std::vector<uint8_t> receive(int numBytes) override;

        void send(std::vector<uint8_t> dataToSend) override;

        void send(uint8_t dataToSend) override;

        void startUart() override;

        std::vector<uint8_t> receive() override;

        void baud(int baud) override;

        int baud() override;

        /** sets the UartMode of the UART peripheral */
        void mode(UartMode mode);

        /** gets the UartMode of the UART peripheral */
        UartMode mode();

        /** Sets whether the UART is enabled */
        void enabled(bool enabled);

        bool enabled();


    private:
        void updateConfig();
        TreehopperUsb &_device;
        bool _enabled = false;
        int _baud = 9600;
        UartMode _mode = UartMode::Uart;
        bool _useOpenDrainTx = false;

        enum class UartConfig
        {
            Disabled,
            Standard,
            OneWire
        };

        enum class UartCommand
        {
            Transmit,
            Receive,
            OneWireReset,
            OneWireScan
        };
    };
}
