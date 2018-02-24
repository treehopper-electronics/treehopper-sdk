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
	[Activity (Label = "Treehopper", Icon = "@drawable/icon", Theme="@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
	{
	    public ObservableCollection<TreehopperUsb> Boards => ConnectionService.Instance.Boards;

        protected override void OnCreate (Bundle bundle)
		{
			TabLayoutResource = Resource.Layout.Tabbar;
			ToolbarResource = Resource.Layout.Toolbar; 

			base.OnCreate (bundle);

            global::Xamarin.Forms.Forms.Init (this, bundle);
			LoadApplication (new TreehopperControlCenter.App ());
		}

	    protected override void OnStart()
	    {
	        base.OnStart();

            ConnectionService.Instance.ActivityOnStart(this);
        }

        protected override void OnResume()
	    {
	        base.OnResume();

            ConnectionService.Instance.ActivityOnResume();
        }
    }
}

