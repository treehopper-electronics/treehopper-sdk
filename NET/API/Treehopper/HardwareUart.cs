namespace Treehopper
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Module that implements hardware UART and OneWire functionality
    /// </summary>
    public class HardwareUart : IOneWire
    {
        private bool isEnabled;
        private int baud = 9600;
        private readonly TreehopperUsb device;
        private UartMode mode = UartMode.Uart;
        private bool useOpenDrainTx = false;

        internal HardwareUart(TreehopperUsb device)
        {
            this.device = device;
        }

        private enum UartConfig : byte
        {
            Disabled,
            Standard,
            OneWire,
        }

        private enum UartCommand : byte
        {
            Transmit,
            Receive,
            OneWireReset,
            OneWireScan
        }

        /// <summary>
        /// Gets or sets the UART mode
        /// </summary>
        public UartMode Mode
        {
            get
            {
                return mode;
            }

            set
            {
                if (mode == value)
                    return;

                mode = value;
                UpdateConfig();
            }
        }

        /// <summary>
        /// Enable or disable the UART
        /// </summary>
        public bool Enabled
        {
            get
            {
                return isEnabled;
            }

            set
            {
                if (isEnabled == value)
                    return;

                isEnabled = value;
                UpdateConfig();
            }
        }

        /// <summary>
        /// Set or get the baud of the UART
        /// </summary>
        public int Baud
        {
            get
            {
                return baud;
            }

            set
            {
                if (baud == value)
                    return;

                baud = value;

                UpdateConfig();
            }
        }

        /// <summary>
        /// Whether to use an open-drain TX pin or not.
        /// </summary>
        public bool UseOpenDrainTx
        {
            get
            {
                return useOpenDrainTx;
            }

            set
            {
                if (useOpenDrainTx == value)
                    return;

                useOpenDrainTx = value;

                UpdateConfig();
            }
        }

        /// <summary>
        /// Send a byte out of the UART
        /// </summary>
        /// <param name="data">The byte to send</param>
        /// <returns>An awaitable task that completes upon transmission of the byte</returns>
        public Task Send(byte data)
        {
            return Send(new byte[] { data });
        }

        /// <summary>
        /// Send data
        /// </summary>
        /// <param name="dataToSend">The data to send</param>
        /// <returns>An awaitable task that completes upon transmission of the data</returns>
        public async Task Send(byte[] dataToSend)
        {
            if (dataToSend.Length > 63)
            {
                throw new Exception("The maximum UART length for one transaction is 63 bytes");
            }

            var data = new byte[dataToSend.Length + 3];
            data[0] = (byte)DeviceCommands.UartTransaction;
            data[1] = (byte)UartCommand.Transmit;
            data[2] = (byte)dataToSend.Length;
            dataToSend.CopyTo(data, 3);
            using (await device.ComsLock.LockAsync().ConfigureAwait(false))
            {
                await device.SendPeripheralConfigPacket(data);
                var receivedData = await device.ReceiveCommsResponsePacket(1).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Receive bytes from the UART
        /// </summary>
        /// <param name="numBytes">The number of bytes to receive</param>
        /// <returns>The bytes received</returns>
        public async Task<byte[]> Receive(int numBytes = 0)
        {
            var retVal = new byte[0];
            if (mode == UartMode.Uart)
            {
                var data = new byte[2];
                data[0] = (byte)DeviceCommands.UartTransaction;
                data[1] = (byte)UartCommand.Receive;

                using (await device.ComsLock.LockAsync().ConfigureAwait(false))
                {
                    await device.SendPeripheralConfigPacket(data);
                    var receivedData = await device.ReceiveCommsResponsePacket(33).ConfigureAwait(false);
                    int len = receivedData[32];
                    retVal = new byte[len];
                    Array.Copy(receivedData, retVal, len);
                }
            }
            else
            {
                var data = new byte[3];
                data[0] = (byte)DeviceCommands.UartTransaction;
                data[1] = (byte)UartCommand.Receive;
                data[2] = (byte)numBytes;

                using (await device.ComsLock.LockAsync().ConfigureAwait(false))
                {
                    await device.SendPeripheralConfigPacket(data);
                    var receivedData = await device.ReceiveCommsResponsePacket(33).ConfigureAwait(false);
                    int len = receivedData[32];
                    retVal = new byte[len];
                    Array.Copy(receivedData, retVal, len);
                }
            }

            return retVal;
        }

        /// <summary>
        /// Reset the One Wire bus
        /// </summary>
        /// <returns>True if at least one device was found. False otherwise.</returns>
        public async Task<bool> OneWireReset()
        {
            Mode = UartMode.OneWire;
            Enabled = true;
            if (mode != UartMode.OneWire)
                throw new Exception("The UART must be in OneWire mode to issue a OneWireReset command");
            var retVal = false;
            var data = new byte[2];
            data[0] = (byte)DeviceCommands.UartTransaction;
            data[1] = (byte)UartCommand.OneWireReset;
            using (await device.ComsLock.LockAsync().ConfigureAwait(false))
            {
                await device.SendPeripheralConfigPacket(data);
                var receivedData = await device.ReceiveCommsResponsePacket(1).ConfigureAwait(false);
                retVal = receivedData[0] > 0 ? true : false;
            }

            return retVal;
        }

        /// <summary>
        /// Search for One Wire devices on the bus
        /// </summary>
        /// <returns>A list of addresses found</returns>
        public async Task<List<ulong>> OneWireSearch()
        {
            Mode = UartMode.OneWire;
            Enabled = true;
            var retVal = new List<ulong>();

            var data = new byte[2];
            data[0] = (byte)DeviceCommands.UartTransaction;
            data[1] = (byte)UartCommand.OneWireScan;
            using (await device.ComsLock.LockAsync().ConfigureAwait(false))
            {
                await device.SendPeripheralConfigPacket(data);
                var receivedData = new byte[8];
                while (true)
                {
                    receivedData = await device.ReceiveCommsResponsePacket(9).ConfigureAwait(false);
                    if (receivedData[0] == 0xff)
                        break;

                    Array.Reverse(receivedData); // endian conversion
                    retVal.Add(BitConverter.ToUInt64(receivedData, 0));
                }
            }

            return retVal;
        }

        /// <summary>
        /// Reset and match a device on the One-Wire bus
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
        /// Start one-wire mode on this interface
        /// </summary>
        public void StartOneWire()
        {
            Mode = UartMode.OneWire;
            Enabled = true;
        }

        /// <summary>
        /// Gets a string representing the UART's state
        /// </summary>
        /// <returns>the UART's string</returns>
        public override string ToString()
        {
            if (Enabled)
                return $"{Mode}, running at {Baud:0.00} baud";
            else
                return "Not enabled";
        }

        private void UpdateConfig()
        {
            if (!isEnabled)
            {
                device.SendPeripheralConfigPacket(new byte[] { (byte)DeviceCommands.UartConfig, (byte)UartConfig.Disabled });
            }
            else if (mode == UartMode.Uart)
            {
                byte timerVal = 0;
                var usePrescaler = false;

                // calculate baud with and without prescaler
                var timerValPrescaler = (int)Math.Round(256.0 - (2000000.0 / baud));
                var timerValNoPrescaler = (int)Math.Round(256.0 - (24000000.0 / baud));

                var prescalerOutOfBounds = timerValPrescaler > 255 || timerValPrescaler < 0;
                var noPrescalerOutOfBounds = timerValNoPrescaler > 255 || timerValNoPrescaler < 0;

                // calculate error
                double prescalerError = Math.Abs(baud - (2000000 / (256 - timerValPrescaler)));
                double noPrescalerError = Math.Abs(baud - (24000000 / (256 - timerValNoPrescaler)));

                if (prescalerOutOfBounds && noPrescalerOutOfBounds)
                {
                    throw new Exception("The specified baud rate was out of bounds.");
                }
                else if (prescalerOutOfBounds)
                {
                    usePrescaler = false;
                    timerVal = (byte)timerValNoPrescaler;
                }
                else if (noPrescalerOutOfBounds)
                {
                    usePrescaler = true;
                    timerVal = (byte)timerValPrescaler;
                }
                else if (prescalerError > noPrescalerError)
                {
                    usePrescaler = false;
                    timerVal = (byte)timerValNoPrescaler;
                }
                else
                {
                    usePrescaler = true;
                    timerVal = (byte)timerValPrescaler;
                }

                var data = new byte[5];
                data[0] = (byte)DeviceCommands.UartConfig;
                data[1] = (byte)UartConfig.Standard;
                data[2] = timerVal;
                data[3] = (byte)(usePrescaler ? 0x01 : 0x00);
                data[4] = (byte)(useOpenDrainTx ? 0x01 : 0x00);
                device.SendPeripheralConfigPacket(data);
            }
            else
            {
                var data = new byte[2];
                data[0] = (byte)DeviceCommands.UartConfig;
                data[1] = (byte)UartConfig.OneWire;
                device.SendPeripheralConfigPacket(data);
            }
        }
    }
}
