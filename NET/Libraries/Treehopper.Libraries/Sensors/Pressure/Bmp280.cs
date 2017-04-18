namespace Treehopper.Libraries.Sensors.Pressure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Treehopper.Libraries.Sensors.Temperature;
    using Utilities;
    using Treehopper.Utilities;
    /// <summary>
    /// Bosch BMP280 barometric pressure / altitude / temperature sensor.
    /// </summary>
    /// <remarks>
    /// <para>This library supports both 4-wire SPI and 2-wire I2c operation
    /// </para></remarks>
    public class Bmp280 : PressureSensor, ITemperatureSensor
    {
        private SMBusDevice i2cDev;
        private SpiDevice spiDev;
        private Trimming trimming;
        internal int tFine;

        /// <summary>
        /// Construct a BMP280 hooked up to the i2C bus
        /// </summary>
        /// <param name="i2c">the i2C bus to use</param>
        /// <param name="sdo">the state of the SDO pin, which sets the address</param>
        public Bmp280(I2c i2c, bool sdo = false)
        {
            i2cDev = new SMBusDevice((byte)(0x76 | (sdo ? 1 : 0)), i2c);
            Start();
        }

        /// <summary>
        /// Construct a BMP280 hooked up to the SPI bus
        /// </summary>
        /// <param name="spi">the SPI bus to use</param>
        /// <param name="csPin">the chip select pin to use</param>
        /// <param name="speedMhz">the speed to operate at</param>
        public Bmp280(Spi spi, SpiChipSelectPin csPin, double speedMhz = 6)
        {
            spiDev = new SpiDevice(spi, csPin, ChipSelectMode.SpiActiveLow, speedMhz, SpiMode.Mode00);
            Start();
        }

        //public enum Filter
        //{
        //    FilterOff,
        //    Filter2,
        //    Filter4,
        //    Filter8,
        //    Filter16
        //}

        internal enum Registers
        {
            ChipId = 0xD0,
            Rst = 0xE0,
            Stat = 0xF3,
            CtrlMeasure = 0xF4,
            Config = 0xF5,
            PressureMsb = 0xF7,
            PressureLsb = 0xF8,
            PressureXlsb = 0xF9,
            TemperatureMsb = 0xFA,
            TemperatureLsb = 0xFB,
            TemperatureXlsb = 0xFC,
            TrimmingStart = 0x88,
        }

#pragma warning disable 649
        internal struct Trimming
        {
            public ushort T1;
            public short T2;
            public short T3;
            public ushort P1;
            public short P2;
            public short P3;
            public short P4;
            public short P5;
            public short P6;
            public short P7;
            public short P8;
            public short P9;
        }
#pragma warning restore 649

        internal byte ChipId { get { return 0x58; } }

        /// <summary>
        /// Temperature, in degrees Celsius
        /// </summary>
        public double Celsius { get; protected set; }

        private double altitude;

        /// <summary>
        /// Altitude, in meters
        /// </summary>
        public double Altitude
        {
            get
            {
                if (AutoUpdateWhenPropertyRead) Update().Wait();
                return altitude;
            }

            set
            {
                altitude = value;
            }
        }

        /// <summary>
        /// Temperature, in degrees Fahrenheit
        /// </summary>
        public double Fahrenheit
        {
            get
            {
                return TemperatureSensor.ToFahrenheit(Celsius);
            }
        }

        /// <summary>
        /// Temperature, in Kelvin
        /// </summary>
        public double Kelvin
        {
            get
            {
                return TemperatureSensor.ToKelvin(Celsius);
            }
        }

        internal int PayloadSize { get; set; } = 6;
        internal byte[] LastReceivedData { get; set; }

        private void Start()
        {
            if (Read((byte)Registers.ChipId, 1).Result[0] != ChipId)
                Utility.Error("BMP280 not found. Are you sure you're not using a BME280?");

            Write((byte)Registers.CtrlMeasure, 0x3F).Wait();
            var trimmingParameters = Read((byte)Registers.TrimmingStart, 24).Result;
            trimming = StructConverter.BytesToStruct<Trimming>(trimmingParameters, Endianness.LittleEndian);
        }

        internal Task Write(byte reg, byte val)
        {
            return i2cDev?.WriteByteData(reg, val) ?? spiDev?.SendReceive(new byte[] { (byte)(reg | 0x80), val });
        }

        internal async Task<byte[]> Read(byte reg, int numBytesToRead)
        {
            return (await i2cDev?.ReadBufferData(reg, numBytesToRead)) ?? (await spiDev?.SendReceive(new byte[] { (byte)Registers.TrimmingStart }.Concat(new byte[24]).ToArray())).Skip(1).Take(numBytesToRead).ToArray();
        }

        /// <summary>
        /// Update the sensor's value
        /// </summary>
        /// <returns>An awaitable task</returns>
        public override async Task Update()
        {
            // do a burst read
            var data = await Read((byte)Registers.PressureMsb, PayloadSize);

            // weirdo 20-bit datatypes, so just handle the bits manually
            int uPressure = ((data[0] << 0x10) | (data[1] << 0x08) | data[2]) >> 4;
            int uTemp = ((data[3] << 0x10) | (data[4] << 0x08) | data[5]) >> 4;

            // From Bosch BMP280 datasheet
            int tVar1 = (((uTemp >> 3) - (trimming.T1 << 1)) * trimming.T2) >> 11;
            int tVar2 = (((((uTemp >> 4) - trimming.T1) * ((uTemp >> 4) - trimming.T1)) >> 12) * trimming.T3) >> 14;
            tFine = tVar1 + tVar2;
            double temperature = ((tFine * 5 + 128) >> 8) / 100.0;
            Celsius = temperature;


            long var1 = ((long)tFine) - 128000;
            long var2 = var1 * var1 * trimming.P6;
            var2 = var2 + ((var1 * trimming.P5) << 17);
            var2 = var2 + ((long)(trimming.P4) << 35);
            var1 = ((var1 * var1 * trimming.P3) >> 8) +
              ((var1 * trimming.P2) << 12);
            var1 = ((((long)1 << 47) + var1)) * (trimming.P1) >> 33;

            if (var1 != 0)
            {
                long p = 1048576 - uPressure;
                p = (((p << 31) - var2) * 3125) / var1;
                var1 = ((trimming.P9) * (p >> 13) * (p >> 13)) >> 25;
                var2 = ((trimming.P8) * p) >> 19;

                p = ((p + var1 + var2) >> 8) + ((long)trimming.P7 << 4);
                double pressure = p / 256.0;
                Pascal = pressure;

                // calculate altitutde
                double kelvin = TemperatureSensor.ToKelvin(temperature);
                altitude = AltitudeFromPressure(kelvin, pressure);
            }

            LastReceivedData = data; // hand off the data to any inherited classes
        }

        private double AltitudeFromPressure(double temperature, double pressure, double seaLevelPressure = 101325)
        {
            var M = 0.0289644; // molar mass of earths' air
            var g = 9.80665; // gravity
            var R = 8.31432; // universal gas constant
            if ((seaLevelPressure / pressure) < (101325 / 22632.1))
            {
                var d = -0.0065;
                var e = 0;
                var j = Math.Pow((pressure / seaLevelPressure), (R * d) / (g * M));
                return e + ((temperature * ((1 / j) - 1)) / d);
            }
            else
            {
                if ((seaLevelPressure / pressure) < (101325 / 5474.89))
                {
                    var e = 11000;
                    var b = temperature - 71.5;
                    var f = (R * b * (Math.Log(pressure / seaLevelPressure))) / ((-g) * M);
                    var l = 101325;
                    var c = 22632.1;
                    var h = ((R * b * (Math.Log(l / c))) / ((-g) * M)) + e;
                    return h + f;
                }
            }
            return double.NaN;
        }

    }
}