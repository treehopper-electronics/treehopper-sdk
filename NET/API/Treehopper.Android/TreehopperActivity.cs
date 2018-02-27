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
    /// <summary>
    /// An activity that automatically manages TreehopperUsb connections.
    /// </summary>
    [Activity(Label = "TreehopperActivity")]
    public class TreehopperActivity : Activity
    {
        public ObservableCollection<TreehopperUsb> Boards => ConnectionService.Instance.Boards;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
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