using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Treehopper.Desktop.MacUsb;

namespace Treehopper
{
    /// <summary>
    ///     This class is used for discovering <see cref="TreehopperUsb" /> devices attached to this computer.
    /// </summary>
    public abstract class ConnectionService : IConnectionService, IDisposable
    {
        private static readonly Lazy<MacUsbConnectionService> macUsbInstance = new Lazy<MacUsbConnectionService>();

        private TaskCompletionSource<TreehopperUsb> waitForFirstBoard = new TaskCompletionSource<TreehopperUsb>();

        public ConnectionService()
        {
            Boards.CollectionChanged += Boards_CollectionChanged;
        }

        /// <summary>
        ///     The singleton instance through which to access ConnectionService.
        /// </summary>
        /// <remarks>
        ///     This instance is created and started upon the first reference to a property or method
        ///     on this object. This typically only becomes an issue if you expect to have debug messages
        ///     from ConnectionService printing even if you haven't actually accessed the object yet.
        /// </remarks>
        public static ConnectionService Instance
        {
            get
            {
                return macUsbInstance.Value;
            }
        }

        /// <summary>
        ///     Collection of <see cref="TreehopperUsb" /> devices attached to this computer
        /// </summary>
        public ObservableCollection<TreehopperUsb> Boards { get; } = new ObservableCollection<TreehopperUsb>();


        /// <summary>
        ///     Get a reference to the first device discovered.
        /// </summary>
        /// <returns>The first board found.</returns>
        /// <remarks>
        ///     <para>
        ///         If no devices have been plugged into the computer,
        ///         this call will await indefinitely until a board is plugged in.
        ///     </para>
        /// </remarks>
        public Task<TreehopperUsb> GetFirstDeviceAsync()
        {
            return waitForFirstBoard.Task;
        }

        public abstract void Dispose();

        private void Boards_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (Boards.Count == 0)
                waitForFirstBoard = new TaskCompletionSource<TreehopperUsb>();

            else if ((e.OldItems?.Count ?? 0) == 0 && e.NewItems.Count > 0)
                waitForFirstBoard.TrySetResult(Boards[0]);
        }
    }
}