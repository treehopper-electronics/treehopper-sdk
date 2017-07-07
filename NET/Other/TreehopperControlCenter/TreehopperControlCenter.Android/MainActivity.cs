using System;
using System.Collections.ObjectModel;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Hardware.Usb;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Treehopper.Android;
using Treehopper;
using Xamarin.Forms;

namespace TreehopperControlCenter.Droid
{
	[Activity (Label = "TreehopperControlCenter", Icon = "@drawable/icon", Theme="@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
	{
	    private ConnectionService connectionService = ConnectionService.Instance;

	    public ObservableCollection<TreehopperUsb> Boards => connectionService.Boards;

        protected override void OnCreate (Bundle bundle)
		{
			TabLayoutResource = Resource.Layout.Tabbar;
			ToolbarResource = Resource.Layout.Toolbar; 

			base.OnCreate (bundle);

		    connectionService.Context = ApplicationContext;

            global::Xamarin.Forms.Forms.Init (this, bundle);
			LoadApplication (new TreehopperControlCenter.App ());
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

