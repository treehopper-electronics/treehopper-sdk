using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Treehopper
{
    /// <summary>
    ///     An interface representing connection services
    /// </summary>
    public interface IConnectionService : INotifyPropertyChanged, IDisposable
    {
        /// <summary>
        ///     A collection of boards connected to this host
        /// </summary>
        ObservableCollection<TreehopperUsb> Boards { get; }

        /// <summary>
        ///     Get a reference to the first board connected to the computer
        /// </summary>
        /// <returns>An awaitable board.</returns>
        /// <remarks>
        ///     <para>
        ///         If a board is already connected to the computer when this function is called, it will return immediately.
        ///         Otherwise, it will block indefinitely until a board is attached to the device.
        ///     </para>
        /// </remarks>
        Task<TreehopperUsb> GetFirstDeviceAsync();
    }
}