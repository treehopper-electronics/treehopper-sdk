using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TreehopperControlCenter
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConnectedPage : TabbedPage
    {
        public ConnectedPage (TreehopperUsb Board)
        {
            Title = Board.ToString();

            Children.Add(new Pages.SignalPage(Board));
            Children.Add(new Pages.LibrariesPage(Board));

            NavigationPage.SetHasBackButton(this, false);

            InitializeComponent();
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}