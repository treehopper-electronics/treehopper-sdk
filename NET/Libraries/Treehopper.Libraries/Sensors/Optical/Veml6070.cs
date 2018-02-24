using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper;

namespace Treehopper.Libraries.Sensors.Optical
{
    /// <summary>
    /// Vishay VEML6070 UVA Light Sensor with I2C Interface
    /// </summary>
    [Supports("Vishay", "VEML6070")]
    public class Veml6070 : IPollable
    {
        private I2C i2c;
        private double _uv;
        private IntegrationTime time;

        /// <summary>
        /// Integration time
        /// </summary>
        public enum IntegrationTime
        {
            TimesHalf = 0x00,
            Times1 = 0x01,
            Times2 = 0x02,
            Times4 = 0x03
        }

        /// <summary>
        /// Construct a new VEML6070 attached to the specified I2C port
        /// </summary>
        /// <param name="i2c">the I2C port to use</param>
        /// <param name="time">The integration time setting</param>
        /// <remarks>
        /// Note that once the sensor is running, it will take 500 milliseconds to get a correct reading. The first reading you obtain will be invalid, and should be discarded.
        /// </remarks>
        public Veml6070(I2C i2c, IntegrationTime time = IntegrationTime.Times1)
        {
            this.i2c = i2c;
            this.i2c.Enabled = true;
            this.time = time;
            this.i2c.SendReceive(0x38, new byte[] {(byte) ((byte) time << 2 | 0x02)}, 0);
        }

        /// <summary>
        /// Whether data will be collected from the sensor when the Uv property is read
        /// </summary>
        public bool AutoUpdateWhenPropertyRead { get; set; } = true;

        /// <summary>
        /// The interval to poll the sensor at. Unused.
        /// </summary>
        public int AwaitPollingInterval { get; set; } = 10;
        public async Task Update()
        {
            var lsb = await i2c.SendReceive(0x38, null, 1).ConfigureAwait(false);
            var msb = await i2c.SendReceive(0x39, null, 1).ConfigureAwait(false);
            int val = msb[0] << 8 | lsb[0];
            _uv = 5.0 * val / Math.Pow(2, (int)time - 1); // take into account the integration time
        }

        /// <summary>
        /// Gets the UV intensity, measured in uW/cm^2
        /// </summary>
        public double Uv
        {
            get
            {
                if (AutoUpdateWhenPropertyRead) Update().Wait();
                return _uv;
            }
        }
    }
}
