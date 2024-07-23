using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Treehopper.Libraries.Sensors.Magnetic;
using Treehopper.Libraries.Sensors.Temperature;

namespace Treehopper.Libraries.Sensors.Inertial
{
    /// <summary>
    /// Bosch BNO055 9-DoF IMU with absolute orientation output
    /// </summary>
    [Supports("Bosch", "BNO055")]
    public partial class Bno055 : TemperatureSensorBase, IAccelerometer, IGyroscope, IMagnetometer
    {
        /// <summary>
        /// Probes the specified I2C bus to discover any BNO055 sensors attached.
        /// </summary>
        /// <param name="i2c">The I2C bus to probe</param>
        /// <param name="rateKhz">The rate, in kHz, to use</param>
        /// <returns>An awaitable task that completes with a list of BNO055 sensors.</returns>
        public static async Task<List<Bno055>> ProbeAsync(I2C i2c, int rateKhz = 100)
        {
            var deviceList = new List<Bno055>();

            try
            {
                var dev = new SMBusDevice(0x28, i2c, 100);
                var whoAmI = await dev.ReadByteDataAsync(0x00).ConfigureAwait(false);
                if (whoAmI == 0xA0)
                    deviceList.Add(new Bno055(i2c, false, rateKhz));
            }
            catch (Exception ex) { }

            try
            {
                var dev = new SMBusDevice(0x29, i2c, 100);
                var whoAmI = await dev.ReadByteDataAsync(0x00).ConfigureAwait(false);
                if (whoAmI == 0xA0)
                    deviceList.Add(new Bno055(i2c, true, rateKhz));
            }
            catch (Exception ex) { }

            return deviceList;
        }

        SMBusDevice dev;
        Bno055Registers registers;
        Vector3 accelerometer;
        Vector3 gyro;
        Vector3 magnetometer;
        Vector3 linearAcceleration;
        Vector3 gravity;
        Quaternion quaternion;
        EularAngles eularAngles;

        public EularAngles EularAngles
        {
            get
            {
                if (AutoUpdateWhenPropertyRead)
                    Task.Run(UpdateAsync).Wait();

                return eularAngles;
            }
        }

        public Quaternion Quaternion
        {
            get
            {
                if (AutoUpdateWhenPropertyRead)
                    Task.Run(UpdateAsync).Wait();

                return quaternion;
            }
        }

        public Vector3 LinearAcceleration
        {
            get
            {
                if (AutoUpdateWhenPropertyRead)
                    Task.Run(UpdateAsync).Wait();

                return linearAcceleration;
            }
        }

        public Vector3 Gravity
        {
            get
            {
                if (AutoUpdateWhenPropertyRead)
                    Task.Run(UpdateAsync).Wait();

                return gravity;
            }
        }

        public Vector3 Accelerometer
        {
            get
            {
                if (AutoUpdateWhenPropertyRead)
                    Task.Run(UpdateAsync).Wait();

                return accelerometer;
            }
        }

        public Vector3 Gyroscope
        {
            get
            {
                if (AutoUpdateWhenPropertyRead)
                    Task.Run(UpdateAsync).Wait();

                return gyro;
            }
        }

        public Vector3 Magnetometer
        {
            get
            {
                if (AutoUpdateWhenPropertyRead)
                    Task.Run(UpdateAsync).Wait();

                return magnetometer;
            }
        }

        public Bno055(I2C i2c, bool addressPin, int rateKhz = 100) : base()
        {
            if (addressPin)
                dev = new SMBusDevice(0x29, i2c, rateKhz);
            else
                dev = new SMBusDevice(0x28, i2c, rateKhz);

            registers = new Bno055Registers(new SMBusRegisterManagerAdapter(dev));

            registers.operatingMode.setOperatingMode(OperatingModes.ConfigMode);
            Task.Run(registers.operatingMode.writeAsync);
            registers.sysTrigger.resetSys = 1;
            Task.Run(registers.sysTrigger.writeAsync);
            registers.sysTrigger.resetSys = 0;
            int id = 0;
            do
            {
                Task.Run(registers.chipId.read).Wait();
                id = registers.chipId.value;
            }
            while (id != 0xA0);
            Task.Delay(50);
            registers.powerMode.setPowerMode(PowerModes.Normal);
            Task.Run(registers.powerMode.writeAsync);
            Task.Delay(10);
            registers.sysTrigger.selfTest = 0;
            Task.Run(registers.sysTrigger.writeAsync);
            Task.Delay(10);
            registers.operatingMode.setOperatingMode(OperatingModes.NineDegreesOfFreedom);
            Task.Run(registers.operatingMode.writeAsync);
            Task.Delay(20);
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
            await registers.readRange(registers.accelX, registers.temp).ConfigureAwait(false);

            accelerometer.X = registers.accelX.value / 1000f;
            accelerometer.Y = registers.accelY.value / 1000f;
            accelerometer.Z = registers.accelZ.value / 1000f;

            magnetometer.X = registers.magnetometerX.value / 16f;
            magnetometer.Y = registers.magnetometerY.value / 16f;
            magnetometer.Z = registers.magnetometerZ.value / 16f;

            gyro.X = registers.gyroX.value / 16f;
            gyro.Y = registers.gyroY.value / 16f;
            gyro.Z = registers.gyroZ.value / 16f;

            linearAcceleration.X = registers.linX.value / 100f;
            linearAcceleration.Y = registers.linY.value / 100f;
            linearAcceleration.Z = registers.linZ.value / 100f;

            gravity.X = registers.gravX.value / 100f;
            gravity.Y = registers.gravY.value / 100f;
            gravity.Z = registers.gravZ.value / 100f;

            eularAngles.Pitch = registers.eulPitch.value / 100f;
            eularAngles.Roll = registers.eulRoll.value / 100f;
            eularAngles.Yaw = registers.eulHeading.value / 100f;

            quaternion.W = registers.quaW.value / 16384f;
            quaternion.X = registers.quaX.value / 16384f;
            quaternion.Y = registers.quaY.value / 16384f;
            quaternion.Z = registers.quaZ.value / 16384f;

            celsius = (char)(registers.temp.value);

            RaisePropertyChanged(this);
            RaisePropertyChanged(this, "EularAngles");
            RaisePropertyChanged(this, "Quaternion");
            RaisePropertyChanged(this, "LinearAcceleration");
            RaisePropertyChanged(this, "Gravity");
            RaisePropertyChanged(this, "Accelerometer");
            RaisePropertyChanged(this, "Magnetometer");
            RaisePropertyChanged(this, "Gyroscope");
        }
    }
}
