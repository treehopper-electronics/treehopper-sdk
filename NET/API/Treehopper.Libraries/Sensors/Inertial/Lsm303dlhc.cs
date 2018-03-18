using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using Treehopper.Libraries.Sensors.Magnetic;

namespace Treehopper.Libraries.Sensors.Inertial
{
    /// <summary>
    /// ST LSM303DLHC 6-DoF IMU
    /// </summary>
    [Supports("ST", "LSM303DLHC")]
    public class Lsm303dlhc : IPollable
    {
        public Lsm303dlhcAccel Accel { get; set; }
        public Lsm303dlhcMag Mag { get; set; }

        private bool autoUpdateWhenPropertyRead = true;

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets whether reading from the sensor's properties should request updates from the sensor automatically (defaults to true).
        /// </summary>
        /// <remarks>
        /// By default, whenever you access one of the properties of this sensor, a new reading will be fetched. If this property
        /// is set to false, you must manually call the UpdateAsync() method to retrieve a new sensor reading.
        /// </remarks>
        public bool AutoUpdateWhenPropertyRead
        {
            get
            {
                return autoUpdateWhenPropertyRead;
            }
            set
            {
                autoUpdateWhenPropertyRead = value;
                Accel.AutoUpdateWhenPropertyRead = value;
                Mag.AutoUpdateWhenPropertyRead = value;
            }
        }

        public Lsm303dlhc(I2C i2c, int rate=100)
        {
            Accel = new Lsm303dlhcAccel(i2c, rate);
            Mag = new Lsm303dlhcMag(i2c, rate);
            Accel.AutoUpdateWhenPropertyRead = true;
            Mag.AutoUpdateWhenPropertyRead = true;
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
        public async Task UpdateAsync()
        {
            await Accel.UpdateAsync().ConfigureAwait(false);
            await Mag.UpdateAsync().ConfigureAwait(false);
        }
    }
}
