using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Treehopper
{
/** Built-in UART peripheral

The UART peripheral allows you to send and receive standard-format RS-232-style asynchronous serial communications. 

## Pins
When the UART is enabled, the following pins will be unavailable for other use:
- <b>TX</b> <i>(Transmit)</i>: This pin carries data from Treehopper to the device you've attached to the UART.
- <b>RX</b> <i>(Receive)</i>: This pin carries data from the device to Treehopper.

Note that UART cross-over is a common problem when people are attaching devices together; always consult the documentation for the device you're attaching to Treehopper to ensure that the TX signal from Treehopper is flowing into the receive input (RX, DIN, etc) of the device, and vice-versa. Since you are unlikely to damage either device by incorrectly connecting the TX and RX pins, it is a common troubleshooting practice to simply swap TX and RX if the system doesn't appear to be functioning properly.

## One-Wire Mode
Treehopper's UART has built-in support for One-Wire mode with few external circuitry requirements. When you use the UART in One-Wire mode, the TX pin will switch to an open-drain mode. You must physically tie the RX and TX pins together --- this is the data pin for the One-Wire bus. Most One-Wire sensors and devices you use will require an external pull-up resistor on the bus.

## Implementation Details
Treehopper's UART is designed for average baud rates; the range of supported rates is 7813 baud to 2.4 Mbaud, though communication will be less reliable above 1-2 Mbaud.

Transmitting data is straightforward: simply pass a byte array --- up to 63 characters long --- to the Send() function once the UART is enabled.

Receiving data is more challenging, since incoming data can appear on the RX pin at any moment when the UART is enabled. Since all actions on Treehopper are initiated on the host, to get around UART's inherent asynchronicity, a 32-byte buffer holds any received data that comes in while the UART is enabled. Then, when the host wants to access this data, it can Receive() it from the board to obtain the buffer.

Whenever Receive() is called, the entire buffer is sent to the host, and the buffer's pointer is reset to 0 (i.e., the buffer is reset). This can be useful for clearing out any gibberish and returning the UART to a known state before you expect to receive data --- for example, if you're addressing a device that you send commands to, and read responses back from, you may wish to call Receive() before sending the command; that way, parsing the received data will be simpler.

## Other Considerations
This ping-pong short-packet-oriented back-and-forth scenario is what Treehopper's UART is built for, as it's what's most commonly needed when interfacing with embedded devices that use a UART. 

There is a tight window of possible baud rates where it is plausible to receive data continuously without interruption. For example, at 9600 baud, the Receive() function only need to finish execution every 33 milliseconds, which can easily be accomplished in most operating systems. However, because data is not double-buffered on the board, under improbable circumstances, continuously-transmitted data may inadvertently be discarded.

Treehopper's UART is not designed to replace a high-quality CDC-class USB-to-serial converter, especially for high data-rate applications. In addition to streaming large volumes of data continuously, USB CDC-class UARTs should also offer lower latency for receiving data. Treehopper also has no way of exposing its UART to the operating system as a COM port, so it's most certainly not a suitable replacement for a USB-to-serial converter in most applications.
*/

    public class HardwareUart : Uart, OneWire
    {
        private readonly TreehopperUsb _device;
        private int _baud = 9600;
        private bool _isEnabled;
        private UartMode _mode = UartMode.Uart;
        private bool _useOpenDrainTx;

        internal HardwareUart(TreehopperUsb device)
        {
            _device = device;
        }

        /// <summary>
        ///     Gets or sets the UART mode
        /// </summary>
        public UartMode Mode
        {
            get { return _mode; }

            set
            {
                if (_mode == value)
                    return;

                _mode = value;
                UpdateConfigAsync();
            }
        }

        /// <summary>
        ///     Enable or disable the UART
        /// </summary>
        public bool Enabled
        {
            get { return _isEnabled; }

            set
            {
                if (_isEnabled == value)
                    return;

                _isEnabled = value;
                Task.Run(UpdateConfigAsync).Wait();
            }
        }

        /// <summary>
        ///     Set or get the baud of the UART.
        /// </summary>
        /// 
        /// Baud can range from 7813 - 2400000 baud, but values less than 2000000 (2 Mbaud) are recommended.
        public int Baud
        {
            get { return _baud; }

            set
            {
                if (_baud == value)
                    return;

                _baud = value;

                Task.Run(UpdateConfigAsync).Wait();
            }
        }

        /// <summary>
        ///     Whether to use an open-drain TX pin or not.
        /// </summary>
        public bool UseOpenDrainTx
        {
            get { return _useOpenDrainTx; }

            set
            {
                if (_useOpenDrainTx == value)
                    return;

                _useOpenDrainTx = value;

                Task.Run(UpdateConfigAsync).Wait();
            }
        }

        /// <summary>
        ///     Send a byte out of the UART
        /// </summary>
        /// <param name="data">The byte to send</param>
        /// <returns>An awaitable task that completes upon transmission of the byte</returns>
        public Task SendAsync(byte data)
        {
            return SendAsync(new[] {data});
        }

        /// <summary>
        ///     Send data
        /// </summary>
        /// <param name="dataToSend">The data to send</param>
        /// <returns>An awaitable task that completes upon transmission of the data</returns>
        public async Task SendAsync(byte[] dataToSend)
        {
            if (dataToSend.Length > 63)
                throw new Exception("The maximum UART length for one transaction is 63 bytes");

            var data = new byte[dataToSend.Length + 3];
            data[0] = (byte) DeviceCommands.UartTransaction;
            data[1] = (byte) UartCommand.Transmit;
            data[2] = (byte) dataToSend.Length;
            dataToSend.CopyTo(data, 3);
            using (await _device.ComsLock.LockAsync().ConfigureAwait(false))
            {
                await _device.SendPeripheralConfigPacketAsync(data).ConfigureAwait(false);
                await _device.ReceiveCommsResponsePacketAsync(1).ConfigureAwait(false);
            }
        }

        /// <summary>
        ///     Receive bytes from the UART in UART or One-Wire mode
        /// </summary>
        /// <param name="oneWireNumBytes">In One-Wire mode, the number of bytes to receive. Not used in UART mode.</param>
        /// <returns>The bytes received</returns>
        /// As soon as the UART is enabled, any received byte will be added to a 32-byte buffer. Calling this Receive() function does two things:
        ///    - sends the current contents of this buffer to this function.
        ///    - reset the pointer in the buffer to the 0th element, effectively resetting it.
        /// If the buffer fills before the Receive() function is called, the existing buffer will be reset --- discarding all data in the buffer.
        /// Consequently, it's important to call the Receive() function frequently when expecting data. 
        /// 
        /// Owing to how it is implemented, you can clear the buffer at any point by calling Receive(). It's common to empty the buffer before 
        /// requesting data from the device attached to the UART; this way, you do not have to worry about existing gibberish data that
        /// might have been inadvertently received.
        public async Task<byte[]> ReceiveAsync(int oneWireNumBytes)
        {
            byte[] retVal;
            if (_mode == UartMode.Uart)
            {
                if(oneWireNumBytes != 0)
                    Utilities.Utility.Error("Since the UART is not in One-Wire Mode, the oneWireNumBytes parameter is ignored");
                var data = new byte[2];
                data[0] = (byte) DeviceCommands.UartTransaction;
                data[1] = (byte) UartCommand.Receive;

                using (await _device.ComsLock.LockAsync().ConfigureAwait(false))
                {
                    await _device.SendPeripheralConfigPacketAsync(data).ConfigureAwait(false);
                    var receivedData = await _device.ReceiveCommsResponsePacketAsync(33).ConfigureAwait(false);
                    int len = receivedData[32];
                    retVal = new byte[len];
                    Array.Copy(receivedData, retVal, len);
                }
            }
            else
            {
                if (oneWireNumBytes == 0)
                    throw new Exception("You must specify the number of bytes to receive in One-Wire Mode");
                var data = new byte[3];
                data[0] = (byte) DeviceCommands.UartTransaction;
                data[1] = (byte) UartCommand.Receive;
                data[2] = (byte) oneWireNumBytes;

                using (await _device.ComsLock.LockAsync().ConfigureAwait(false))
                {
                    await _device.SendPeripheralConfigPacketAsync(data).ConfigureAwait(false);
                    var receivedData = await _device.ReceiveCommsResponsePacketAsync(33).ConfigureAwait(false);
                    int len = receivedData[32];
                    retVal = new byte[len];
                    Array.Copy(receivedData, retVal, len);
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Reset the One Wire bus
        /// </summary>
        /// <returns>True if at least one device was found. False otherwise.</returns>
        public async Task<bool> OneWireResetAsync()
        {
            Mode = UartMode.OneWire;
            Enabled = true;
            bool retVal;
            var data = new byte[2];
            data[0] = (byte) DeviceCommands.UartTransaction;
            data[1] = (byte) UartCommand.OneWireReset;
            using (await _device.ComsLock.LockAsync().ConfigureAwait(false))
            {
                await _device.SendPeripheralConfigPacketAsync(data).ConfigureAwait(false);
                var receivedData = await _device.ReceiveCommsResponsePacketAsync(1).ConfigureAwait(false);
                retVal = receivedData[0] > 0;
            }

            return retVal;
        }

        /// <summary>
        ///     Search for One Wire devices on the bus
        /// </summary>
        /// <returns>A list of addresses found</returns>
        public async Task<List<ulong>> OneWireSearchAsync()
        {
            Mode = UartMode.OneWire;
            Enabled = true;
            var retVal = new List<ulong>();

            var data = new byte[2];
            data[0] = (byte) DeviceCommands.UartTransaction;
            data[1] = (byte) UartCommand.OneWireScan;
            using (await _device.ComsLock.LockAsync().ConfigureAwait(false))
            {
                await _device.SendPeripheralConfigPacketAsync(data).ConfigureAwait(false);
                while (true)
                {
                    var receivedData = await _device.ReceiveCommsResponsePacketAsync(9).ConfigureAwait(false);
                    if (receivedData[0] == 0xff)
                        break;

                    Array.Reverse(receivedData); // endian conversion
                    retVal.Add(BitConverter.ToUInt64(receivedData, 0));
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Reset and match a device on the One-Wire bus
        /// </summary>
        /// <param name="address">The address to reset and match</param>
        /// <returns></returns>
        public async Task OneWireResetAndMatchAddressAsync(ulong address)
        {
            Mode = UartMode.OneWire;
            Enabled = true;
            await OneWireResetAsync().ConfigureAwait(false);
            var addr = BitConverter.GetBytes(address);
            var data = new byte[9];
            data[0] = 0x55; // MATCH ROM
            Array.Copy(addr, 0, data, 1, 8);
            await SendAsync(data).ConfigureAwait(false);
        }

        /// <summary>
        ///     Start one-wire mode on this interface
        /// </summary>
        public Task StartOneWireAsync()
        {
            _mode = UartMode.OneWire;
            _isEnabled = true;
            return UpdateConfigAsync();
        }

        /// <summary>
        ///     Gets a string representing the UART's state
        /// </summary>
        /// <returns>the UART's string</returns>
        public override string ToString()
        {
            if (Enabled)
                return $"{Mode}, running at {Baud:0.00} baud";
            return "Not enabled";
        }

        private async Task UpdateConfigAsync()
        {
            if (!_isEnabled)
            {
                await _device.SendPeripheralConfigPacketAsync(new[] {(byte) DeviceCommands.UartConfig, (byte) UartConfig.Disabled}).ConfigureAwait(false);
            }
            else if (_mode == UartMode.Uart)
            {
                byte timerVal;
                bool usePrescaler;

                // calculate baud with and without prescaler
                var timerValPrescaler = (int) Math.Round(256.0 - 2000000.0 / _baud);
                var timerValNoPrescaler = (int) Math.Round(256.0 - 24000000.0 / _baud);

                var prescalerOutOfBounds = timerValPrescaler > 255 || timerValPrescaler < 0;
                var noPrescalerOutOfBounds = timerValNoPrescaler > 255 || timerValNoPrescaler < 0;

                // calculate error
                double prescalerError = Math.Abs(_baud - 2000000 / (256 - timerValPrescaler));
                double noPrescalerError = Math.Abs(_baud - 24000000 / (256 - timerValNoPrescaler));

                if (prescalerOutOfBounds && noPrescalerOutOfBounds)
                    throw new Exception("The specified baud rate is out of bounds.");
                if (prescalerOutOfBounds)
                {
                    usePrescaler = false;
                    timerVal = (byte) timerValNoPrescaler;
                }
                else if (noPrescalerOutOfBounds)
                {
                    usePrescaler = true;
                    timerVal = (byte) timerValPrescaler;
                }
                else if (prescalerError > noPrescalerError)
                {
                    usePrescaler = false;
                    timerVal = (byte) timerValNoPrescaler;
                }
                else
                {
                    usePrescaler = true;
                    timerVal = (byte) timerValPrescaler;
                }

                var data = new byte[5];
                data[0] = (byte) DeviceCommands.UartConfig;
                data[1] = (byte) UartConfig.Standard;
                data[2] = timerVal;
                data[3] = (byte) (usePrescaler ? 0x01 : 0x00);
                data[4] = (byte) (_useOpenDrainTx ? 0x01 : 0x00);
                await _device.SendPeripheralConfigPacketAsync(data).ConfigureAwait(false);
            }
            else
            {
                var data = new byte[2];
                data[0] = (byte) DeviceCommands.UartConfig;
                data[1] = (byte) UartConfig.OneWire;
                await _device.SendPeripheralConfigPacketAsync(data).ConfigureAwait(false);
            }
        }

        /// <summary>
        ///     Receive bytes from the UART in UART mode
        /// </summary>
        /// <returns>The bytes received</returns>
        /// As soon as the UART is enabled, any received byte will be added to a 32-byte buffer. Calling this Receive() function does two things:
        ///    - sends the current contents of this buffer to this function.
        ///    - reset the pointer in the buffer to the 0th element, effectively resetting it.
        /// If the buffer fills before the Receive() function is called, the existing buffer will be reset --- discarding all data in the buffer.
        /// Consequently, it's important to call the Receive() function frequently when expecting data. 
        /// 
        /// Owing to how it is implemented, you can clear the buffer at any point by calling Receive(). It's common to empty the buffer before 
        /// requesting data from the device attached to the UART; this way, you do not have to worry about existing gibberish data that
        /// might have been inadvertently received.
        public Task<byte[]> ReceiveAsync()
        {
            return ReceiveAsync(0);
        }

        /// <summary>
        /// Start the UART with the specified baud
        /// </summary>
        /// <param name="baud">The baud, in bps, to use</param>
        public Task StartUartAsync(int baud)
        {
            _baud = baud;
            _mode = UartMode.Uart;
            _isEnabled = true;
            return UpdateConfigAsync();
        }

        private enum UartConfig : byte
        {
            Disabled,
            Standard,
            OneWire
        }

        private enum UartCommand : byte
        {
            Transmit,
            Receive,
            OneWireReset,
            OneWireScan
        }
    }
}