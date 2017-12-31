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

namespace Treehopper
{
    public sealed class ConnectionService : IConnectionService, INotifyPropertyChanged
    {
        private readonly SemaphoreSlim boardAddedSignal = new SemaphoreSlim(0, 1);

        private DeviceWatcher deviceWatcher;
        private TypedEventHandler<DeviceWatcher, DeviceInformation> handlerAdded;
        private TypedEventHandler<DeviceWatcher, object> handlerEnumCompleted;
        private TypedEventHandler<DeviceWatcher, DeviceInformationUpdate> handlerRemoved;
        private TypedEventHandler<DeviceWatcher, object> handlerStopped;
        private TypedEventHandler<DeviceWatcher, DeviceInformationUpdate> handlerUpdated;

        public ConnectionService()
        {
            Boards = new ObservableCollection<TreehopperUsb>();

            StartWatcher();
        }

        public static IConnectionService Instance { get; } = new ConnectionService();

        public ObservableCollection<TreehopperUsb> Boards { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public async Task<TreehopperUsb> GetFirstDeviceAsync()
        {
            await boardAddedSignal.WaitAsync();
            return Boards[0];
        }

        public void Dispose()
        {
            // TODO: Clean stuff up
        }

        public void AddBoard()
        {
        }

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
                try
                {
                    if (boardAddedSignal.CurrentCount == 0)
                        boardAddedSignal.Release();
                }
                catch
                {
                }
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