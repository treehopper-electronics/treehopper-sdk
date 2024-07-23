using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Threading.Tasks;
using Treehopper.Libraries.Sensors.Temperature;
using Treehopper.Utilities;

namespace Treehopper.Libraries.Sensors.Inertial
{
    /// <summary>
    ///     InvenSense MPU6050 6-DoF IMU
    /// </summary>
    [Supports("InvenSense", "MPU-6050")]
    public partial class Mpu6050 : TemperatureSensorBase, IAccelerometer, IGyroscope
    {
        internal SMBusDevice dev;

        internal Vector3 accelerometer;
        private Vector3 accelerometerOffset;

        internal Vector3 gyroscope;
        private Vector3 gyroscopeOffset;

        protected Mpu6050Registers _registers;

        /// <summary>
        /// Discover any MPU6050 IMUs (or optionally MPU9250s) attached to the specified bus.
        /// </summary>
        /// <param name="i2c">The bus to probe.</param>
        /// <param name="includeMpu9250">Whether to include MPU-9250 IMUs.</param>
        /// <param name="rate">The rate, in kHz, to use.</param>
        /// <returns>An awaitable task that completes with a list of of discovered sensors</returns>
        public static async Task<IList<Mpu6050>> ProbeAsync(I2C i2c, bool includeMpu9250 = false, int rate=100)
        {
            var deviceList = new List<Mpu6050>();
            try
            {
                var dev = new SMBusDevice(0x68, i2c, rate);
                var whoAmI = await dev.ReadByteDataAsync(0x75).ConfigureAwait(false);
                if (whoAmI == 0x68 || (whoAmI == 0x71 & includeMpu9250))
                    deviceList.Add(new Mpu6050(i2c, false));
            }
            catch (Exception ex) { }

            try
            {
                var dev = new SMBusDevice(0x69, i2c, rate);
                var whoAmI = await dev.ReadByteDataAsync(0x75).ConfigureAwait(false);
                if (whoAmI == 0x68 || (whoAmI == 0x71 & includeMpu9250))
                    deviceList.Add(new Mpu6050(i2c, true));
            }
            catch (Exception ex) { }

            return deviceList;
        }

        /// <summary>
        ///     Construct an MPU9150 9-Dof IMU
        /// </summary>
        /// <param name="i2c">The I2c module this module is connected to.</param>
        /// <param name="addressPin">The address of the module</param>
        /// <param name="ratekHz">The rate, in kHz, to use with this IC</param>
        public Mpu6050(I2C i2c, bool addressPin = false, int ratekHz = 400)
        {
            this.dev = new SMBusDevice((byte)(addressPin ? 0x69 : 0x68), i2c, ratekHz);
            this._registers = new Mpu6050Registers(new SMBusRegisterManagerAdapter(dev));
            Task.Run(async () =>
            {
                await _registers.powerMgmt1.readAsync();
                _registers.powerMgmt1.reset = 1;
                await _registers.powerMgmt1.writeAsync().ConfigureAwait(false);
                _registers.powerMgmt1.reset = 0;
                _registers.powerMgmt1.sleep = 0;
                await _registers.powerMgmt1.writeAsync().ConfigureAwait(false);
                _registers.powerMgmt1.clockSel = 1;
                await _registers.powerMgmt1.writeAsync().ConfigureAwait(false);
                _registers.configuration.dlpf = 3;
                await _registers.configuration.writeAsync().ConfigureAwait(false);
                _registers.sampleRateDivider.value = 4;
                await _registers.sampleRateDivider.writeAsync().ConfigureAwait(false);
                await _registers.accelConfig2.readAsync().ConfigureAwait(false);
                _registers.accelConfig2.accelFchoice = 0;
                _registers.accelConfig2.dlpfCfg = 3;
                await _registers.accelConfig2.writeAsync().ConfigureAwait(false);
                await _registers.powerMgmt1.readAsync().ConfigureAwait(false);
            }).Wait();
            AccelerometerScale = AccelScales.Fs_2g;
            GyroscopeScale = GyroScales.Dps_250;
        }

        /// <summary>
        ///     Gets or sets the accelerometer scale
        /// </summary>
        public AccelScales AccelerometerScale
        {
            get
            {
                return _registers.accelConfig.getAccelScale();
            }

            set
            {
                _registers.accelConfig.setAccelScale(value);
                Task.Run(_registers.accelConfig.writeAsync).Wait();
            }
        }

        /// <summary>
        ///     Gets or sets the gyroscope scale
        /// </summary>
        public GyroScales GyroscopeScale
        {
            get {
                return _registers.gyroConfig.getGyroScale();
            }

            set
            {
                _registers.gyroConfig.setGyroScale(value);
                Task.Run(_registers.gyroConfig.writeAsync).Wait();
            }
        }

        /// <summary>
        ///     Gets the accelerometer data
        /// </summary>
        public Vector3 Accelerometer
        {
            get
            {
                if (AutoUpdateWhenPropertyRead) UpdateAsync().Wait();
                return accelerometer;
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
            await _registers.readRange(_registers.accel_x, _registers.gyro_z).ConfigureAwait(false);
            var accelScale = getAccelScale();
            var gyroScale = getGyroScale();
            accelerometer.X = (float)(_registers.accel_x.value * accelScale - accelerometerOffset.X);
            accelerometer.Y = (float)(_registers.accel_y.value * accelScale - accelerometerOffset.Y);
            accelerometer.Z = (float)(_registers.accel_z.value * accelScale - accelerometerOffset.Z);

            celsius = _registers.temp.value / 333.87 + 21.0;

            gyroscope.X = (float) (_registers.gyro_x.value * gyroScale - gyroscopeOffset.X);
            gyroscope.Y = (float) (_registers.gyro_y.value * gyroScale - gyroscopeOffset.Y);
            gyroscope.Z = (float) (_registers.gyro_z.value * gyroScale - gyroscopeOffset.Z);

            RaisePropertyChanged(this);
            RaisePropertyChanged(this, nameof(Celsius));
            RaisePropertyChanged(this, nameof(Fahrenheit));
            RaisePropertyChanged(this, nameof(Kelvin));
        }

        /// <summary>
        ///     Gets the gyroscope data
        /// </summary>
        public Vector3 Gyroscope
        {
            get
            {
                if (AutoUpdateWhenPropertyRead) UpdateAsync().Wait();

                return gyroscope;
            }
        }

        /// <summary>
        ///     Calibrate IMU relative to gravity
        /// </summary>
        /// <returns></returns>
        public virtual async Task Calibrate()
        {
            var accelOffset = new Vector3();
            var gyroOffset = new Vector3();

            accelOffset.X = 0;
            accelOffset.Y = 0;
            accelOffset.Z = 0;

            gyroOffset.X = 0;
            gyroOffset.Y = 0;
            gyroOffset.Z = 0;

            for (var i = 0; i < 80; i++)
            {
                await UpdateAsync().ConfigureAwait(false); // get dater
                accelOffset.X += Accelerometer.X;
                accelOffset.Y += Accelerometer.Y;
                accelOffset.Z += Accelerometer.Z;

                gyroOffset.X += Gyroscope.X;
                gyroOffset.Y += Gyroscope.Y;
                gyroOffset.Z += Gyroscope.Z;
                await Task.Delay(10).ConfigureAwait(false);
            }

            accelOffset.X /= 80.0f;
            accelOffset.Y /= 80.0f;
            accelOffset.Z /= 80.0f;

            // subtract off gravity
            if (accelOffset.Z > 0.5) accelOffset.Z -= 1.0f;
            else if (accelOffset.Z < -0.5) accelOffset.Z += 1.0f;

            gyroOffset.X /= 80.0f;
            gyroOffset.Y /= 80.0f;
            gyroOffset.Z /= 80.0f;

            accelerometerOffset = accelOffset;
            gyroscopeOffset = gyroOffset;
        }


        internal double getAccelScale()
        {
            switch (AccelerometerScale)
            {
                case AccelScales.Fs_2g:
                    return 2.0 / 32768.0;
                case AccelScales.Fs_4g:
                    return 4.0 / 32768.0;
                case AccelScales.Fs_8g:
                    return 8.0 / 32768.0;
                case AccelScales.Fs_16g:
                    return 16.0 / 32768.0;
                default:
                    return 0;
            }
        }

        internal double getGyroScale()
        {
            switch (GyroscopeScale)
            {
                case GyroScales.Dps_250:
                    return 250.0 / 32768.0;
                case GyroScales.Dps_500:
                    return 500.0 / 32768.0;
                case GyroScales.Dps_1000:
                    return 1000.0 / 32768.0;
                case GyroScales.Dps_2000:
                    return 2000.0 / 32768.0;
                default:
                    return 0;
            }
        }
    }
}