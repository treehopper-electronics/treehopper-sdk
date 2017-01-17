using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Treehopper
{
    /// <summary>
    /// Whether to use UART or OneWire mode
    /// </summary>
    public enum UartMode
    {
        /// <summary>
        /// The module is operating in UART mode
        /// </summary>
        Uart,

        /// <summary>
        /// The module is operating in OneWire mode. Only the TX pin is used.
        /// </summary>
        OneWire
    }

    /// <summary>
    /// Module that implements hardware UART and OneWire functionality
    /// </summary>
    public class Uart : IOneWire
    {
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


        internal Uart(TreehopperUsb device)
        {
            this.device = device;
        }

        private bool isEnabled;
        private int baud = 9600;
        private TreehopperUsb device;

        private UartMode mode = UartMode.Uart;

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
        /// Get the actual baud rate currently used
        /// </summary>
        public double ActualBaud { get; private set; }

        private bool useOpenDrainTx = false;

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
        

        private void UpdateConfig()
        {
            if (!isEnabled)
            {
                device.sendPeripheralConfigPacket(new byte[] { (byte)DeviceCommands.UartConfig, (byte)UartConfig.Disabled });
            }
            else if(mode == UartMode.Uart)
            {
                byte timerVal = 0;
                bool usePrescaler = false;
                // calculate baud with and without prescaler
                int timerValPrescaler = (int)Math.Round(256.0 - 2000000.0 / baud);
                int timerValNoPrescaler = (int)Math.Round(256.0 - 24000000.0 / baud);

                bool prescalerOutOfBounds = timerValPrescaler > 255 || timerValPrescaler < 0;
                bool noPrescalerOutOfBounds = timerValNoPrescaler > 255 || timerValNoPrescaler < 0;

                // calculate error
                double prescalerError = Math.Abs(baud - 2000000 / (256 - timerValPrescaler));
                double noPrescalerError = Math.Abs(baud - 24000000 / (256 - timerValNoPrescaler));

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

                byte[] data = new byte[5];
                data[0] = (byte)DeviceCommands.UartConfig;
                data[1] = (byte)UartConfig.Standard;
                data[2] = timerVal;
                data[3] = (byte)(usePrescaler ? 0x01 : 0x00);
                data[4] = (byte)(useOpenDrainTx ? 0x01 : 0x00);
                device.sendPeripheralConfigPacket(data);
            }
            else
            {
                byte[] data = new byte[2];
                data[0] = (byte)DeviceCommands.UartConfig;
                data[1] = (byte)UartConfig.OneWire;
                device.sendPeripheralConfigPacket(data);
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
            if(dataToSend.Length > 63)
            {
                throw new Exception("The maximum UART length for one transaction is 63 bytes");
            }

            byte[] data = new byte[dataToSend.Length + 3];
            data[0] = (byte)DeviceCommands.UartTransaction;
            data[1] = (byte)UartCommand.Transmit;
            data[2] = (byte)dataToSend.Length;
            dataToSend.CopyTo(data, 3);
            using (await device.ComsLock.LockAsync().ConfigureAwait(false))
            {
                device.sendPeripheralConfigPacket(data);
                byte[] receivedData = await device.receiveCommsResponsePacket(1).ConfigureAwait(false);
            }
        }

        
        /// <summary>
        /// Receive bytes from the UART
        /// </summary>
        /// <param name="numBytes">The number of bytes to receive</param>
        /// <returns>The bytes received</returns>
        public async Task<byte[]> Receive(int numBytes = 0)
        {
            byte[] retVal = new byte[0];
            if (mode == UartMode.Uart)
            {
                byte[] data = new byte[2];
                data[0] = (byte)DeviceCommands.UartTransaction;
                data[1] = (byte)UartCommand.Receive;

                using (await device.ComsLock.LockAsync().ConfigureAwait(false))
                {
                    device.sendPeripheralConfigPacket(data);
                    byte[] receivedData = await device.receiveCommsResponsePacket(33).ConfigureAwait(false);
                    int len = receivedData[32];
                    retVal = new byte[len];
                    Array.Copy(receivedData, retVal, len);
                }
            }
            else
            {
                byte[] data = new byte[3];
                data[0] = (byte)DeviceCommands.UartTransaction;
                data[1] = (byte)UartCommand.Receive;
                data[2] = (byte)numBytes;

                using (await device.ComsLock.LockAsync().ConfigureAwait(false))
                {
                    device.sendPeripheralConfigPacket(data);
                    byte[] receivedData = await device.receiveCommsResponsePacket(33).ConfigureAwait(false);
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
            bool retVal = false;
            byte[] data = new byte[2];
            data[0] = (byte)DeviceCommands.UartTransaction;
            data[1] = (byte)UartCommand.OneWireReset;
            using (await device.ComsLock.LockAsync().ConfigureAwait(false))
            {
                device.sendPeripheralConfigPacket(data);
                byte[] receivedData = await device.receiveCommsResponsePacket(1).ConfigureAwait(false);
                retVal = receivedData[0] > 0 ? true : false;
            }
            return retVal;
        }

        /// <summary>
        /// Search for One Wire devices on the bus
        /// </summary>
        /// <returns>A list of addresses found</returns>
        public async Task<List<UInt64>> OneWireSearch()
        {
            Mode = UartMode.OneWire;
            Enabled = true;
            List<UInt64> retVal = new List<UInt64>();
            //if (!await OneWireReset())
            //    return new List<UInt64>();
            byte[] data = new byte[2];
            data[0] = (byte)DeviceCommands.UartTransaction;
            data[1] = (byte)UartCommand.OneWireScan;
            using (await device.ComsLock.LockAsync().ConfigureAwait(false))
            {
                device.sendPeripheralConfigPacket(data);
                byte[] receivedData = new byte[8];
                while (true)
                {
                    receivedData = await device.receiveCommsResponsePacket(9).ConfigureAwait(false);
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
        public async Task OneWireResetAndMatchAddress(UInt64 address)
        {
            Mode = UartMode.OneWire;
            Enabled = true;
            await OneWireReset().ConfigureAwait(false);
            byte[] addr = BitConverter.GetBytes(address);
            //Array.Reverse(addr); // endian conversion
            byte[] data = new byte[9];
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

        public override string ToString()
        {
            if (Enabled)
                return string.Format("{0}, running at {1:0.00} baud", Mode, Baud);
            else
                return "Not enabled";
        }
    }
}
