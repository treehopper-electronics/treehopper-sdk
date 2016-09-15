using Nito.AsyncEx;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Treehopper
{

    //public enum I2cMode { Master, Slave };

    /// <summary>
    /// The I2c class is used for interacting with the I2C module on the Treehopper Board. 
    /// </summary>
    public class I2c : II2c
    {
        TreehopperUsb device;

        internal I2c(TreehopperUsb device)
        {
            this.device = device;
        }

        private bool enabled;
        public bool Enabled
        {
            get { return enabled; }
            set
            {
                if (value == enabled)
                    return;
                enabled = value;
                SendConfig();
            }
        }

        double speed = 100;
        public double Speed
        {
            get
            {
                return speed;
            }
            set
            {
                if (speed == value)
                    return;
                speed = value;
                SendConfig();
            }
        }

        /// <summary>
        /// Start I2C communication with the given mode and rate. SDA is on Pin 10, SCL is on pin 11
        /// </summary>
        /// <param name="mode">Master or slave mode</param>
        /// <param name="rate">Communication rate, in kHz.</param>
        private void SendConfig()
        {
            double TH0 = 256 - 4000 / (3*speed);
            if(TH0 < 0 || TH0 > 255)
            {
                throw new Exception("Rate out of limits. Valid rate is 62.5 kHz - 16000 kHz (16 MHz)");
            }
            byte[] dataToSend = new byte[3];
            dataToSend[0] = (byte)DeviceCommands.I2cConfig;
            dataToSend[1] = (byte)(enabled ? 0x01 : 0x00);
            dataToSend[2] = (byte)Math.Round(TH0);
            device.sendPeripheralConfigPacket(dataToSend);
        }

        /// <summary>
        /// Sends and Receives data. This is a blocking call that won't return until I2C communication is complete.
        /// </summary>
        /// <param name="address">The address of the device. This address should not include the read/write bit.</param>
        /// <param name="dataToWrite">Array of one or more bytes to write to the device.</param>
        /// <param name="numBytesToRead">Number of bytes to receive from the device.</param>
        /// <returns>Data read from the device.</returns>
        public async Task<byte[]> SendReceive(byte address, byte[] dataToWrite, byte numBytesToRead, BurstMode mode = BurstMode.NoBurst)
        {
            byte[] returnedData = new byte[numBytesToRead];
            int txLen = dataToWrite.Length;
            using (await device.ComsMutex.LockAsync())
            {
                byte[] receivedData;
                int srcIndex = 0;
                int bytesRemaining = txLen;
                while (bytesRemaining > 0)
                {
                    int numBytesToTransfer = bytesRemaining > 57 ? 57 : bytesRemaining;
                    byte[] dataToSend = new byte[7 + numBytesToTransfer]; // 2 bytes for the header
                    dataToSend[0] = (byte)DeviceCommands.I2cTransaction;
                    dataToSend[1] = address;
                    dataToSend[2] = (byte)txLen; // total length (0-255)
                    dataToSend[3] = (byte)srcIndex; // offset
                    dataToSend[4] = (byte)numBytesToTransfer; // length of this packet
                    dataToSend[5] = numBytesToRead;
                    dataToSend[6] = (byte)mode;
                    Array.Copy(dataToWrite, srcIndex, dataToSend, 7, numBytesToTransfer);
                    device.sendPeripheralConfigPacket(dataToSend);
                    srcIndex += numBytesToTransfer;
                    bytesRemaining -= numBytesToTransfer;
                    if (mode == BurstMode.BurstRx) // don't send additional data, just wait for read
                        break;
                }

                // no need to wait if we're not reading anything
                if (mode == BurstMode.BurstTx)
                    return returnedData;

                bytesRemaining = numBytesToRead;
                srcIndex = 0;
                while (bytesRemaining > 0)
                {
                    int numBytesToTransfer = bytesRemaining > 64 ? 64 : bytesRemaining;
                    receivedData = await device.receiveCommsResponsePacket((uint)numBytesToTransfer);
                    Array.Copy(receivedData, 0, returnedData, srcIndex, numBytesToTransfer);
                    srcIndex += numBytesToTransfer;
                    bytesRemaining -= numBytesToTransfer;
                }

                return returnedData;
            }

        }
    }
}
