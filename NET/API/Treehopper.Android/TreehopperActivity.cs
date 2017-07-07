using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Hardware.Usb;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Treehopper.Android
{
    [Activity(Label = "TreehopperActivity")]
    public class TreehopperActivity : Activity
    {
        private ConnectionService connectionService = ConnectionService.Instance;

        public ObservableCollection<TreehopperUsb> Boards => connectionService.Boards;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            connectionService.Context = ApplicationContext;
            // Create your application here
        }

        protected override void OnStart()
        {
            base.OnStart();
            connectionService.Scan();
        }

        protected override void OnResume()
        {
            base.OnResume();

            IntentFilter filter = new IntentFilter();
            filter.AddAction(UsbManager.ActionUsbDeviceDetached);
            RegisterReceiver(connectionService, filter);

            connectionService.Scan();

            Intent intent = this.Intent;
            if (intent != null)
            {
                if (intent.Action == UsbManager.ActionUsbDeviceAttached)
                {
                    UsbDevice usbDevice = (UsbDevice)intent.GetParcelableExtra(UsbManager.ExtraDevice);
                    connectionService.DeviceAdded(usbDevice);
                }
            }
        }
    }
}