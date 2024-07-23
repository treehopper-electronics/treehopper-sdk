﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Treehopper.Libraries.Sensors.Temperature;
using Treehopper.Libraries.Utilities;
using Treehopper.Utilities;

namespace Treehopper.Libraries.Sensors.Pressure
{
    /// <summary>
    ///     Bosch BMP280 barometric pressure / altitude / temperature sensor.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This library supports both 4-wire SPI and 2-wire I2c operation
    ///     </para>
    /// </remarks>
    [Supports("Bosch", "BMP280")]
    public partial class Bmp280 : PressureSensor, ITemperatureSensor
    {
        public static async Task<IList<Bmp280>> ProbeAsync(I2C i2c, bool includeBme280 = true)
        {
            var deviceList = new List<Bmp280>();
            try
            {
                var dev = new SMBusDevice(0x76, i2c, 100);
                var whoAmI = await dev.ReadByteDataAsync(0xD0).ConfigureAwait(false);
                if (whoAmI == 0x58 || (whoAmI == 0x60 & includeBme280))
                    deviceList.Add(new Bmp280(i2c, false));
            }
            catch (Exception ex) { }

            try
            {
                var dev = new SMBusDevice(0x77, i2c, 100);
                var whoAmI = await dev.ReadByteDataAsync(0xD0).ConfigureAwait(false);
                if (whoAmI == 0x58 || (whoAmI == 0x60 & includeBme280))
                    deviceList.Add(new Bmp280(i2c, true));
            }
            catch (Exception ex) { }
            return deviceList;
        }

        protected Bmp280Registers registers;
        protected double tFine;
        private double altitude;
        private readonly SMBusDevice dev;

        /// <summary>
        ///     Construct a BMP280 hooked up to the i2C bus
        /// </summary>
        /// <param name="i2c">the i2C bus to use</param>
        /// <param name="sdoPin">the state of the SDO pin, which sets the address</param>
        public Bmp280(I2C i2c, bool sdoPin = false)
        {
            dev = new SMBusDevice((byte)(0x76 | (sdoPin ? 1 : 0)), i2c);
            registers = new Bmp280Registers(new SMBusRegisterManagerAdapter(dev));

            registers.ctrlMeasure.setMode(Modes.Normal);
            registers.ctrlMeasure.setOversamplingPressure(OversamplingPressures.Oversampling_x16);
            registers.ctrlMeasure.setOversamplingTemperature(OversamplingTemperatures.Oversampling_x16);
            Task.Run(registers.ctrlMeasure.writeAsync).Wait();

            Task.Run(() => registers.readRange(registers.t1, registers.h1)).Wait();
        }

        public double ReferencePressure { get; set; } = 101325;

        /// <summary>
        ///     Altitude, in meters
        /// </summary>
        public double Altitude
        {
            get
            {
                if (AutoUpdateWhenPropertyRead)
                    Task.Run(UpdateAsync).Wait();
                return altitude;
            }

            set { altitude = value; }
        }

        internal int PayloadSize { get; set; } = 6;
        internal byte[] LastReceivedData { get; set; }

        private double celsius;

        public double Celsius
        {
            get {
                if (AutoUpdateWhenPropertyRead)
                    Task.Run(UpdateAsync).Wait();
                return celsius;
            }
        }



        /// <summary>
        ///     Temperature, in degrees Fahrenheit
        /// </summary>
        public double Fahrenheit
        {
            get
            {
                return TemperatureSensorBase.ToFahrenheit(Celsius);
            }
        }

        /// <summary>
        ///     Temperature, in Kelvin
        /// </summary>
        public double Kelvin
        {
            get
            {
                return TemperatureSensorBase.ToKelvin(Celsius);
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
            await registers.readRange(registers.pressure, registers.humidity).ConfigureAwait(false); // even though this the BMP280, assume it's a BME280 so the bus is less chatty.

            // From Appendix A of the Bosch BMP280 datasheet
            var var1 = (registers.temperature.value / 16384.0 - registers.t1.value / 1024.0) * registers.t2.value;
            var var2 = ((registers.temperature.value / 131072.0 - registers.t1.value / 8192.0) * (registers.temperature.value / 131072.0 - registers.t1.value / 8192.0)) * registers.t3.value;
            tFine = (var1 + var2);
            celsius = (var1 + var2) / 5120.0;

            double p;
            var1 = tFine / 2.0 - 64000.0;
            var2 = var1 * var1 * registers.p6.value / 32768.0;
            var2 = var2 + var1 * registers.p5.value * 2.0;
            var2 = (var2 / 4.0) + registers.p4.value * 65536.0;
            var1 = (registers.p3.value * var1 * var1 / 524288.0 + registers.p2.value * var1) / 524288.0;
            var1 = (1.0 + var1 / 32768.0) * registers.p1.value;
            if (var1 == 0.0)
            {
                // avoid exception caused by division by zero
            }
            else
            {
                p = 1048576.0 - registers.pressure.value;
                p = (p - (var2 / 4096.0)) * 6250.0 / var1;
                var1 = registers.p9.value * p * p / 2147483648.0;
                var2 = p * registers.p8.value / 32768.0;
                p = p + (var1 + var2 + registers.p7.value) / 16.0;
                pascal = p;
                var kelvin = TemperatureSensorBase.ToKelvin(celsius);
                altitude = AltitudeFromPressure(kelvin, pascal);
            }

            RaisePropertyChanged(this, nameof(Celsius));
            RaisePropertyChanged(this, nameof(Fahrenheit));
            RaisePropertyChanged(this, nameof(Kelvin));
            RaisePropertyChanged(this, nameof(Altitude));
            RaisePropertyChanged(this);
        }

        private double AltitudeFromPressure(double temperature, double pressure)
        {
            var M = 0.0289644; // molar mass of earths' air
            var g = 9.80665; // gravity
            var R = 8.31432; // universal gas constant
            if (ReferencePressure / pressure < 101325 / 22632.1)
            {
                var d = -0.0065;
                var e = 0;
                var j = Math.Pow(pressure / ReferencePressure, R * d / (g * M));
                return e + temperature * (1 / j - 1) / d;
            }
            if (ReferencePressure / pressure < 101325 / 5474.89)
            {
                var e = 11000;
                var b = temperature - 71.5;
                var f = R * b * Math.Log(pressure / ReferencePressure) / (-g * M);
                var l = 101325;
                var c = 22632.1;
                var h = R * b * Math.Log(l / c) / (-g * M) + e;
                return h + f;
            }
            return double.NaN;
        }
    }
}