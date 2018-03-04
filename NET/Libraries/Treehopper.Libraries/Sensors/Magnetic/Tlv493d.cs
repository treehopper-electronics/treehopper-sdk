using System.ComponentModel;
using System.Numerics;
using System.Threading.Tasks;
using Treehopper.Libraries.Sensors.Temperature;

namespace Treehopper.Libraries.Sensors.Magnetic
{
    /// <summary>
    /// TLV493D-A1B6 3D magnetic / temperature sensor
    /// </summary>
    [Supports("Infineon", "TLV493D-A1B6")]
    public class Tlv493d : TemperatureSensorBase, IMagnetometer
    {
        private readonly byte address;
        private readonly I2C i2c;
        private Vector3 _magnetometer;

        /// <summary>
        /// Construct a TLV493D-A1B6 3D magnetic / temperature sensor
        /// </summary>
        /// <param name="i2c"></param>
        /// <param name="address"></param>
        public Tlv493d(I2C i2c, byte address = 0x5E)
        {
            i2c.Enabled = true;
            this.i2c = i2c;
            this.address = address;

            // get the current data from the device
            var result = i2c.SendReceiveAsync(address, null, 10).Result;

            // set the config to Master-Controlled mode
            var dataToWrite = new byte[4];
            dataToWrite[0] = 0x00;
            dataToWrite[1] = (byte) ((result[7] & 0x18) | 0x03); // fastmode = 1, lp_mode = 1
            dataToWrite[2] = result[8];
            dataToWrite[3] = (byte) ((result[9] & 0x1F) | 0x40); // LP = 1 -> 12 ms period

            Task.Run(() => i2c.SendReceiveAsync(address, dataToWrite, 0)).Wait();
        }

        /// <summary>
        /// Magnetic flux in each axis, measured in mT
        /// </summary>
        public Vector3 Magnetometer
        {
            get
            {
                if (AutoUpdateWhenPropertyRead)
                    Task.Run(UpdateAsync).Wait();

                return _magnetometer;
            }
        }

        /// <summary>
        /// Requests a reading from the sensor and updates its data properties with the gathered values.
        /// </summary>
        /// <returns>An awaitable Task</returns>
        /// <remarks>
        /// Note that when #AutoUpdateWhenPropertyRead is `true` (which it is, by default), this method is implicitly 
        /// called when any sensor data property is read from --- there's no need to call this method unless you set
        /// AutoUpdateWhenPropertyRead to `false`.
        /// 
        /// Unless otherwise noted, this method updates all sensor data simultaneously, which can often lead to more efficient
        /// bus usage (as well as reducing USB chattiness).
        /// </remarks>
        public override async Task UpdateAsync()
        {
            var value = await i2c.SendReceiveAsync(address, null, 7).ConfigureAwait(false);

            // the datasheet lists a digital value of 340 @ 25C, and a resolution of 1.1C per LSB
            celsius = (((short) ((((value[3] & 0xf0) << 4) | value[6]) << 4) >> 4) - 340) * 1.1 + 25.0;

            // These are signed 12-bit values -- shift them up by 4 to pick up the sign, then shift them back down by 4
            // ...then convert to mT
            _magnetometer.X = ((short) (((value[0] << 4) | (value[4] >> 4)) << 4) >> 4) * 0.098f;
            _magnetometer.Y = ((short) (((value[1] << 4) | value[4]) << 4) >> 4) * 0.098f;
            _magnetometer.Z = ((short) (((value[2] << 4) | value[5]) << 4) >> 4) * 0.098f;

            RaisePropertyChanged(this);
            RaisePropertyChanged(this, nameof(Magnetometer));
        }
    }
}