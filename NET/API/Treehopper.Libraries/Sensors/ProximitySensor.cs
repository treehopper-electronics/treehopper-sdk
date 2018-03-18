using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Sensors
{
    public abstract class ProximitySensor : IProximity
    {
        protected double meters = 0;
        public double Millimeters => Meters * 1000;
        public double Centimeters => Meters * 100;
        public double Inches => Meters * 39.3701;
        public double Feet => Meters * 3.28085;
        public double Meters
        {
            get
            {
                if (AutoUpdateWhenPropertyRead)
                    Task.Run(UpdateAsync).Wait();

                return meters;
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

        public void RaisePropertyChanged(object sender)
        {
            PropertyChanged?.Invoke(sender, new PropertyChangedEventArgs("Millimeters"));
            PropertyChanged?.Invoke(sender, new PropertyChangedEventArgs("Centimeters"));
            PropertyChanged?.Invoke(sender, new PropertyChangedEventArgs("Inches"));
            PropertyChanged?.Invoke(sender, new PropertyChangedEventArgs("Feet"));
            PropertyChanged?.Invoke(sender, new PropertyChangedEventArgs("Meters"));
        }

        public void RaisePropertyChanged(object sender, string property)
        {
            PropertyChanged?.Invoke(sender, new PropertyChangedEventArgs(property));
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
        public abstract Task UpdateAsync();
    }
}
