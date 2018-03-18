using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper;
using Treehopper.Libraries;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Treehopper.Libraries.Sensors.Temperature;
using Treehopper.Libraries.Sensors.Optical;
using Treehopper.Libraries.Sensors;

namespace TreehopperApp.Pages.Libraries
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Proximity : LibraryComponent
    {
        public string[] Sensors { get; set; } = new string[] { "VCNL4010" };
        public string SelectedSensor { get; set; }
        public TreehopperUsb Board { get; }

        public ProximitySensor Sensor { get; set; }

        public Proximity(LibrariesPage page, TreehopperUsb Board = null) : base("Proximity", page)
        {
            this.Board = Board;
            InitializeComponent();
            BindingContext = this;
        }

        public Proximity() : base("Proximity", new LibrariesPage())
        {
            InitializeComponent();
        }

        public override async Task Start()
        {
            switch (SelectedSensor)
            {
                case "VCNL4010":
                    Sensor = new Vcnl4010(Board.I2c);
                    break;
            }

            Sensor.AutoUpdateWhenPropertyRead = false;
            OnPropertyChanged(nameof(Sensor));
        }

        public override async Task Stop()
        {
            Sensor = null;
        }

        public override void Dispose()
        {

        }

        public override async Task Update()
        {
            if (Sensor == null) return;
            await Sensor.UpdateAsync().ConfigureAwait(false);
        }
    }
}