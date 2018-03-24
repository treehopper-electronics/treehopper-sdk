using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Devices.Enumeration;
using Windows.Devices.Usb;
using Windows.Foundation;
using Windows.UI.Core;
using Treehopper.Uwp;
using System.Collections.Specialized;

namespace Treehopper
{
    public sealed class ConnectionService : IConnectionService
    {
        private TaskCompletionSource<TreehopperUsb> waitForFirstBoard = new TaskCompletionSource<TreehopperUsb>();

        private DeviceWatcher deviceWatcher;
        private TypedEventHandler<DeviceWatcher, DeviceInformation> handlerAdded;
        private TypedEventHandler<DeviceWatcher, object> handlerEnumCompleted;
        private TypedEventHandler<DeviceWatcher, DeviceInformationUpdate> handlerRemoved;
        private TypedEventHandler<DeviceWatcher, object> handlerStopped;
        private TypedEventHandler<DeviceWatcher, DeviceInformationUpdate> handlerUpdated;
        private static readonly ConnectionService instance = new ConnectionService();

        /// \cond PRIVATE
        public ConnectionService()
        {
            Boards = new ObservableCollection<TreehopperUsb>();
            Boards.CollectionChanged += Boards_CollectionChanged;
            StartWatcher();
        }
        

        private void Boards_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (Boards.Count == 0)
                waitForFirstBoard = new TaskCompletionSource<TreehopperUsb>();

            else if ((e.OldItems?.Count ?? 0) == 0 && e.NewItems.Count > 0)
                waitForFirstBoard.TrySetResult(Boards[0]);
        }

        /// <summary>
        ///     The singleton instance through which to access %ConnectionService.
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
                return instance;
            }
        }

        /// <summary>
        /// The %Treehopper boards attached to the computer.
        /// </summary>
        public ObservableCollection<TreehopperUsb> Boards { get; }


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

        public void Dispose()
        {
            // TODO: Clean stuff up
        }
        /// \endcond

        private void StartWatcher()
        {
            //deviceWatcher = DeviceInformation.CreateWatcher(UsbDevice.GetDeviceSelector(0x04d8, 0xf426));
            deviceWatcher = DeviceInformation.CreateWatcher(
                UsbDevice.GetDeviceSelector(TreehopperUsb.Settings.Vid, TreehopperUsb.Settings.Pid));
            // Hook up handlers for the watcher events before starting the watcher

            handlerAdded = async (watcher, deviceInfo) =>
            {
                Debug.WriteLine("Device added: " + deviceInfo.Name);

                var newConnection = new UsbConnection(deviceInfo);

                var newBoard = new TreehopperUsb(newConnection);
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () => { Boards.Add(newBoard); });
            };
            deviceWatcher.Added += handlerAdded;

            handlerUpdated = (watcher, deviceInfoUpdate) => { Debug.WriteLine("Device updated"); };
            deviceWatcher.Updated += handlerUpdated;

            handlerRemoved = async (watcher, deviceInfoUpdate) =>
            {
                Debug.WriteLine("Device removed");
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    Boards.Where(board => ((UsbConnection) board.Connection).DevicePath == deviceInfoUpdate.Id)
                        .ToList()
                        .All(i =>
                            {
                                i.Disconnect();
                                Boards.Remove(i);
                                return true;
                            }
                        );
                });
            };
            deviceWatcher.Removed += handlerRemoved;

            handlerEnumCompleted = (watcher, obj) => { Debug.WriteLine("Enum completed"); };
            deviceWatcher.EnumerationCompleted += handlerEnumCompleted;

            handlerStopped = (watcher, obj) => { Debug.WriteLine("Device or something stopped"); };
            deviceWatcher.Stopped += handlerStopped;

            Debug.WriteLine("Starting the wutchah");
            deviceWatcher.Start();
        }
    }
}