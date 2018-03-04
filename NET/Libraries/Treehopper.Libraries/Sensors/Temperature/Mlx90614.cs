using System.ComponentModel;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Sensors.Temperature
{
    /// <summary>
    ///     Melexis MLX90614 non-contact IR thermopile temperature sensor
    /// </summary>
    public class Mlx90614
    {
        /// <summary>
        ///     The SMBus device
        /// </summary>
        protected SMBusDevice dev;

        /// <summary>
        ///     Construct a new MLX90614 attached to the given i2c port
        /// </summary>
        /// <param name="i2c">The i2c module to use</param>
        public Mlx90614(I2C i2c)
        {
            dev = new SMBusDevice(0x5A, i2c);
            Object = new TempRegister(dev, 0x07);
            Ambient = new TempRegister(dev, 0x06);
        }

        /// <summary>
        ///     The ambient temperature sensor
        /// </summary>
        public TemperatureSensorBase Ambient { get; protected set; }

        /// <summary>
        ///     The non-contact thermopile temperature sensor
        /// </summary>
        public TemperatureSensorBase Object { get; protected set; }

        /// <summary>
        ///     Raw (uncorrected) IR data from the thermopile array
        /// </summary>
        public int RawIrData => dev.ReadWordDataAsync(0x25).Result;

        internal class TempRegister : TemperatureSensorBase
        {
            private readonly SMBusDevice dev;
            private readonly byte register;

            internal TempRegister(SMBusDevice dev, byte register)
            {
                this.register = register;
                this.dev = dev;
            }

            /// <summary>
            /// Requests a reading from the sensor and updates its data properties with the gathered values.
            /// </summary>
            /// <returns>An awaitable Task</returns>
            /// <remarks>
            /// Note that when #AutoUpdateWhenPropertyRead is `true` (which it is, by default), this method is implicitly 
            /// called when any sensor data property is read from --- there's no need to call this method unless you set
            /// AutoUpdateWhenPropertyRead to `false`.
            /// 
            /// Unless otherwise noted, this method updates all sensor data simultaneously, which can often lead to more efficient
            /// bus usage (as well as reducing USB chattiness).
            /// </remarks>
            public override async Task UpdateAsync()
            {
                var data = await dev.ReadWordDataAsync(register);

                data &= 0x7FFF; // chop off the error bit of the high byte
                celsius = data * 0.02 - 273.15;

                RaisePropertyChanged(this);
            }
        }
    }
}