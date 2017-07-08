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

namespace TreehopperControlCenter
{
	public partial class MainPage : ContentPage
	{

        public TreehopperUsb Board { get; set; }

        public ObservableCollection<PinViewModel> Pins { get; set; } = new ObservableCollection<PinViewModel>();

		public MainPage()
		{
			InitializeComponent();
		    pins.ItemsSource = Pins;
            pins.ItemSelected += Pins_ItemSelected;

            Debug.WriteLine("Waiting for board...");
            ConnectionService.Instance.Boards.CollectionChanged += Boards_CollectionChanged;

            Pins.Add(new PinViewModel());
            Pins.Add(new PinViewModel());
            Pins.Add(new PinViewModel());
            Pins.Add(new PinViewModel());

            Pins[1].SelectedPinMode = "Digital Output";
            Pins[2].SelectedPinMode = "SoftPWM";
            Pins[3].SelectedPinMode = "Analog Input";

        }

        private void Pins_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            ((ListView)sender).SelectedItem = null;
        }

        public async Task Start()
	    {
            await Board.ConnectAsync();
            connectMessage.IsVisible = false;
            boardViewer.IsVisible = true;
            Board.Connection.UpdateRate = 25;
            ledSwitch.Toggled += LedSwitch_Toggled;
            Debug.WriteLine("Board connected!");
            Pins.Clear();
            foreach (var pin in Board.Pins)
                Pins.Add(new PinViewModel(pin));
        }

        private async void Boards_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if(e.Action == NotifyCollectionChangedAction.Add)
            {
                if (Board != null) return; // we already have a board, thank you.

                Board = (TreehopperUsb)e.NewItems[0];
                await Start();

            } else if(e.Action == NotifyCollectionChangedAction.Remove)
            {
                Board.Disconnect();
                ledSwitch.Toggled -= LedSwitch_Toggled;
                Debug.WriteLine("Board disconnected!");
                boardViewer.IsVisible = false;
                connectMessage.IsVisible = true;
                Pins.Clear();
                Board = null;

            }
        }

        private void LedSwitch_Toggled(object sender, ToggledEventArgs e)
        {
            Board.Led = e.Value;
        }
    }
}
