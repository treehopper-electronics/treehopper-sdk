using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Sensors.Optical
{
    public partial class Tsl2561 : AmbientLightSensor
    {
        /// <summary>
        /// Probes the specified I2C bus to discover any TSL2561 sensors attached.
        /// </summary>
        /// <param name="i2c">The bus to probe.</param>
        /// <param name="rate">The rate to use. Defaults to 100.</param>
        /// <returns>A TSL2561 if found, or null if not found.</returns>
        public static async Task<Tsl2561> ProbeAsync(I2C i2c, int rate = 100)
        {
            var dev = new SMBusDevice(0x29, i2c, rate);
            var result = await dev.ReadByteDataAsync(0x8A);
            if (result == 0x50)
                return new Tsl2561(i2c, AddrSel.Gnd, rate);

            dev = new SMBusDevice(0x39, i2c, rate);
            result = await dev.ReadByteDataAsync(0x8A);
            if (result == 0x50)
                return new Tsl2561(i2c, AddrSel.Float, rate);

            dev = new SMBusDevice(0x49, i2c, rate);
            result = await dev.ReadByteDataAsync(0x8A);
            if (result == 0x50)
                return new Tsl2561(i2c, AddrSel.Vdd, rate);

            return null;
        }

        public enum AddrSel
        {
            /// <summary>
            /// AddrSel is connected to GND; device address is 0x29
            /// </summary>
            Gnd = 0x29,

            /// <summary>
            /// AddrSel is floating; device address is 0x39
            /// </summary>
            Float = 0x39,

            /// <summary>
            /// AddrSel is connected to vdd; device address is 0x49
            /// </summary>
            Vdd = 0x49
        }

        public Tsl2561(I2C i2c, AddrSel addrSel, int rate = 100) : base()
        {
            registers = new Tsl2561Registers(new SMBusRegisterManagerAdapter(new SMBusDevice((byte)addrSel, i2c, rate)));
            registers.control.setPower(Powers.powerUp);
            registers.control.write().Wait();
            IntegrationTime = IntegrationTimings.Time_402ms;
            HighGain = true;
        }

        private Tsl2561Registers registers;

        public IntegrationTimings IntegrationTime
        {
            get
            {
                return registers.timing.getIntegrationTiming();
            }

            set
            {
                registers.timing.setIntegrationTiming(value);
                registers.timing.write().Wait();
            }
        }

        public bool HighGain
        {
            get
            {
                return registers.timing.gain == 1;
            }

            set
            {
                registers.timing.gain = (value ? 1 : 0);
                registers.timing.write().Wait();
            }
        }

        public int RawVisible
        {
            get
            {
                if (AutoUpdateWhenPropertyRead)
                    UpdateAsync().Wait();
                return registers.data0.value;
            }
        }

        public int RawIr
        {
            get
            {
                if (AutoUpdateWhenPropertyRead)
                    UpdateAsync().Wait();
                return registers.data1.value;
            }
        }

        public async override Task UpdateAsync()
        {
            await registers.data0.read().ConfigureAwait(false);
            await registers.data1.read().ConfigureAwait(false);

            var ch0 = (double)registers.data0.value;
            var ch1 = (double)registers.data1.value;

            var ratio = ch1 / ch0;
            if(ratio <= 0.5)
            {
                _lux = 0.0304 * ch0 - 0.062 * Math.Pow(ratio, 1.4);
            }
            else if(ratio <= 0.61)
            {
                _lux = 0.0224 * ch0 - 0.031 * ch1;
            }
            else if(ratio <= 0.81)
            {
                _lux = 0.0128 * ch0 - 0.0153 * ch1;
            }
            else if(ratio <= 1.3)
            {
                _lux = 0.00146 * ch0 - 0.00112 * ch1;
            }
            else
            {
                _lux = 0;
            }

            if(!HighGain)
            {
                _lux *= 16;
            }

            if(IntegrationTime == IntegrationTimings.Time_101ms)
            {
                _lux *= (402.0 / 101.0);
            } else if(IntegrationTime == IntegrationTimings.Time_13_7ms)
            {
                _lux *= (402.0 / 13.7);
            }
        }
    }
}
