using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Usb;
using Windows.Foundation;

namespace Treehopper
{
    public sealed class ConnectionService : IConnectionService, INotifyPropertyChanged
    {
        static readonly IConnectionService instance = new ConnectionService();

        public static IConnectionService Instance
        {
            get
            {
                return instance;                    
            }
        }

        public ConnectionService()
        {
            connectedDevices = new ObservableCollection<TreehopperUsb>();

            StartWatcher();
        }

        ObservableCollection<TreehopperUsb> connectedDevices;
        public ObservableCollection<TreehopperUsb> Boards
        {
            get
            {
                return connectedDevices;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void AddBoard() { }

        DeviceWatcher deviceWatcher;
        TypedEventHandler<DeviceWatcher, DeviceInformation> handlerAdded = null;
        TypedEventHandler<DeviceWatcher, DeviceInformationUpdate> handlerUpdated = null;
        TypedEventHandler<DeviceWatcher, DeviceInformationUpdate> handlerRemoved = null;
        TypedEventHandler<DeviceWatcher, Object> handlerEnumCompleted = null;
        TypedEventHandler<DeviceWatcher, Object> handlerStopped = null;

        private SemaphoreSlim boardAddedSignal = new SemaphoreSlim(0, 1);

        void StartWatcher()
        {
            //deviceWatcher = DeviceInformation.CreateWatcher(UsbDevice.GetDeviceSelector(0x04d8, 0xf426));
            deviceWatcher = DeviceInformation.CreateWatcher(UsbDevice.GetDeviceSelector(TreehopperUsb.Settings.Vid, TreehopperUsb.Settings.Pid));
            // Hook up handlers for the watcher events before starting the watcher

            handlerAdded = new TypedEventHandler<DeviceWatcher, DeviceInformation>(async (watcher, deviceInfo) =>
            {
                Debug.WriteLine("Device added: " + deviceInfo.Name);

                UsbConnection newConnection = new UsbConnection(deviceInfo);

                TreehopperUsb newBoard = new TreehopperUsb(newConnection);
                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    connectedDevices.Add(newBoard);
                });
                try
                {
                    if (boardAddedSignal.CurrentCount == 0)
                        boardAddedSignal.Release();
                }
                catch { }
            });
            deviceWatcher.Added += handlerAdded;

            handlerUpdated = new TypedEventHandler<DeviceWatcher, DeviceInformationUpdate>((watcher, deviceInfoUpdate) =>
            {
                Debug.WriteLine("Device updated");
            });
            deviceWatcher.Updated += handlerUpdated;

            handlerRemoved = new TypedEventHandler<DeviceWatcher, DeviceInformationUpdate>(async (watcher, deviceInfoUpdate) =>
            {
                Debug.WriteLine("Device removed");
                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    connectedDevices.Where(board => ((UsbConnection)(board.Connection)).DevicePath == deviceInfoUpdate.Id).ToList().All(i =>
                    {
                        i.Disconnect();
                        connectedDevices.Remove(i);
                        return true;
                    }
                    );
                });
            });
            deviceWatcher.Removed += handlerRemoved;

            handlerEnumCompleted = new TypedEventHandler<DeviceWatcher, Object>((watcher, obj) =>
            {
                Debug.WriteLine("Enum completed");
            });
            deviceWatcher.EnumerationCompleted += handlerEnumCompleted;

            handlerStopped = new TypedEventHandler<DeviceWatcher, Object>((watcher, obj) =>
            {
                Debug.WriteLine("Device or something stopped");
            });
            deviceWatcher.Stopped += handlerStopped;

            Debug.WriteLine("Starting the wutchah");
            deviceWatcher.Start();
        }

        public async Task<TreehopperUsb> GetFirstDeviceAsync()
        {
            await boardAddedSignal.WaitAsync();
            return Boards[0];
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
