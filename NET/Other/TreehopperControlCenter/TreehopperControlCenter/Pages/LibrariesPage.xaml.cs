using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TreehopperControlCenter.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LibrariesPage : ContentPage
	{
		public LibrariesPage (TreehopperUsb Board)
		{
			InitializeComponent ();
		}

        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}