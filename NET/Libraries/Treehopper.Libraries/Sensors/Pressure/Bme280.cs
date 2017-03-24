using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper.Libraries.Sensors.Humidity;

namespace Treehopper.Libraries.Sensors.Pressure
{
    /// <summary>
    /// BME280 barometric pressure, temperature, and humidity sensor
    /// </summary>
    public class Bme280 : Bmp280, IHumiditySensor
    {
        private byte H1;
        private short H2;
        private byte H3;
        private short H4;
        private short H5;
        private byte H6;
        private double humidity;

        /// <summary>
        /// Construct a BMP280 hooked up to the i2C bus
        /// </summary>
        /// <param name="i2c">the i2C bus to use</param>
        /// <param name="sdo">the state of the SDO pin, which sets the address</param>
        public Bme280(I2c i2c, bool sdo = false) : base(i2c, sdo)
        {
            Start();
        }

        /// <summary>
        /// Construct a BMP280 hooked up to the SPI bus
        /// </summary>
        /// <param name="spi">the SPI bus to use</param>
        /// <param name="csPin">the chip select pin to use</param>
        /// <param name="speedMhz">the speed to operate at</param>
        public Bme280(Spi spi, SpiChipSelectPin csPin, double speedMhz) : base(spi, csPin, speedMhz)
        {
            Start();
        }

        /// <summary>
        /// The relative humidity
        /// </summary>
        public double RelativeHumidity
        {
            get
            {
                if (AutoUpdateWhenPropertyRead) Update().Wait();
                return humidity;
            }
        }

        public int AwaitPollingInterval { get; set; }

        /// <summary>
        /// Reads current data from the BME280
        /// </summary>
        /// <returns></returns>
        public override async Task Update()
        {
            // first the BMP280 stuff
            await base.Update();

            // now the BME stuff
            int adc_H = (LastReceivedData[6] << 8) | LastReceivedData[7];
            int v_x1_u32r;

            v_x1_u32r = (tFine - ((int)76800));

            v_x1_u32r = (((((adc_H << 14) - (((int)H4) << 20) -
                    (((int)H5) * v_x1_u32r)) + ((int)16384)) >> 15) *
                     (((((((v_x1_u32r * ((int)H6)) >> 10) *
                      (((v_x1_u32r * ((int)H3)) >> 11) + ((int)32768))) >> 10) +
                    ((int)2097152)) * ((int)H2) + 8192) >> 14));

            v_x1_u32r = (v_x1_u32r - (((((v_x1_u32r >> 15) * (v_x1_u32r >> 15)) >> 7) *
                           ((int)H1)) >> 4));

            v_x1_u32r = (v_x1_u32r < 0) ? 0 : v_x1_u32r;
            v_x1_u32r = (v_x1_u32r > 419430400) ? 419430400 : v_x1_u32r;
            humidity = (v_x1_u32r >> 12) / 1024.0;
        }

        private void Start()
        {
            PayloadSize = 8; // BME280 is 8-byte payload

            // read humidity calibration parameters
            H1 = Read(0xA1, 1).Result[0];
            var data = Read(0xE1, 7).Result;
            H2 = (short)(data[0] | (data[1] << 8));
            H3 = data[2];
            H4 = (short)(((data[3] << 4) | (data[4] & 0x0F)));
            H5 = (short)(((data[5] << 4) | (data[4] >> 4)));
            H6 = data[6];
        }
    }
}
