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

        private static bool? isLinux;

        private static bool? isWindows;

        private static bool? isMac;

        private TaskCompletionSource<TreehopperUsb> waitForFirstBoard = new TaskCompletionSource<TreehopperUsb>();

        /// <summary>
        ///     Manages Treehopper boards connected to the computer. If optional filter parameters are provided, only boards
        ///     matching the filter will be available.
        /// </summary>
        /// <param name="nameFilter">Name filter</param>
        /// <param name="serialFilter">Serial filter</param>
        public ConnectionService(string nameFilter = "", string serialFilter = "")
        {
            Boards.CollectionChanged += Boards_CollectionChanged;
        }

        /// <summary>
        ///     Retrieve a reference to the static instance of the <see cref="ConnectionService" /> that should be used for
        ///     discovering boards.
        /// </summary>
        /// <remarks>
        ///     A single instance of <see cref="ConnectionService" /> is created and started upon the first reference to
        ///     <see cref="Instance" />.
        ///     In general, there is no need to construct your own <see cref="ConnectionService" />; just access
        ///     <see cref="Instance" /> for any
        ///     board discovery functionalities you need.
        /// </remarks>
        public static ConnectionService Instance
        {
            get
            {
                return macUsbInstance.Value;

                throw new Exception("Unsupported operating system");
            }
        }

        /// <summary>
        ///     Occurs when a <see cref="TreehopperBoard" /> is removed from the system.
        /// </summary>
        //public event BoardEventHandler BoardRemoved;
        public event PropertyChangedEventHandler PropertyChanged;

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
        ///     <para>
        ///         Remember to call <see cref="TreehopperUsb.ConnectAsync()" /> before starting communication.
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