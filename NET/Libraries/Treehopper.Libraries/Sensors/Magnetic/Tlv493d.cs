using System.ComponentModel;
using System.Numerics;
using System.Threading.Tasks;
using Treehopper.Libraries.Sensors.Temperature;

namespace Treehopper.Libraries.Sensors.Magnetic
{
    /// <summary>
    /// TLV493D-A1B6 3D magnetic / temperature sensor
    /// </summary>
    public class Tlv493d : TemperatureSensor
    {
        private readonly byte address;
        private readonly I2C i2c;
        private Vector3 magneticFlux;

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
            var result = i2c.SendReceive(address, null, 10).Result;

            // set the config to Master-Controlled mode
            var dataToWrite = new byte[4];
            dataToWrite[0] = 0x00;
            dataToWrite[1] = (byte) ((result[7] & 0x18) | 0x03); // fastmode = 1, lp_mode = 1
            dataToWrite[2] = result[8];
            dataToWrite[3] = (byte) ((result[9] & 0x1F) | 0x40); // LP = 1 -> 12 ms period

            i2c.SendReceive(address, dataToWrite, 0).Wait();
        }

        /// <summary>
        /// Magnetic flux in each axis, measured in mT
        /// </summary>
        public Vector3 MagneticFlux
        {
            get
            {
                if (AutoUpdateWhenPropertyRead) Update().Wait();

                return magneticFlux;
            }
        }

        public override event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Update the temperature and MagneticFlux properties with the latest values from the sensor
        /// </summary>
        /// <returns>An awaitable task</returns>
        public override async Task Update()
        {
            var value = await i2c.SendReceive(address, null, 7);

            // the datasheet lists a digital value of 340 @ 25C, and a resolution of 1.1C per LSB
            Celsius = (((short) ((((value[3] & 0xf0) << 4) | value[6]) << 4) >> 4) - 340) * 1.1 + 25.0;

            // These are signed 12-bit values -- shift them up by 4 to pick up the sign, then shift them back down by 4
            // ...then convert to mT
            magneticFlux.X = ((short) (((value[0] << 4) | (value[4] >> 4)) << 4) >> 4) * 0.098f;
            magneticFlux.Y = ((short) (((value[1] << 4) | value[4]) << 4) >> 4) * 0.098f;
            magneticFlux.Z = ((short) (((value[2] << 4) | value[5]) << 4) >> 4) * 0.098f;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Celsius)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Fahrenheit)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Kelvin)));
        }
    }
}