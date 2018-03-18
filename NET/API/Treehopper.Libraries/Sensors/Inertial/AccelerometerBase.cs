using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Sensors.Inertial
{
    /// <summary>
    /// Base accelerometer class
    /// </summary>
    public abstract class AccelerometerBase : IAccelerometer
    {
        protected Vector3 _accelerometer;

        /// <summary>
        /// Gets a vector with the three-dimensional acceleration data, in g.
        /// </summary>
        /// <remarks>
        /// \f$1\, \text{g} = 9.8\, \text{m}/\text{s}^2\f$
        /// </remarks>
        public Vector3 Accelerometer
        {
            get
            {
                if (AutoUpdateWhenPropertyRead)
                    Task.Run(UpdateAsync).Wait();

                return _accelerometer;
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
            PropertyChanged?.Invoke(sender, new PropertyChangedEventArgs(nameof(Accelerometer)));
        }

        protected void RaisePropertyChanged(object sender, string property)
        {
            PropertyChanged?.Invoke(sender, new PropertyChangedEventArgs(property));
        }
    }
}
