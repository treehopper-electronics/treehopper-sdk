﻿using System;
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

namespace TreehopperControlCenter
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {

        public TreehopperUsb Board { get; set; }

		public MainPage()
		{
			InitializeComponent();

            Debug.WriteLine("Waiting for board...");
            Device.BeginInvokeOnMainThread(() =>
            {
                ConnectionService.Instance.Boards.CollectionChanged += Boards_CollectionChanged;
            });


            Pins.Add(new PinViewModel());
            Pins.Add(new PinViewModel());
            Pins.Add(new PinViewModel());
            Pins.Add(new PinViewModel());

            Pins[1].SelectedPinMode = "Digital Output";
            Pins[2].SelectedPinMode = "SoftPWM";
            Pins[3].SelectedPinMode = "Analog Input";
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            ConnectionService.Instance.Dispose();
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

        private void Boards_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
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
                await Navigation.PopToRootAsync();
                Board.Disconnect();
                Device.BeginInvokeOnMainThread(() =>
                {
                    ledSwitch.Toggled -= LedSwitch_Toggled;
                    Debug.WriteLine("Board disconnected!");
                    boardViewer.IsVisible = false;
                    connectMessage.IsVisible = true;
                    Pins.Clear();
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
