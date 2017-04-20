using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Treehopper.Utilities;

namespace Treehopper
{
    /// <summary>
    ///     The I2c class is used for interacting with the I2C module on the Treehopper Board.
    /// </summary>
    internal class HardwareI2C : I2C
    {
        private readonly TreehopperUsb _device;
        private bool _enabled;
        private double _speed = 100;

        internal HardwareI2C(TreehopperUsb device)
        {
            _device = device;
        }

        public Pin Sda => _device.Pins[3];

        public Pin Scl => _device.Pins[4];

        public bool Enabled
        {
            get { return _enabled; }

            set
            {
                if (value == _enabled)
                    return;
                _enabled = value;
                if (_enabled)
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
            get { return _speed; }

            set
            {
                if (_speed.CloseTo(value)) return;
                _speed = value;
                SendConfig();
            }
        }

        /// <summary>
        ///     Sends and Receives data. This is a blocking call that won't return until I2C communication is complete.
        /// </summary>
        /// <param name="address">The address of the device. This address should not include the read/write bit.</param>
        /// <param name="dataToWrite">Array of one or more bytes to write to the device.</param>
        /// <param name="numBytesToRead">Number of bytes to receive from the device.</param>
        /// <returns>Data read from the device.</returns>
        public async Task<byte[]> SendReceive(byte address, byte[] dataToWrite, byte numBytesToRead)
        {
            if (!Enabled)
                Debug.WriteLine(
                    "NOTICE: I2c.SendReceive() called before enabling the peripheral. This call will be ignored.");

            var receivedData = new byte[numBytesToRead];
            var txLen = dataToWrite?.Length ?? 0;

            using (await _device.ComsLock.LockAsync())
            {
                var dataToSend = new byte[4 + txLen]; // 2 bytes for the header
                dataToSend[0] = (byte) DeviceCommands.I2cTransaction;
                dataToSend[1] = address;
                dataToSend[2] = (byte) txLen; // total length (0-255)
                dataToSend[3] = numBytesToRead;

                if (txLen > 0)
                    Array.Copy(dataToWrite, 0, dataToSend, 4, txLen);

                var bytesRemaining = dataToSend.Length;
                var offset = 0;

                // for long transactions (> 64 bytes - 4 byte header), we send <=64 byte chunks, one by one.
                while (bytesRemaining > 0)
                {
                    var transferLength = bytesRemaining > 64 ? 64 : bytesRemaining;
                    var tmp = dataToSend.Skip(offset).Take(transferLength);
                    await _device.SendPeripheralConfigPacket(tmp.ToArray()).ConfigureAwait(false);
                    offset += transferLength;
                    bytesRemaining -= transferLength;
                }

                if (numBytesToRead == 0)
                {
                    var result = await _device.ReceiveCommsResponsePacket(1).ConfigureAwait(false);
                    if (result[0] != 255)
                    {
                        var error = (I2CTransferError) result[0];
                        Debug.WriteLine("NOTICE: I2C transaction resulted in an error: " + error);
                        if (TreehopperUsb.Settings.ThrowExceptions)
                            throw new I2CTransferException {Error = error};
                    }
                }
                else
                {
                    bytesRemaining = numBytesToRead + 1; // received data length + status byte
                    var srcIndex = 0;
                    var result = new byte[bytesRemaining];
                    while (bytesRemaining > 0)
                    {
                        var numBytesToTransfer = bytesRemaining > 64 ? 64 : bytesRemaining;
                        var chunk = await _device.ReceiveCommsResponsePacket((uint) numBytesToTransfer).ConfigureAwait(false);
                        Array.Copy(chunk, 0, result, srcIndex,
                            chunk.Length); // just in case we don't get what we're expecting
                        srcIndex += numBytesToTransfer;
                        bytesRemaining -= numBytesToTransfer;
                    }

                    if (result[0] != 255)
                    {
                        var error = (I2CTransferError) result[0];
                        Debug.WriteLine("NOTICE: I2C transaction resulted in an error: " + error);
                        if (TreehopperUsb.Settings.ThrowExceptions)
                            throw new I2CTransferException {Error = error};
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
            if (_enabled)
                return $"Enabled, {_speed:0.00} kHz";
            return "Not enabled";
        }

        private void SendConfig()
        {
            var th0 = 256.0 - 4000.0 / (3.0 * _speed);
            if (th0 < 0 || th0 > 255.0)
                throw new Exception("Rate out of limits. Valid rate is 62.5 kHz - 16000 kHz (16 MHz)");

            var dataToSend = new byte[3];
            dataToSend[0] = (byte) DeviceCommands.I2cConfig;
            dataToSend[1] = (byte) (_enabled ? 0x01 : 0x00);
            dataToSend[2] = (byte) Math.Round(th0);
            _device.SendPeripheralConfigPacket(dataToSend);
        }
    }
}