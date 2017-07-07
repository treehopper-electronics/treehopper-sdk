using System;
using System.Collections.Generic;
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
		    Connect();
            
            Pins.Add(new PinViewModel());
		}

	    public async Task Connect()
	    {
	        Debug.WriteLine("Waiting for board...");
Board = await ConnectionService.Instance.GetFirstDeviceAsync();
            
            await Board.ConnectAsync();
            Board.Connection.UpdateRate = 25;
            ledSwitch.Toggled += LedSwitch_Toggled;
            Debug.WriteLine("Board connected!");
	        Pins.Clear();
            foreach(var pin in Board.Pins)
                Pins.Add(new PinViewModel(pin));
	    }

        private void LedSwitch_Toggled(object sender, ToggledEventArgs e)
        {
            Board.Led = e.Value;
        }
    }
}
