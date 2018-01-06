using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Hardware.Usb;
using Android.Util;
using Treehopper.Android;

namespace Treehopper
{
    public class ConnectionService : BroadcastReceiver, IConnectionService
    {
        private static string TAG = "ConnectionService";

        static readonly ConnectionService instance = new ConnectionService();

        public static ConnectionService Instance => instance;

        public UsbManager Manager => (UsbManager)context.GetSystemService(Context.UsbService);

        readonly static string ActionUsbPermission = "io.treehopper.android.USB_PERMISSION";

        private object lockObject = new object();

        private PendingIntent pendingIntent;

        public ConnectionService() : base()
        {
            
        }

        private void Boards_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (Boards.Count == 0)
                waitForFirstBoard = new TaskCompletionSource<TreehopperUsb>();

            else if ((e.OldItems?.Count ?? 0) == 0 && e.NewItems.Count > 0)
                waitForFirstBoard.TrySetResult(Boards[0]);
        }

        public void ActivityOnStart(Activity activity)
        {
            this.activity = activity;
            this.context = activity.ApplicationContext;
            Boards.CollectionChanged += Boards_CollectionChanged;

            Scan();

            IntentFilter filter = new IntentFilter();
            filter.AddAction(UsbManager.ActionUsbDeviceDetached);
            filter.AddAction(UsbManager.ActionUsbDeviceAttached);
            activity.RegisterReceiver(this, filter);
        }

        private TaskCompletionSource<TreehopperUsb> waitForFirstBoard = new TaskCompletionSource<TreehopperUsb>();

        public ObservableCollection<TreehopperUsb> Boards { get; set; } = new ObservableCollection<TreehopperUsb>();

        public void ActivityOnResume()
        {
            Intent intent = activity.Intent;
            if (intent != null)
            {
                if (intent.Action == UsbManager.ActionUsbDeviceAttached)
                {
                    UsbDevice usbDevice = (UsbDevice)intent.GetParcelableExtra(UsbManager.ExtraDevice);
                    DeviceAdded(usbDevice);
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Task<TreehopperUsb> GetFirstDeviceAsync()
        {
            return waitForFirstBoard.Task;
        }

        public override void OnReceive(Context context, Intent intent)
        {
            if (intent.Action == UsbManager.ActionUsbDeviceDetached)
            {
                UsbDevice usbDevice = (UsbDevice)intent.GetParcelableExtra(UsbManager.ExtraDevice);
                DeviceRemoved(usbDevice);
            }

            if (intent.Action == UsbManager.ActionUsbDeviceAttached)
            {
                UsbDevice usbDevice = (UsbDevice)intent.GetParcelableExtra(UsbManager.ExtraDevice);
                createTreehopperFromDevice(usbDevice);
            }

            if (intent.Action == ActionUsbPermission)
            {
                lock (lockObject)
                {
                    UsbDevice device = (UsbDevice)intent.GetParcelableExtra(UsbManager.ExtraDevice);

                    if (intent.GetBooleanExtra(UsbManager.ExtraPermissionGranted, false))
                    {
                        if (device != null)
                        {
                            if (Boards.Count(b => b.SerialNumber == device.SerialNumber) > 0) return;
                            var board = new TreehopperUsb(new UsbConnection(device, Manager));
                            Log.Info(TAG, "Got permission to add new board (name=" + board.Name + ", serial=" + board.SerialNumber + "). Total number of boards: " + Boards.Count);
                            Boards.Add(board);
                        }
                    }
                    else
                    {
                        Log.Debug(TAG, "permission denied for device " + device);
                    }
                }
            }
        }


        public void DeviceAdded(UsbDevice device)
        {
            createTreehopperFromDevice(device);
        }

        public void DeviceRemoved(UsbDevice device)
        {
            Log.Info(TAG, "DeviceRemoved called");
            if (device == null)
            {
                // ugh, Android didn't tell us which device was removed, but if we only have one connected, we'll remove it
                if (Boards.Count == 1)
                {
                    Boards[0].Disconnect();
                    Boards.Clear();
                }
            }
            else
            {
                if (device.VendorId == 0x10c4 && device.ProductId == 0x8a7e)
                {
                    TreehopperUsb removedBoard = Boards.FirstOrDefault(b => b.SerialNumber == device.SerialNumber);
                    if(removedBoard != null)
                    {
                        removedBoard.Disconnect();
                        Boards.Remove(removedBoard);
                    }
                }
            }
        }

        private void createTreehopperFromDevice(UsbDevice device)
        {
            Log.Info(TAG, "createTreehopperFromDevice called");
            if (device.VendorId == 0x10c4 && device.ProductId == 0x8a7e)
            {
                if (Boards.Count(b => b.SerialNumber == device.SerialNumber) != 0)
                {
                    Log.Info(TAG, "device found. Not adding to list.");
                }
                else
                {
                    Log.Info(TAG, "device not found. Adding.");

                    Manager.RequestPermission(device, pendingIntent);
                }
            }
        }

        private Context context;
        private Activity activity;

        public void Scan()
        {
            if (context == null)
                return;

            pendingIntent = PendingIntent.GetBroadcast(context, 0, new Intent(ActionUsbPermission), 0);
            IntentFilter filter = new IntentFilter(ActionUsbPermission);
            context.RegisterReceiver(this, filter);

            foreach (var entry in Manager.DeviceList)
            {
                UsbDevice device = entry.Value;
                createTreehopperFromDevice(device);
            }
        }

        public void Dispose()
        {
            
        }
    }
}