using System;
using Android.Hardware.Usb;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Java.Util;
using System.Diagnostics;

namespace Treehopper
{
	public class ConnectionReceiver : BroadcastReceiver
	{
		public override void OnReceive (Context context, Intent intent)
		{
			if (intent.Action == TreehopperUsbConnection.ActionUsbPermission) {
				if (intent.GetBooleanExtra (UsbManager.ExtraPermissionGranted, false)) {
					TreehopperUsbConnection.AddBoard();
				}
			}

			
		}
	}

	public class TreehopperUsbConnection : ITreehopperConnection
	{
        #region Instance

        public TreehopperUsbConnection(UsbDevice device)
        {
            this.device = device;
			TreehopperUsbConnection.manager.RequestPermission(this.device, TreehopperUsbConnection.mPendingIntent);
        }

        public event PinEventData PinEventDataReceived;

        public UsbDevice device;

        public string SerialNumber
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public string DeviceName
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

		public static string ActionUsbPermission = "com.treehopper.library.USB_PERMISSION";

		UsbDeviceConnection connection;

		UsbEndpoint pinConfigEndpoint;
		UsbEndpoint pinReportEndpoint;
		UsbEndpoint peripheralConfigEndpoint;
		UsbEndpoint peripheralResponseEndpoint;

        public bool Open()
        {
			UsbInterface intf = device.GetInterface(0);
			pinReportEndpoint = intf.GetEndpoint (0);
			peripheralResponseEndpoint = intf.GetEndpoint (1);
			pinConfigEndpoint = intf.GetEndpoint(2);
			peripheralConfigEndpoint = intf.GetEndpoint (3);
			connection = TreehopperUsbConnection.manager.OpenDevice (this.device);
			if (connection != null) {
				if (connection.ClaimInterface (intf, true)) {
					return true;
				} else {
					return false; // couldn't claim interface
				}
			} else {
				return false; // openDevice failed
			}
        }

        public void SendDataPeripheralChannel(byte[] data)
        {
            throw new NotImplementedException();
        }

        public void SendDataPinConfigChannel(byte[] data)
        {
			connection.BulkTransfer (pinConfigEndpoint, data, data.Length, 1000);
        }
        #endregion

        #region ConnectionManager

        public static List<TreehopperUsbConnection> ConnectedDevices { get; set; }

        public static event ConnectionAddedHandler ConnectionAdded;

        public static event ConnectionRemovedHandler ConnectionRemoved;

        public static Context ApplicationContext { get; set; }

        static UsbManager manager;

		static ConnectionReceiver mUsbReceiver;

		static PendingIntent mPendingIntent;

		public static void AddBoard()
		{
			ConnectedDevices.Add (ConnectedDevice);
			if (ConnectionAdded != null)
				ConnectionAdded (ConnectedDevice);
		}

		public static TreehopperUsbConnection ConnectedDevice {get; set; }

        public static void StartConnectionManager()
        {
			ConnectedDevices = new List<TreehopperUsbConnection>();

			if (ApplicationContext != null)
            {
				mPendingIntent = PendingIntent.GetBroadcast(ApplicationContext, 0, new Intent(ActionUsbPermission), 0);
				IntentFilter filter = new IntentFilter(ActionUsbPermission);
				mUsbReceiver = new ConnectionReceiver ();
				ApplicationContext.RegisterReceiver (mUsbReceiver, filter);
				manager  = (UsbManager)ApplicationContext.GetSystemService(Context.UsbService);
                var deviceList = manager.DeviceList;
				foreach(var device in deviceList)
				{
					ConnectedDevice = new TreehopperUsbConnection(device.Value);
					break;
				}


            }


        }

        #endregion
    }
}

