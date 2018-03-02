using System.ComponentModel;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Sensors.Pressure
{
    /// <summary>
    ///     Pressure sensor base
    /// </summary>
    public abstract class PressureSensor : IPressure
    {
        protected double pascal;
        /// <summary>
        ///     The pressure, in bars
        /// </summary>
        public double Bar => Pascal / 100000.0;

        /// <summary>
        ///     The pressure, in Atmospheres.
        /// </summary>
        public double Atm => Pascal / (1.01325 * 100000.0);

        /// <summary>
        ///     The pressure, in PSI
        /// </summary>
        public double Psi => Atm / 14.7;

        /// <summary>
        /// Gets or sets whether reading from the sensor's properties should request updates from the sensor automatically (defaults to true).
        /// </summary>
        /// <remarks>
        /// By default, whenever you access one of the properties of this sensor, a new reading will be fetched. If this property
        /// is set to false, you must manually call the UpdateAsync() method to retrieve a new sensor reading.
        /// </remarks>
        public bool AutoUpdateWhenPropertyRead { get; set; } = true;

        /// <summary>
        ///     The pressure, in Pascal
        /// </summary>
        public double Pascal {
            get
            {
                if (AutoUpdateWhenPropertyRead)
                    Task.Run(UpdateAsync).Wait();

                return pascal;
            }
        }

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

        public void RaisePropertyChanged(object sender, string property)
        {
            PropertyChanged?.Invoke(sender, new PropertyChangedEventArgs(property));
        }

        public void RaisePropertyChanged(object sender)
        {
            PropertyChanged?.Invoke(sender, new PropertyChangedEventArgs("Pascal"));
            PropertyChanged?.Invoke(sender, new PropertyChangedEventArgs("Psi"));
            PropertyChanged?.Invoke(sender, new PropertyChangedEventArgs("Atm"));
            PropertyChanged?.Invoke(sender, new PropertyChangedEventArgs("Bar"));
        }
    }
}