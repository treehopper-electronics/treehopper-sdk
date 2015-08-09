using System;
using System.Diagnostics;
using System.Threading;

namespace Treehopper
{

    //public enum I2cMode { Master, Slave };

    /// <summary>
    /// The I2c class is used for interacting with the I2C module on the Treehopper Board. 
    /// </summary>
    public class I2c
    {
        TreehopperUSB device;

        internal I2c(TreehopperUSB device)
        {
            this.device = device;
        }

        /// <summary>
        /// Start I2C communication with the given mode and rate. SDA is on Pin 10, SCL is on pin 11
        /// </summary>
        /// <param name="mode">Master or slave mode</param>
        /// <param name="rate">Communication rate, in kHz.</param>
        public void Start( double rate = 100.0)
        {
            double SSPADD = (12000.0 / rate - 1);
            if(SSPADD < 3 || SSPADD > 255)
            {
                throw new Exception("Rate out of limits. Valid rate is 46.875 kHz - 3000 kHz (3 MHz)");
            }
            byte[] dataToSend = new byte[3];
            dataToSend[0] = (byte)DeviceCommands.I2cConfig;
            dataToSend[1] = 0; // this is hard-coded until the API can be updated with slave support.
            dataToSend[2] = (byte)SSPADD;
            device.sendPeripheralConfigPacket(dataToSend);
        }
        private Object lockObject = new object();
        /// <summary>
        /// Sends and Receives data. This is a blocking call that won't return until I2C communication is complete.
        /// </summary>
        /// <param name="address">The address of the device. This address should not include the read/write bit.</param>
        /// <param name="dataToWrite">Array of one or more bytes to write to the device.</param>
        /// <param name="numBytesToRead">Number of bytes to receive from the device.</param>
        /// <returns>Data read from the device.</returns>
        public byte[] SendReceive(byte address, byte[] dataToWrite, byte numBytesToRead)
        {
           
            byte[] returnedData = new byte[numBytesToRead];
            byte[] dataToSend = new byte[4 + dataToWrite.Length];
            dataToSend[0] = (byte)DeviceCommands.I2cTransaction;
            dataToSend[1] = address;
            dataToSend[2] = (byte)dataToWrite.Length;
            dataToSend[3] = numBytesToRead;
            dataToWrite.CopyTo(dataToSend, 4);
            lock(lockObject)
            {
                device.sendPeripheralConfigPacket(dataToSend);
                Thread.Sleep(1);
                byte[] response = device.receiveCommsResponsePacket();
                if (numBytesToRead > 0)
                {
                    Array.Copy(response, 1, returnedData, 0, numBytesToRead);
                }
            }

            return returnedData;
        }
    }
}
