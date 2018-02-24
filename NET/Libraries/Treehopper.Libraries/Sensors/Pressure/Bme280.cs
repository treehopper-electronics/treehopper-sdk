using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Treehopper.Libraries.Sensors.Humidity;

namespace Treehopper.Libraries.Sensors.Pressure
{
    /// <summary>
    ///     Bosch BME280 barometric pressure, temperature, and humidity sensor
    /// </summary>
    [Supports("Bosch", "BME280")]
    public class Bme280 : Bmp280, IHumiditySensor
    {
        public static async Task<IList<Bme280>> Probe(I2C i2c)
        {
            var deviceList = new List<Bme280>();
            try
            {
                var dev = new SMBusDevice(0x76, i2c, 100);
                var whoAmI = await dev.ReadByteData(0xD0).ConfigureAwait(false);
                if (whoAmI == 0x60)
                    deviceList.Add(new Bme280(i2c, false));
            }
            catch (Exception ex) { }

            try
            {
                var dev = new SMBusDevice(0x77, i2c, 100);
                var whoAmI = await dev.ReadByteData(0xD0).ConfigureAwait(false);
                if (whoAmI == 0x60)
                    deviceList.Add(new Bme280(i2c, true));
            }
            catch (Exception ex) { }
            return deviceList;
        }

        private double humidity;
        private short h4, h5;
        /// <summary>
        ///     Construct a BMP280 hooked up to the i2C bus
        /// </summary>
        /// <param name="i2c">the i2C bus to use</param>
        /// <param name="sdo">the state of the SDO pin, which sets the address</param>
        public Bme280(I2C i2c, bool sdo = false) : base(i2c, sdo)
        {
            Task.Run(() => registers.readRange(registers.h2, registers.h6)).Wait();

            // RegisterGenerator doesn't get the endianness right on the h4/h5 12-bit values, so manually create them:
            h4 = (short)((short)((registers.h4.value << 4 | registers.h4h5.h4Low) << 4) >> 4);
            h5 = (short)((short)((registers.h5.value << 4 | registers.h4h5.h5Low) << 4) >> 4);

            registers.ctrlHumidity.setOversampling(Oversamplings.Oversampling_x16);
            Task.Run(registers.ctrlHumidity.write).Wait();
            Task.Run(registers.ctrlMeasure.write).Wait();
        }

        /// <summary>
        ///     The relative humidity
        /// </summary>
        public double RelativeHumidity
        {
            get
            {
                if (AutoUpdateWhenPropertyRead) Update().Wait();
                return humidity;
            }
        }

        /// <summary>
        ///     Reads current data from the BME280
        /// </summary>
        /// <returns></returns>
        public override async Task Update()
        {
            // first the BMP280 stuff
            await base.Update();

            // now the BME stuff
            double var_H;

            var_H = tFine - 76800.0;
            var_H = (registers.humidity.value - (h4 * 64.0 + h5 / 16384.0 * var_H)) *
                registers.h2.value / 65536.0 * 
                (1.0 + registers.h6.value / 67108864.0 * var_H *
                (1.0 + registers.h3.value / 67108864.0 * var_H));

            var_H = var_H * (1.0 - registers.h1.value * var_H / 524288.0);

            if (var_H > 100.0)
                var_H = 100.0;
            else if (var_H < 0.0)
                var_H = 0.0;

            humidity = var_H;
        }
    }
}