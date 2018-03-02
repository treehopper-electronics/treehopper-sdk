using System.ComponentModel;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Sensors
{
    /// <summary>
    ///     Represents any sensor or input that must be polled to retrieve an update
    /// </summary>
    public interface IPollable : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or sets whether reading from the sensor's properties should request updates from the sensor automatically 
        /// (defaults to `true`).
        /// </summary>
        /// <remarks>
        /// By default, whenever you access one of the properties of this sensor, a new reading will be fetched. If this property
        /// is set to false, you must manually call the UpdateAsync() method to retrieve a new sensor reading.
        /// 
        /// This functionality enables more efficient sensor updates, since it allows you to intelligently control when data is gathered.
        /// 
        /// </remarks>
        bool AutoUpdateWhenPropertyRead { get; set; }

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
        Task UpdateAsync();
    }
}