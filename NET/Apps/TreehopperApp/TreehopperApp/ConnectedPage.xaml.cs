using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Xamarin.Forms.Xaml;

namespace TreehopperApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConnectedPage : Xamarin.Forms.TabbedPage
    {
        TreehopperUsb Board;
        public ConnectedPage (TreehopperUsb Board)
        {
            this.Board = Board;
            Title = Board.ToString();
            On<Xamarin.Forms.PlatformConfiguration.Android>().SetIsSwipePagingEnabled(false);
            Children.Add(new Pages.SignalPage(Board));
            Children.Add(new Pages.LibrariesPage(Board));

            NavigationPage.SetHasBackButton(this, false);

            InitializeComponent();
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            //Board.Dispose();
            //ConnectionService.Instance.Dispose();
        }
    }
}