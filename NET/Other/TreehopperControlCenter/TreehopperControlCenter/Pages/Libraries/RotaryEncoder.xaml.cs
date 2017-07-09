using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TreehopperControlCenter.Pages.Libraries
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class RotaryEncoder : LibraryComponent
    {
		public RotaryEncoder (LibrariesPage page, TreehopperUsb Board = null) : base("Rotary Encoder", page)
		{
			InitializeComponent();
		}
	}
}