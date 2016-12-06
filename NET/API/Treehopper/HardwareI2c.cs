using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Treehopper
{
    public enum I2cTransferError
    {
        ArbitrationLostError,
        NackError,
        UnknownError,
        TxunderError,
        Success = 0x255
    }
    //public enum I2cMode { Master, Slave };

    /// <summary>
    /// The I2c class is used for interacting with the I2C module on the Treehopper Board. 
    /// </summary>
    internal class HardwareI2c : I2c
    {
        TreehopperUsb device;

        internal HardwareI2c(TreehopperUsb device)
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

        private void SendConfig()
        {
            double TH0 = 256.0 - 4000.0 / (3.0 * speed);
            if(TH0 < 0 || TH0 > 255.0)
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
        public async Task<byte[]> SendReceive(byte address, byte[] dataToWrite, byte numBytesToRead)
        {
            if(!Enabled)
            {
                Debug.WriteLine("NOTICE: I2c.SendReceive() called before enabling the peripheral. This call will be ignored.");
            }
            byte[] receivedData = new byte[numBytesToRead];
            int txLen = dataToWrite.Length;

            using (await device.ComsLock.LockAsync())
            //lock (device.ComsLock)
            {
                byte[] dataToSend = new byte[4 + txLen]; // 2 bytes for the header
                dataToSend[0] = (byte)DeviceCommands.I2cTransaction;
                dataToSend[1] = address;
                dataToSend[2] = (byte)txLen; // total length (0-255)
                dataToSend[3] = numBytesToRead;

                Array.Copy(dataToWrite, 0, dataToSend, 4, txLen);

                int bytesRemaining = dataToSend.Length; 
                int offset = 0;

                // for long transactions (> 64 bytes - 4 byte header), we send <=64 byte chunks, one by one.
                while (bytesRemaining > 0)
                {
                    int transferLength = bytesRemaining > 64 ? 64 : bytesRemaining;
                    var tmp = dataToSend.Skip(offset).Take(transferLength);
                    device.sendPeripheralConfigPacket(tmp.ToArray());
                    offset += transferLength;
                    bytesRemaining -= transferLength;
                }

                if (numBytesToRead == 0)
                {
                    //var result = device.receiveCommsResponsePacket((uint)1).Result;
                    var result = await device.receiveCommsResponsePacket((uint)1).ConfigureAwait(false);
                    if (result[0] != 255)
                    {
                        var error = (I2cTransferError)result[0];
                        Debug.WriteLine("NOTICE: I2C transaction resulted in an error: " + error);
                        if(TreehopperUsb.Settings.ThrowExceptions)
                            throw new I2cTransferException() { Error = error };
                    }
                        
                } else
                {
                    bytesRemaining = numBytesToRead + 1; // received data length + status byte
                    int srcIndex = 0;
                    var result = new byte[bytesRemaining];
                    while (bytesRemaining > 0)
                    {
                        int numBytesToTransfer = bytesRemaining > 64 ? 64 : bytesRemaining;
                        //var chunk = device.receiveCommsResponsePacket((uint)numBytesToTransfer).Result;
                        var chunk = await device.receiveCommsResponsePacket((uint)numBytesToTransfer).ConfigureAwait(false);
                        Array.Copy(chunk, 0, result, srcIndex, receivedData.Length); // just in case we don't get what we're expecting
                        srcIndex += numBytesToTransfer;
                        bytesRemaining -= numBytesToTransfer;
                    }

                    if (result[0] != 255)
                    {
                        var error = (I2cTransferError)result[0];
                        Debug.WriteLine("NOTICE: I2C transaction resulted in an error: " + error);
                        if (TreehopperUsb.Settings.ThrowExceptions)
                            throw new I2cTransferException() { Error = error };
                    } else
                    {
                        Array.Copy(result, 1, receivedData, 0, numBytesToRead);
                    }
                    

                }
            }

            return receivedData;
        }
    }

    public class I2cTransferException : Exception
    {
        public I2cTransferError Error { get; set; }
    }
}
