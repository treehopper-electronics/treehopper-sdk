namespace Treehopper
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// The I2c class is used for interacting with the I2C module on the Treehopper Board. 
    /// </summary>
    internal class HardwareI2c : I2c
    {
        private TreehopperUsb device;
        private double speed = 100;
        private bool enabled;

        internal HardwareI2c(TreehopperUsb device)
        {
            this.device = device;
        }

        public bool Enabled
        {
            get
            {
                return enabled;
            }

            set
            {
                if (value == enabled)
                    return;
                enabled = value;
                if (enabled)
                {
                    Sda.Mode = PinMode.Reserved;
                    Scl.Mode = PinMode.Reserved;
                }
                else
                {
                    Sda.Mode = PinMode.Unassigned;
                    Scl.Mode = PinMode.Unassigned;
                }

                SendConfig();
            }
        }

        public double Speed
        {
            get
            {
                return speed;
            }

            set
            {
                if (speed.CloseTo(value)) return;
                speed = value;
                SendConfig();
            }
        }

        public Pin Sda
        {
            get { return device.Pins[3]; }
        }

        public Pin Scl
        {
            get { return device.Pins[4]; }
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
            if (!Enabled)
            {
                Debug.WriteLine("NOTICE: I2c.SendReceive() called before enabling the peripheral. This call will be ignored.");
            }

            byte[] receivedData = new byte[numBytesToRead];
            int txLen = dataToWrite?.Length ?? 0;

            using (await device.ComsLock.LockAsync())
            {
                byte[] dataToSend = new byte[4 + txLen]; // 2 bytes for the header
                dataToSend[0] = (byte)DeviceCommands.I2cTransaction;
                dataToSend[1] = address;
                dataToSend[2] = (byte)txLen; // total length (0-255)
                dataToSend[3] = numBytesToRead;

                if(txLen > 0)
                    Array.Copy(dataToWrite, 0, dataToSend, 4, txLen);

                int bytesRemaining = dataToSend.Length;
                int offset = 0;

                // for long transactions (> 64 bytes - 4 byte header), we send <=64 byte chunks, one by one.
                while (bytesRemaining > 0)
                {
                    int transferLength = bytesRemaining > 64 ? 64 : bytesRemaining;
                    var tmp = dataToSend.Skip(offset).Take(transferLength);
                    device.SendPeripheralConfigPacket(tmp.ToArray());
                    offset += transferLength;
                    bytesRemaining -= transferLength;
                }

                if (numBytesToRead == 0)
                {
                    var result = await device.ReceiveCommsResponsePacket((uint)1).ConfigureAwait(false);
                    if (result[0] != 255)
                    {
                        var error = (I2cTransferError)result[0];
                        Debug.WriteLine("NOTICE: I2C transaction resulted in an error: " + error);
                        if (TreehopperUsb.Settings.ThrowExceptions)
                            throw new I2cTransferException() { Error = error };
                    }
                }
                else
                {
                    bytesRemaining = numBytesToRead + 1; // received data length + status byte
                    int srcIndex = 0;
                    var result = new byte[bytesRemaining];
                    while (bytesRemaining > 0)
                    {
                        int numBytesToTransfer = bytesRemaining > 64 ? 64 : bytesRemaining;
                        var chunk = await device.ReceiveCommsResponsePacket((uint)numBytesToTransfer).ConfigureAwait(false);
                        Array.Copy(chunk, 0, result, srcIndex, chunk.Length); // just in case we don't get what we're expecting
                        srcIndex += numBytesToTransfer;
                        bytesRemaining -= numBytesToTransfer;
                    }

                    if (result[0] != 255)
                    {
                        var error = (I2cTransferError)result[0];
                        Debug.WriteLine("NOTICE: I2C transaction resulted in an error: " + error);
                        if (TreehopperUsb.Settings.ThrowExceptions)
                            throw new I2cTransferException() { Error = error };
                    }
                    else
                    {
                        Array.Copy(result, 1, receivedData, 0, numBytesToRead);
                    }
                }
            }

            return receivedData;
        }

        public override string ToString()
        {
            if (enabled)
                return string.Format("Enabled, {0:0.00} kHz", speed);
            else
                return "Not enabled";
        }

        private void SendConfig()
        {
            double th0 = 256.0 - (4000.0 / (3.0 * speed));
            if (th0 < 0 || th0 > 255.0)
            {
                throw new Exception("Rate out of limits. Valid rate is 62.5 kHz - 16000 kHz (16 MHz)");
            }

            byte[] dataToSend = new byte[3];
            dataToSend[0] = (byte)DeviceCommands.I2cConfig;
            dataToSend[1] = (byte)(enabled ? 0x01 : 0x00);
            dataToSend[2] = (byte)Math.Round(th0);
            device.SendPeripheralConfigPacket(dataToSend);
        }
    }
}
