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
        public string[] Sensors { get; set; } = new string[] { "DS18B20", "LM75", "MCP9808", "MLX90614", "MLX90615" };
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

        public override async Task Start()
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

                case "MLX90614":
                    var mlx90614 = new Mlx90614(Board.I2c);
                    Sensor = mlx90614.Object;
                    break;

                case "MLX90615":
                    var mlx90615 = new Mlx90615(Board.I2c);
                    Sensor = mlx90615.Object;
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