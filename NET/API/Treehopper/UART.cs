
using System;
using System.Threading.Tasks;
using Nito.AsyncEx;
using System.Collections.Generic;

namespace Treehopper
{

    public enum UartMode
    {
        Uart,
        OneWire
    }

    public class Uart
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

        public bool IsEnabled
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

        public double ActualBaud { get; set; }

        private bool useOpenDrainTx = false;

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
            using (await mutex.LockAsync())
            {
                device.sendPeripheralConfigPacket(data);
                byte[] receivedData = await device.receiveCommsResponsePacket(1);
            }
        }

        private readonly AsyncLock mutex = new AsyncLock();

        public async Task<byte[]> Receive(int numBytes = 0)
        {
            byte[] retVal = new byte[0];
            if (mode == UartMode.Uart)
            {
                byte[] data = new byte[2];
                data[0] = (byte)DeviceCommands.UartTransaction;
                data[1] = (byte)UartCommand.Receive;

                using (await mutex.LockAsync())
                {
                    device.sendPeripheralConfigPacket(data);
                    byte[] receivedData = await device.receiveCommsResponsePacket(33);
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

                using (await mutex.LockAsync())
                {
                    device.sendPeripheralConfigPacket(data);
                    byte[] receivedData = await device.receiveCommsResponsePacket(33);
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
            if (mode != UartMode.OneWire)
                throw new Exception("The UART must be in OneWire mode to issue a OneWireReset command");
            bool retVal = false;
            byte[] data = new byte[2];
            data[0] = (byte)DeviceCommands.UartTransaction;
            data[1] = (byte)UartCommand.OneWireReset;
            using (await mutex.LockAsync())
            {
                device.sendPeripheralConfigPacket(data);
                byte[] receivedData = await device.receiveCommsResponsePacket(1);
                retVal = receivedData[0] != 0xf0;
            }
            return retVal;
        }

        public async Task<List<UInt64>> OneWireScan()
        {
            if (!await OneWireReset())
                return new List<UInt64>();


            throw new NotImplementedException();
        }
    }
}
