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

namespace TreehopperControlCenter.Pages.Libraries
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Temperature : LibraryComponent
    {
        public string[] Sensors { get; set; } = new string[] { "LM75", "DS18B20", "MCP9808" };
        public string SelectedSensor { get; set; }
        public TreehopperUsb Board { get; }

        public ITemperatureSensor Sensor { get; set; }

        public Temperature(LibrariesPage page, TreehopperUsb Board = null) : base("Temperature", page)
        {
            this.Board = Board;
            InitializeComponent();
            BindingContext = this;
        }

        public Temperature() : base("Temperature", new LibrariesPage())
        {
            InitializeComponent();
        }

        protected override async Task Start()
        {
            switch (SelectedSensor)
            {
                case "LM75":
                    Sensor = new Lm75(Board.I2c);
                    break;

                case "DS18B20":
                    Sensor = new Ds18b20(Board.Uart);
                    break;

                case "MCP9808":
                    Sensor = new Mcp9808(Board.I2c);
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