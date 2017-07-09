using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TreehopperControlCenter.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SignalPage : ContentPage
	{
        public ObservableCollection<PinViewModel> Pins { get; set; } = new ObservableCollection<PinViewModel>();

        TreehopperUsb Board;

        public SignalPage (TreehopperUsb board)
		{
            Board = board;
			InitializeComponent ();

            ledSwitch.Toggled += LedSwitch_Toggled;

            foreach(var pin in board.Pins)
            {
                Pins.Add(new PinViewModel(pin));
            }

            pins.ItemsSource = Pins;
            pins.ItemSelected += Pins_ItemSelected;

        }

        private void Pins_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            ((ListView)sender).SelectedItem = null;
        }

        private void LedSwitch_Toggled(object sender, ToggledEventArgs e)
        {
            Board.Led = e.Value;
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}