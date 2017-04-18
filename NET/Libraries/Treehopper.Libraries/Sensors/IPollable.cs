using System.Threading.Tasks;

namespace Treehopper.Libraries.Sensors
{
    /// <summary>
    /// Represents any sensor or input that must be polled to retrieve an update
    /// </summary>
    public interface IPollable
    {
        /// <summary>
        /// Whether the input should fetch an update whenever a property is read
        /// </summary>
        bool AutoUpdateWhenPropertyRead { get; set; }

        int AwaitPollingInterval { get; set; }

        /// <summary>
        /// Forces the input or sensor to update the data
        /// </summary>
        /// <returns>An awaitable task</returns>
        Task Update();
    }
}
