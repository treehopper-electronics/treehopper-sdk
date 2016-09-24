using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Hardware.Usb;
using Android.OS;
using Treehopper;
namespace Blink
{
    [IntentFilter(new[] { UsbManager.ActionUsbDeviceAttached, UsbManager.ActionUsbDeviceDetached })]
    [MetaData(UsbManager.ActionUsbDeviceAttached, Resource = "@xml/device_filter")]
    [MetaData(UsbManager.ActionUsbDeviceDetached, Resource = "@xml/device_filter")]
    [Activity(Label = "Blink", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        int count = 1;

        TreehopperUsb board;

        protected async override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button>(Resource.Id.MyButton);

            GetSystemService(Context.UsbService);

            ConnectionService.Instance.Context = this.ApplicationContext; // we have to set the context before we do anything

            RegisterReceiver(ConnectionService.Instance, ConnectionService.Instance.UsbPermissionIntentFilter);

            board = await ConnectionService.Instance.GetFirstDeviceAsync();
            await board.ConnectAsync();

            board[0].Mode = PinMode.PushPullOutput;
            board[0].DigitalValue = false;

            button.Click += Button_Click;
        }

        private void Button_Click(object sender, EventArgs e)
        {
            board.Pins[0].DigitalValue = !board.Pins[0].DigitalValue;
        }
    }
}

