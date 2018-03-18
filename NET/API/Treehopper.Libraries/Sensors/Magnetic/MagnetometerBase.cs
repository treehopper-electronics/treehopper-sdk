using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Sensors.Magnetic
{
    public abstract class MagnetometerBase : IMagnetometer
    {
        protected Vector3 _magnetometer;

        /// <summary>
        /// Gets the three-axis magnetometer value, in uT (micro-Tesla)
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
        /// Gets or sets whether reading from the sensor's properties should request updates from the sensor automatically (defaults to true).
        /// </summary>
        /// <remarks>
        /// By default, whenever you access one of the properties of this sensor, a new reading will be fetched. If this property
        /// is set to false, you must manually call the UpdateAsync() method to retrieve a new sensor reading.
        /// </remarks>
        public bool AutoUpdateWhenPropertyRead { get; set; } = true;

        public event PropertyChangedEventHandler PropertyChanged;

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
        public abstract Task UpdateAsync();

        protected void RaisePropertyChanged(object sender)
        {
            PropertyChanged?.Invoke(sender, new PropertyChangedEventArgs(nameof(Magnetometer)));
        }

        protected void RaisePropertyChanged(object sender, string property)
        {
            PropertyChanged?.Invoke(sender, new PropertyChangedEventArgs(property));
        }
    }
}
