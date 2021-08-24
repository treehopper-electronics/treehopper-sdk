package io.treehopper;

import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

import io.treehopper.interfaces.I2c;

/** Built-in I<sup>2</sup>C module

# Quick Start
Once you've connected to your board, you can enable the %I2C peripheral by settings the enabled() property to true, and then send/receive data.

\code{.java}
auto board = ConnectionService::instance().getFirstDevice();
board.connect();
board.i2c.enabled(true);

byte writeData = 0x31; // the register to read from
byte address = 0x17; // 7-bit address
byte[] readData = board.i2c.sendReceive(address, new byte[]{ writeData }, 2)
\endcode

If you want to change the communication rate from the default 100 kHz, consult the speed() property.

Unless you need to perform raw I2C transactions, we recommend using SMBusDevice, which provides useful methods for reading and writing I2C peripheral registers. Almost all %I2C drivers in %Treehopper::Libraries use it.

Speaking of which, before writing a driver yourself, check to make it's not already in Treehopper::Libraries. You may save yourself some time!

# Background
I<sup>2</sup>C (%I2C, or IIC) is a low-speed synchronous serial protocol that allows up to 127 ICs to communicate with a master over a shared two-wire open-drain bus. It has largely replaced \link HardwareSpi Spi\endlink for many sensors and peripherals.

The %Treehopper::Libraries distribution for your language/platform has support for many different peripherals you can use with the I<sup>2</sup>C peripheral; see the \ref libraries documentation for more details.

Here's an example of a typical I<sup>2</sup>C arrangement:
![Typical I2C interfacing with Treehopper](images/i2c-overview.svg)

## Addressing
Each <i>I2C</i> peripheral on the bus must have a unique 7-bit address. This is almost always specified in the datasheet of the peripheral, and might also include the states of one or more address pins — input pins on the chip that can either be permanently tied low or high to control the address. This allows multiple instances of the same IC to be placed on the same bus, so long as the address pins are tied in a unique combination. 

## SMBus
System Management Bus (SMBus) is a protocol definition that sits on top of I<sup>2</sup>C, and is implemented by almost all modern I<sup>2</sup>C peripherals. Peripherals expose all functionality through <i>registers</i> (which are similar to the registers of an MCU). SMBus uses an 8-bit value to specify the register, thus supporting 255 addresses. By manipulating these registers, the peripheral can be commanded to perform its functions, and data can also be read back from it.

# Implementation
%Treehopper implements an SMBus-compliant I<sup>2</sup>C master role that is compatible with almost all I<sup>2</sup>C peripherals on the market. %Treehopper does not support multi-master scenarios or I<sup>2</sup>C slave functionality.

## SendReceiveAsync Function
It would be impractical for %Treehopper to directly expose low-level I<sup>2</sup>C functions (start bit, stop bit, ack/nack); instead, %Treehopper's I<sup>2</sup>C module supports a single high-level sendReceive() function that is used to exchange data.

This function can be used to either write data to the device (if `numBytesToRead` is `0`), read data from the device (if `writeData` is `null`), or both write data to the device and then read from it.

This function is well-suited to reading and writing registers on an I2C SMBus-compatible peripheral.

For example, to read a 16-bit register at address 0x31 from an I2C device with address 0x17, one can call:

```{.java}
byte writeData = 0x31; // the register to read from
byte address = 0x17; // 7-bit address
byte[] readData = board.i2c.sendReceive(address, new byte[]{ writeData }, 2)
```

This translates to this transaction:
![ReadWrite function maps to a standard SMBus-compatible transaction](images/i2c-function-mapping.svg)

Note that %Treehopper correctly implements the Restart condition when it requests data from the device after writing `writeData` to it. While unusual, if your peripheral required a STOP condition to be sent before requesting data from it, simply break up the transaction into two sendReceive() calls:

```{.java}
byte writeData = 0x31; // the register to read from
byte address = 0x17; // 7-bit address
board.i2c.sendReceive(address, new byte[]{ writeData }, 0)
byte[] readData = board.i2c.sendReceive(address, null, 2)
```

## Errors
%Treehopper correctly detects and forwards %I2C errors to your application.

# Frequent Issues
It can be difficult to diagnose I<sup>2</sup>C problems without a logic analyzer, but there are several common issues that arise that can be easily diagnosed without specialized tools.

## Pull-Up Resistors
%Treehopper does not have on-board I<sup>2</sup>C pull-up resistors on the SCL and SDA pins, as this would interfere with analog inputs on these pins. There are methodologies for selecting these resistors, but there's quite a bit of latitude -- we've found 4.7-10k resistors seem to work almost all the time, with normal numbers of slaves (say, fewer than 10) on a bus. If you have fewer slaves, you may need to decrease these resistor values.

Note that many off-the-shelf modules you might buy from [Adafruit](https://www.adafruit.com), [SparkFun](https://www.sparkfun.com), [Amazon](https://www.amazon.com/)</a> or an [eBay](https://www.ebay.com/) vendor probably already have I2C pull-up resistors on them. It is usually not an issue if you have more than one of these modules on the bus, but depending on the pull-up resistor values use, the ICs may struggle to drive the bus with a large number of pull-up resistors on it.

## Addressing
At the protocol level, the device's 7-bit address is shifted to the left by 1, leaving the least-significant bit to be used to indicate a 1 for <i>Input</i> (read), and a 0 for <i>Output</i> (write) transactions. The %Treehopper API (and all %Treehopper libraries) use this 7-bit address. Unfortunately, the datasheets for some peripherals specify the peripheral's address in this shifted 8-bit format. To add further confusion, many peripherals have external address pins that can be tied high or low to set or clear the respective address bits. For example, Figure 1-4 from the MCP23017 datasheet gives
![MCP23017 address](images/mcp23017.png)
To determine what address to use with %Treehopper, ignore the R/W bit completely, thus the 7-bit address is 0b0100(a2)(a1)(a0). If we were to tie A0 high while leaving A1 and A2 low, the address would be 0b0100001, which is 0x21.

## Address Conflicts
With only 127 different I<sup>2</sup>C addresses available, it's actually quite common for ICs to have conflicting addresses. And some ICs --- especially low pin-count sensors --- lack external address pins that can be used to set the address. While many of these devices have a programmable address, this is an annoying chicken-and-the-egg problem that requires you to individually program the addresses of the ICs before they're installed together on your board.

Some language APIs have <b>I2cMux</b>-inherited components in the Treehopper.Libraries.Interface.Mux namespace that might be useful for handling address conflicts. For example, the Treehopper.Libraries.Interface.Mux.I2cMux class allows you to use low-cost analog muxes (such as jellybean 405x-type parts that are often just a few cents each) as a transparent mux to share one Treehopper I<sup>2</sup>C bus with multiple slaves with conflicting addresses. 

## Logic-Level Conversion
%Treehopper is a 3.3V device, which almost all modern peripheral ICs use as their recommended operating (or at least I/O) voltage. Furthermore, because I<sup>2</sup>C is an open-drain interface, logic-level conversion is usually not necessary when dealing with peripherals that operate anywhere between 2.8 and 5V. This range covers the vast majority of ICs in use today.

If your 5V device has TTL-compatible logic (i.e., a V<sub>IH</sub> of 2V), no logic-level conversion is needed -- you can simply wire these devices directly to %Treehopper's SCL and SDA pins, making sure to pull them up to 3.3V. Since TTL specifies a minimum high voltage of 2V, the 3.3V signals generated by the pull-ups is sufficient. If the 5V device has a CMOS-compatible input, you should consider pulling up the SCL and SDA lines to 5V instead.

On the opposite end of the spectrum, if you're dealing with 2.8V devices, make sure to pull up the bus to 2.8 --- not 3.3 --- volts. If you have lower-voltage devices, you'll need to build or buy a bidirectional logic level converter (which can be [as simple as a transistor and some pull-ups](http://www.nxp.com/documents/application_note/AN10441.pdf)).
*/
public class HardwareI2c implements I2c {

    static final Logger logger = LogManager.getLogger("HardwareI2c");

    boolean enabled;
    double speed = 100.0;
    TreehopperUsb board;

    HardwareI2c(TreehopperUsb board) {
        this.board = board;
    }

    /**
     * Gets whether the I2c peripheral is enabled
     *
     * @return
     */
    @Override
    public boolean isEnabled() {
        return enabled;
    }

    /**
     * Sets whether the I2c peripheral is enabled
     *
     * @param enabled
     */
    @Override
    public void setEnabled(boolean enabled) {
        if (this.enabled == enabled) return;

        this.enabled = enabled;
        SendConfig();
    }

    /**
     * Gets the I2c peripheral speed
     *
     * @return the I2c speed, in kHz
     */
    @Override
    public double getSpeed() {
        return speed;
    }

    /**
     * Sets the I2c peripheral speed
     *
     * @param speed the I2c speed, in kHz
     */
    @Override
    public void setSpeed(double speed) {
        if (Utility.CloseTo(speed, this.speed)) return; // don't update if we're already there

        this.speed = speed;
        SendConfig();
    }

    private void SendConfig() {
        double TH0 = 256.0 - 4000.0 / (3.0 * speed);
        if (TH0 < 0 || TH0 > 255) {
            if (TreehopperUsb.Settings.shouldThrowExceptions()) {
                throw new IllegalArgumentException("Speed out of limits. Valid speeds are 62.5 kHz to 16000 kHz");
            }

            logger.error("Speed out of limits. Valid rate is 62.5 kHz - 16000 kHz (16 MHz)");
        }
        byte[] dataToSend = new byte[3];
        dataToSend[0] = (byte) DeviceCommands.I2cConfig.ordinal();
        dataToSend[1] = (byte) (enabled ? 0x01 : 0x00);
        dataToSend[2] = (byte) Math.round(TH0);
        board.sendPeripheralConfigPacket(dataToSend);
    }

    /**
     * send and receive data with this I2c module
     *
     * @param address        the slave address to send/receive with
     * @param dataToWrite    the data to write
     * @param numBytesToRead the number of bytes to read
     * @return the data received
     */
    @Override
    public byte[] sendReceive(byte address, byte[] dataToWrite, int numBytesToRead) {
        if (!enabled) {
            String message = "I2c.SendReceive() called before enabling the peripheral. This call will be ignored.";
            logger.error(message);
            if (TreehopperUsb.Settings.shouldThrowExceptions()) {
                throw new RuntimeException(message);
            }
        }

        if (numBytesToRead > 255) {
            logger.error("You may only receive up to 255 bytes per transaction.");
            if (TreehopperUsb.Settings.shouldThrowExceptions()) {
                throw new IllegalArgumentException("You may only receive up to 255 bytes per transaction.");
            }
        }

        byte[] receivedData = new byte[numBytesToRead];
        int txLen = dataToWrite.length;

        synchronized (board.comsLock) {
            byte[] dataToSend = new byte[4 + txLen]; // 2 bytes for the header
            dataToSend[0] = (byte) DeviceCommands.I2cTransaction.ordinal();
            dataToSend[1] = address;
            dataToSend[2] = (byte) txLen; // total length (0-255)
            dataToSend[3] = (byte) numBytesToRead;

            System.arraycopy(dataToWrite, 0, dataToSend, 4, txLen);

            int bytesRemaining = dataToSend.length;
            int offset = 0;

            // for long transactions (> 64 bytes - 4 byte header), we send <=64 byte chunks, one by one.
            while (bytesRemaining > 0) {
                int transferLength = bytesRemaining > 64 ? 64 : bytesRemaining;
                byte[] tmp = new byte[transferLength];
                System.arraycopy(dataToSend, offset, tmp, 0, transferLength);
                board.sendPeripheralConfigPacket(tmp);

                offset += transferLength;
                bytesRemaining -= transferLength;
            }

            if (numBytesToRead == 0) {
                //var result = device.receiveCommsResponsePacket((uint)1).Result;
                byte[] result = board.receiveCommsResponsePacket(1);
                int resultCode = result[0] & 0xff;
                if (resultCode != 255) {
                    I2cTransferError error = I2cTransferError.values()[resultCode];

                    logger.error("I2C transaction resulted in an error: " + error);
                    if (TreehopperUsb.Settings.shouldThrowExceptions())
                        throw new RuntimeException("I2C transaction resulted in an error: " + error);
                }

            } else {
                bytesRemaining = numBytesToRead + 1; // received data length + status byte
                int srcIndex = 0;
                byte[] result = new byte[bytesRemaining];
                while (bytesRemaining > 0) {
                    int numBytesToTransfer = bytesRemaining > 64 ? 64 : bytesRemaining;

                    byte[] chunk = board.receiveCommsResponsePacket(numBytesToTransfer);
                    System.arraycopy(chunk, 0, result, srcIndex, chunk.length); // just in case we don't get what we're expecting
                    srcIndex += numBytesToTransfer;
                    bytesRemaining -= numBytesToTransfer;
                }
                int resultCode = result[0] & 0xff;
                if (resultCode != 255) {
                    I2cTransferError error = I2cTransferError.values()[resultCode];

                    logger.error("I2C transaction resulted in an error: " + error);
                    if (TreehopperUsb.Settings.shouldThrowExceptions())
                        throw new RuntimeException("I2C transaction resulted in an error: " + error);
                } else {
                    System.arraycopy(result, 1, receivedData, 0, numBytesToRead);
                }
            }
        }

        return receivedData;
    }

    enum I2cTransferError {
        ArbitrationLostError(0),
        NackError(1),
        UnknownError(2),
        TxunderError(3),
        Success(255);

        private int numVal;

        I2cTransferError(int numVal) {
            this.numVal = numVal;
        }

        public int getNumVal() {
            return numVal;
        }
    }
}
