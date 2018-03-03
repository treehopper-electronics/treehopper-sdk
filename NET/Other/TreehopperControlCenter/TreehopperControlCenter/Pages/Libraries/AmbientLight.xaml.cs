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

namespace TreehopperControlCenter.Pages.Libraries
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AmbientLight : LibraryComponent
    {
        public string[] Sensors { get; set; } = new string[] { "BH1750", "TSL2591", "VCNL4010" };
        public string SelectedSensor { get; set; }
        public TreehopperUsb Board { get; }

        public IAmbientLightSensor Sensor { get; set; }

        public AmbientLight(LibrariesPage page, TreehopperUsb Board = null) : base("Ambient Light Sensor", page)
        {
            this.Board = Board;
            InitializeComponent();
            BindingContext = this;
        }

        public AmbientLight() : base("Ambient Light Sensor", new LibrariesPage())
        {
            InitializeComponent();
        }

        protected override async Task Start()
        {
            switch (SelectedSensor)
            {
                case "BH1750":
                    Sensor = (await Bh1750.Probe(Board.I2c))[0];
                    break;

                case "TSL2591":
                    Sensor = await Tsl2591.Probe(Board.I2c);
                    break;

                case "VCNL4010":
                    Sensor = new Vcnl4010(Board.I2c);
                    break;
            }

            Sensor.AutoUpdateWhenPropertyRead = false;
            OnPropertyChanged(nameof(Sensor));
        }

        protected override async Task Stop()
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