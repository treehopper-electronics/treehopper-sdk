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
using Treehopper.Libraries.Sensors.Pressure;

namespace TreehopperApp.Pages.Libraries
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Altitude : LibraryComponent
    {
        public string[] Sensors { get; set; } = new string[] { "BMP280/BME280" };
        public string SelectedSensor { get; set; }
        public TreehopperUsb Board { get; }

        public Bmp280 Sensor { get; set; }

        public Altitude(LibrariesPage page, TreehopperUsb Board = null) : base("Altitude", page)
        {
            this.Board = Board;
            InitializeComponent();
            BindingContext = this;
        }

        public Altitude() : base("Altitude", new LibrariesPage())
        {
            InitializeComponent();
        }

        public override async Task Start()
        {
            switch (SelectedSensor)
            {
                case "BMP280/BME280":
                    Sensor = (await Bmp280.ProbeAsync(Board.I2c, true))[0];
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