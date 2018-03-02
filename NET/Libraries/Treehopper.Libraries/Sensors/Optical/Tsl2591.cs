using System;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Sensors.Optical
{
    /// <summary>
    /// Avago/ams TSL2591 High-dynamic range digital light sensor
    /// </summary>
    [Supports("ams (Avago)", "TSL2591")]
    public partial class Tsl2591 : AmbientLightSensor
    {
        /// <summary>
        /// Probes the specified I2C bus to discover any TSL2591 sensors attached.
        /// </summary>
        /// <param name="i2c">The bus to probe.</param>
        /// <param name="rate">The rate to use.</param>
        /// <returns>A TSL2591 if found, or null if not found.</returns>
        public static async Task<Tsl2591> Probe(I2C i2c, int rate=100)
        {
            var dev = new SMBusDevice(0x29, i2c, rate);
            var result = await dev.ReadByteData(0xB2);
            if (result == 0x50)
                return new Tsl2591(i2c, rate);

            return null;
        }

        private Tsl2591Registers registers;

        /// <summary>
        ///     Construct a TSL2591 ambient light sensor
        /// </summary>
        /// <param name="i2c">The bus to use.</param>
        /// <param name="rate">The rate to use.</param>
        public Tsl2591(I2C i2c, int rate=100)
        {
            registers = new Tsl2591Registers(new SMBusDevice(0x29, i2c, rate));
            registers.enable.powerOn = 1;
            registers.enable.alsEnable = 1;
            Task.Run(registers.enable.write).Wait();
        }

        public int IntegrationTimeValue
        {
            get
            {
                return (registers.config.alsTime + 1) * 100;
            }
        }

        public int GainSettingValue
        {
            get
            {
                switch (registers.config.getAlsGain())
                {
                    case AlsGains.Low:
                    default:
                        return 1;
                    case AlsGains.Medium:
                        return 25;
                    case AlsGains.High:
                        return 428;
                    case AlsGains.Max:
                        return 9876;
                }
            }
        }

        /// <summary>
        ///     Get or set the integration time of this sensor
        /// </summary>
        public AlsTimes IntegrationTime
        {
            get
            {
                return registers.config.getAlsTime();
            }
            set
            {
                if (registers.config.getAlsTime() == value)
                    return;

                registers.config.setAlsTime(value);
                Task.Run(registers.config.write).Wait();
            }
        }

        /// <summary>
        ///     Get or set the gain setting
        /// </summary>
        public AlsGains GainSetting
        {
            get
            {
                return registers.config.getAlsGain();
            }
            set
            {
                if (registers.config.getAlsGain() == value)
                    return;
                registers.config.setAlsGain(value);
                Task.Run(registers.config.write).Wait();
            }
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
            await registers.readRange(registers.ch0, registers.ch1);
            double cpl = (IntegrationTimeValue * GainSettingValue) / 408.0;
            this._lux = (registers.ch0.value - registers.ch1.value) * (1.0 - ((double)registers.ch1.value / registers.ch0.value)) / cpl;
        }
    }
}