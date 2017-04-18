using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Treehopper
{
    /// <summary>
    ///     Module that implements hardware UART and OneWire functionality
    /// </summary>
    public class HardwareUart : IOneWire
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
                UpdateConfig();
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
                UpdateConfig();
            }
        }

        /// <summary>
        ///     Set or get the baud of the UART
        /// </summary>
        public int Baud
        {
            get { return _baud; }

            set
            {
                if (_baud == value)
                    return;

                _baud = value;

                UpdateConfig();
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

                UpdateConfig();
            }
        }

        /// <summary>
        ///     Send a byte out of the UART
        /// </summary>
        /// <param name="data">The byte to send</param>
        /// <returns>An awaitable task that completes upon transmission of the byte</returns>
        public Task Send(byte data)
        {
            return Send(new[] {data});
        }

        /// <summary>
        ///     Send data
        /// </summary>
        /// <param name="dataToSend">The data to send</param>
        /// <returns>An awaitable task that completes upon transmission of the data</returns>
        public async Task Send(byte[] dataToSend)
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
                await _device.SendPeripheralConfigPacket(data);
                await _device.ReceiveCommsResponsePacket(1).ConfigureAwait(false);
            }
        }

        /// <summary>
        ///     Receive bytes from the UART
        /// </summary>
        /// <param name="numBytes">The number of bytes to receive</param>
        /// <returns>The bytes received</returns>
        public async Task<byte[]> Receive(int numBytes = 0)
        {
            byte[] retVal;
            if (_mode == UartMode.Uart)
            {
                var data = new byte[2];
                data[0] = (byte) DeviceCommands.UartTransaction;
                data[1] = (byte) UartCommand.Receive;

                using (await _device.ComsLock.LockAsync().ConfigureAwait(false))
                {
                    await _device.SendPeripheralConfigPacket(data);
                    var receivedData = await _device.ReceiveCommsResponsePacket(33).ConfigureAwait(false);
                    int len = receivedData[32];
                    retVal = new byte[len];
                    Array.Copy(receivedData, retVal, len);
                }
            }
            else
            {
                var data = new byte[3];
                data[0] = (byte) DeviceCommands.UartTransaction;
                data[1] = (byte) UartCommand.Receive;
                data[2] = (byte) numBytes;

                using (await _device.ComsLock.LockAsync().ConfigureAwait(false))
                {
                    await _device.SendPeripheralConfigPacket(data);
                    var receivedData = await _device.ReceiveCommsResponsePacket(33).ConfigureAwait(false);
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
        public async Task<bool> OneWireReset()
        {
            Mode = UartMode.OneWire;
            Enabled = true;
            if (_mode != UartMode.OneWire)
                throw new Exception("The UART must be in OneWire mode to issue a OneWireReset command");
            bool retVal;
            var data = new byte[2];
            data[0] = (byte) DeviceCommands.UartTransaction;
            data[1] = (byte) UartCommand.OneWireReset;
            using (await _device.ComsLock.LockAsync().ConfigureAwait(false))
            {
                await _device.SendPeripheralConfigPacket(data);
                var receivedData = await _device.ReceiveCommsResponsePacket(1).ConfigureAwait(false);
                retVal = receivedData[0] > 0;
            }

            return retVal;
        }

        /// <summary>
        ///     Search for One Wire devices on the bus
        /// </summary>
        /// <returns>A list of addresses found</returns>
        public async Task<List<ulong>> OneWireSearch()
        {
            Mode = UartMode.OneWire;
            Enabled = true;
            var retVal = new List<ulong>();

            var data = new byte[2];
            data[0] = (byte) DeviceCommands.UartTransaction;
            data[1] = (byte) UartCommand.OneWireScan;
            using (await _device.ComsLock.LockAsync().ConfigureAwait(false))
            {
                await _device.SendPeripheralConfigPacket(data);
                while (true)
                {
                    var receivedData = await _device.ReceiveCommsResponsePacket(9).ConfigureAwait(false);
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
        public async Task OneWireResetAndMatchAddress(ulong address)
        {
            Mode = UartMode.OneWire;
            Enabled = true;
            await OneWireReset().ConfigureAwait(false);
            var addr = BitConverter.GetBytes(address);
            var data = new byte[9];
            data[0] = 0x55; // MATCH ROM
            Array.Copy(addr, 0, data, 1, 8);
            await Send(data).ConfigureAwait(false);
        }

        /// <summary>
        ///     Start one-wire mode on this interface
        /// </summary>
        public void StartOneWire()
        {
            Mode = UartMode.OneWire;
            Enabled = true;
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

        private void UpdateConfig()
        {
            if (!_isEnabled)
            {
                _device.SendPeripheralConfigPacket(new[] {(byte) DeviceCommands.UartConfig, (byte) UartConfig.Disabled});
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
                    throw new Exception("The specified baud rate was out of bounds.");
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
                _device.SendPeripheralConfigPacket(data);
            }
            else
            {
                var data = new byte[2];
                data[0] = (byte) DeviceCommands.UartConfig;
                data[1] = (byte) UartConfig.OneWire;
                _device.SendPeripheralConfigPacket(data);
            }
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