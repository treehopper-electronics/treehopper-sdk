using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper;
using Treehopper.Libraries;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TreehopperControlCenter.Pages.Libraries
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class RotaryEncoder : LibraryComponent
    {
        public DigitalIn A { get; set; }
        public DigitalIn B { get; set; }
        public int Increment { get; set; } = 4;
        public TreehopperUsb Board { get; }

        public Treehopper.Libraries.Input.RotaryEncoder Encoder { get; set; }

        public RotaryEncoder (LibrariesPage page, TreehopperUsb Board = null) : base("Rotary Encoder", page)
		{
            this.Board = Board;
            Board.Connection.UpdateRate = 0; // go real fast!
            InitializeComponent();
            BindingContext = this;
		}

        public RotaryEncoder() : base("Rotary Encoder", new LibrariesPage())
        {
            InitializeComponent();
        }

        protected override async Task Start()
        {
            Encoder = new Treehopper.Libraries.Input.RotaryEncoder(A, B, Increment);
            OnPropertyChanged(nameof(Encoder));
        }

        protected override async Task Stop()
        {
            Encoder.Dispose();
            Encoder = null;
        }

        public override void Dispose()
        {
            
        }

        public override async Task Update()
        {
            // nothing here, since digital inputs are event-driven
        }
    }
}