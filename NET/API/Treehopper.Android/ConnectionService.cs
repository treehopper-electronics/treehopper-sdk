using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Hardware.Usb;

namespace Treehopper.Android
{
    public class ConnectionService : BroadcastReceiver, IConnectionService
    {
        static readonly ConnectionService instance = new ConnectionService();

        public static ConnectionService Instance => instance;

        public Context ApplicationContext { get; set; }

        public UsbManager Manager => (UsbManager)Context.GetSystemService(Context.UsbService);

        PendingIntent mPendingIntent;

        public string ActionUsbPermission = "com.treehopper.library.USB_PERMISSION";

        public ConnectionService() : base()
        {
            
        }

        public ObservableCollection<TreehopperUsb> Boards { get; set; } = new ObservableCollection<TreehopperUsb>();

        public event PropertyChangedEventHandler PropertyChanged;

        public async Task<TreehopperUsb> GetFirstDeviceAsync()
        {
            return Boards[0];
        }

        public override void OnReceive(Context context, Intent intent)
        {
            if (intent.Action == UsbManager.ActionUsbDeviceDetached || intent.Action == UsbManager.ActionUsbDeviceAttached)
            {
                UpdateBoardList();
            }
        }
        private Context context;
        public Context Context
        {
            get
            {
                return context;
            }

            set
            {
                if (context == value)
                    return;
                context = value;
                UpdateBoardList();
            }
        }

        public IntentFilter UsbPermissionIntentFilter = new IntentFilter("com.android.example.USB_PERMISSION");

        public void UpdateBoardList()
        {
            if (Context == null)
                return;

            // Create Treehopper boards from any new devices in the DeviceList
            var addedBoards = Manager.DeviceList.Values.Where(board => !Boards.Any(existingBoard => existingBoard.SerialNumber == board.SerialNumber));
            foreach(var newBoard in addedBoards)
            {
                Boards.Add(new TreehopperUsb(new UsbConnection(newBoard, Manager)));
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Boards"));
            }

            // close and remove Treehopper boards that aren't in the DeviceList
            var removedBoards = Boards.Where(board => !Manager.DeviceList.Values.Any(managerBoard => managerBoard.SerialNumber == board.SerialNumber));
            foreach(var removedBoard in removedBoards)
            {
                removedBoard.Disconnect();
                removedBoard.Dispose();
                Boards.Remove(removedBoard);
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Boards"));
            }
        }
    }
}