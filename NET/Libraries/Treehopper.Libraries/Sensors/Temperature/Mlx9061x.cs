using System.Threading.Tasks;

namespace Treehopper.Libraries.Sensors.Temperature
{
    /// <summary>
    /// Melexis MLX90614 non-contact IR thermopile temperature sensor
    /// </summary>
    public class Mlx90614
    {
        /// <summary>
        /// The SMBus device
        /// </summary>
        protected SMBusDevice dev;

        /// <summary>
        /// Construct a new MLX90614 attached to the given i2c port
        /// </summary>
        /// <param name="i2c">The i2c module to use</param>
        public Mlx90614(I2c i2c)
        {
            dev = new SMBusDevice(0x5A, i2c);
            Object =  new TempRegister(dev, 0x07);
            Ambient = new TempRegister(dev, 0x06);
        }

        /// <summary>
        /// The ambient temperature sensor
        /// </summary>
        public TemperatureSensor Ambient { get; protected set; }

        /// <summary>
        /// The non-contact thermopile temperature sensor
        /// </summary>
        public TemperatureSensor Object { get; protected set; }

        /// <summary>
        /// Raw (uncorrected) IR data from the thermopile array
        /// </summary>
        public int RawIrData => dev.ReadWordData(0x25).Result;

        internal class TempRegister : TemperatureSensor
        {

            private readonly SMBusDevice dev;
            private readonly byte register;

            internal TempRegister(SMBusDevice dev, byte register)
            {
                this.register = register;
                this.dev = dev;
            }

            /// <summary>
            /// Update the temperature register
            /// </summary>
            /// <returns>An awaitable task</returns>
            public override async Task Update()
            {
                var data = await dev.ReadWordData(register).ConfigureAwait(false);

                data &= 0x7FFF; // chop off the error bit of the high byte
                Celsius = data * 0.02 - 273.15;
            }
        }
    }

    /// <summary>
    /// MLX90615 non-contact IR thermopile temperature sensor
    /// </summary>
    public class Mlx90615 : Mlx90614
    {
        /// <summary>
        /// Construct a new MLX90615 attached to the given i2c port
        /// </summary>
        /// <param name="module"></param>
        public Mlx90615(I2c module) : base(module)
        {
            dev = new SMBusDevice(0x5B, module);
            Object = new TempRegister(dev, 0x27);
            Ambient = new TempRegister(dev, 0x26);
        }
    }
}
