using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Treehopper.Libraries;
using Treehopper.Libraries.Sensors.Inertial;

namespace TreehopperControlCenter.Pages.Libraries
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Accelerometer : LibraryComponent
    {
        public string SelectedAccelerometer { get; set; }

        public string[] Accelerometers { get; set; } = new string[] { "MPU-9250", "ADXL-345", "LIS3DH", "BNO055", "LSM303DLHC" };

        public TreehopperUsb Board { get; }

        public IAccelerometer Sensor { get; set; }

        //public double AccelerometerX { get; set; }
        public double AccelerometerX => Sensor?.Accelerometer.X ?? 0;
        public double AccelerometerY => Sensor?.Accelerometer.Y ?? 0;
        public double AccelerometerZ => Sensor?.Accelerometer.Z ?? 0;

        public Accelerometer(LibrariesPage page, TreehopperUsb Board = null) : base("Accelerometer", page)
        {
            this.Board = Board;
            InitializeComponent();
            BindingContext = this;
        }

        public Accelerometer() : base("Accelerometer", new LibrariesPage())
        {
            InitializeComponent();
            BindingContext = this;
        }

        protected override async Task Start()
        {
            switch(SelectedAccelerometer)
            {
                case "MPU-9250":
                    Sensor = (await Mpu6050.ProbeAsync(Board.I2c, true))[0];
                    break;

                case "ADXL-345":
                    Sensor = (await Adxl345.ProbeAsync(Board.I2c))[0];
                    break;

                case "LIS3DH":
                    Sensor = (await Lis3dh.ProbeAsync(Board.I2c))[0];
                    break;

                case "BNO055":
                    Sensor = (await Bno055.ProbeAsync(Board.I2c))[0];
                    break;

                case "LSM303DLHC":
                    var lsm303dlhc = new Lsm303dlhc(Board.I2c);
                    Sensor = lsm303dlhc.Accel;
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
            Sensor = null;
        }

        public override async Task Update()
        {
            if(Sensor != null)
            {
                await Sensor.UpdateAsync().ConfigureAwait(false);

                //AccelerometerX = Sensor.Accelerometer.X;


                OnPropertyChanged(nameof(AccelerometerX));
                OnPropertyChanged(nameof(AccelerometerY));
                OnPropertyChanged(nameof(AccelerometerZ));
            }
                
        }
    }
}