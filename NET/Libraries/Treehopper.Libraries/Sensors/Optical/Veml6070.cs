using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        public event PropertyChangedEventHandler PropertyChanged;

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
        /// Gets or sets whether reading from the sensor's properties should request updates from the sensor automatically (defaults to true).
        /// </summary>
        /// <remarks>
        /// By default, whenever you access one of the properties of this sensor, a new reading will be fetched. If this property
        /// is set to false, you must manually call the UpdateAsync() method to retrieve a new sensor reading.
        /// </remarks>
        public bool AutoUpdateWhenPropertyRead { get; set; } = true;

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
        public async Task UpdateAsync()
        {
            var lsb = await i2c.SendReceive(0x38, null, 1).ConfigureAwait(false);
            var msb = await i2c.SendReceive(0x39, null, 1).ConfigureAwait(false);
            int val = msb[0] << 8 | lsb[0];
            _uv = 5.0 * val / Math.Pow(2, (int)time - 1); // take into account the integration time

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Uv)));
        }

        /// <summary>
        /// Gets the UV intensity, measured in uW/cm^2
        /// </summary>
        public double Uv
        {
            get
            {
                if (AutoUpdateWhenPropertyRead)
                    Task.Run(UpdateAsync).Wait();
                return _uv;
            }
        }
    }
}
