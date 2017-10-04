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
        }

        protected override void OnStart()
        {
            base.OnStart();
            connectionService.ActivityOnStart(this);
        }

        protected override void OnResume()
        {
            base.OnResume();
            connectionService.ActivityOnResume();
        }
    }
}