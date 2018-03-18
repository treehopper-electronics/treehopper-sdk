using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Treehopper;
using Xamarin.Forms.Xaml;

namespace TreehopperApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {

        public TreehopperUsb Board { get; set; }

		public MainPage()
		{
			InitializeComponent();
            Debug.WriteLine("Waiting for board...");
        }

        public async Task StartApp()
        {
            Board = await ConnectionService.Instance.GetFirstDeviceAsync();
            await Start();
            ConnectionService.Instance.Boards.CollectionChanged += Boards_CollectionChanged;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        private void Pins_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            ((ListView)sender).SelectedItem = null;
        }

        public async Task Start()
	    {
            await Board.ConnectAsync();
            Board.Connection.UpdateRate = 25;
            Debug.WriteLine("Board connected!");

            await Navigation.PushAsync(new ConnectedPage(Board));
        }

        private async void Boards_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if(e.Action == NotifyCollectionChangedAction.Add)
            {
                if (Board != null) return; // we already have a board, thank you.

                Board = (TreehopperUsb)e.NewItems[0];
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await Start();
                });


            } else if(e.Action == NotifyCollectionChangedAction.Remove)
            {
                Board.Disconnect();
                Device.BeginInvokeOnMainThread(() =>
                {
                    Navigation.PopToRootAsync();
                    //ledSwitch.Toggled -= LedSwitch_Toggled;
                    Debug.WriteLine("Board disconnected!");
                    //boardViewer.IsVisible = false;
                    connectMessage.IsVisible = true;
                    //Pins.Clear();
                    Board = null;
                });

                Debug.WriteLine("Board disconnected!");
                Board = null;
            }
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}
