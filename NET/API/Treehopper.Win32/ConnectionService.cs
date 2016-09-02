using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Treehopper;

using LibUsbDotNet;
using LibUsbDotNet.DeviceNotify;
using LibUsbDotNet.Main;

namespace Treehopper
{
    public sealed class ConnectionService : IConnectionService, IDisposable
    {
        static readonly IConnectionService instance = new ConnectionService();
        public static IConnectionService Instance
        {
            get
            {
                return instance;
            }
        }

        IDeviceNotifier notifier;
        public ConnectionService()
        {
            UsbDevice.ForceLibUsbWinBack = false;
            notifier = DeviceNotifier.OpenDeviceNotifier();
            notifier.OnDeviceNotify += Notifier_OnDeviceNotify;
            Rescan();
        }

        private void Notifier_OnDeviceNotify(object sender, DeviceNotifyEventArgs e)
        {
            Debug.WriteLine("DeviceNotifier received notification");
            Rescan();
        }

        private void Rescan()
        {
            foreach (UsbRegistry regDevice in UsbDevice.AllDevices)
            {
                if (regDevice.Vid != TreehopperUsb.Settings.Vid || regDevice.Pid != TreehopperUsb.Settings.Pid)
                    continue;
                // read in the serial number
                var connection = new UsbConnection(regDevice.Device);
                if (Boards.Where(i => i.Connection.SerialNumber == connection.SerialNumber).Count() == 0)
                {
                    var board = new TreehopperUsb(connection);
                    Debug.WriteLine("Added board: " + board);
                    Boards.Add(board);
                }
            }

            foreach (var board in Boards.ToList())
            {
                bool deviceFound = false;
                foreach(UsbRegistry dev in UsbDevice.AllDevices)
                {
                    var connection = new UsbConnection(dev.Device);
                    if (connection.SerialNumber == board.SerialNumber)
                        deviceFound = true;
                }
                if(!deviceFound)
                {
                    Debug.WriteLine("Removing board: " + board);
                    Boards.Remove(board);
                    board.Dispose();
                }
            }
        }

        public ObservableCollection<TreehopperUsb> Boards { get; } = new ObservableCollection<TreehopperUsb>();

        public event PropertyChangedEventHandler PropertyChanged;

        public void Dispose()
        {
            
        }

        public async Task<TreehopperUsb> First()
        {
            while (Boards.Count == 0)
            {
                await Task.Delay(100);
            }

            return Boards[0];
        }
    }
}
