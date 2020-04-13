using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Treehopper.Libraries.Sensors.Temperature;

namespace Treehopper.Libraries.Sensors.Magnetic
{
    /// <summary>
    /// Magnetic portion of the ST LSM303DLHC IMU
    /// </summary>
    public partial class Lsm303dlhcMag : TemperatureSensorBase, IMagnetometer
    {
        Lsm303dlhcMagRegisters registers;
        Vector3 _magnetometer;

        /// <summary>
        /// Gets the three-axis magnetometer value, in uT (micro-Tesla)
        /// </summary>
        public Vector3 Magnetometer
        {
            get
            {
                if (AutoUpdateWhenPropertyRead)
                    Task.Run(UpdateAsync);

                return _magnetometer;
            }
        }

        /// <summary>
        /// Construct the magnetometer portion of an LSM303DLHC attached to the specified bus.
        /// </summary>
        /// <param name="i2c">The bus to use.</param>
        /// <param name="rate">The rate, in kHz, to use.</param>
        public Lsm303dlhcMag(I2C i2c, int rate=100)
        {
            registers = new Lsm303dlhcMagRegisters(new SMBusRegisterManagerAdapter(new SMBusDevice(0x1E, i2c, rate)));
            registers.mr.setMagSensorMode(MagSensorModes.ContinuousConversion);
            registers.crb.setGainConfiguration(GainConfigurations.gauss_1_3);
            registers.cra.tempEnable = 1;
            registers.cra.setMagDataRate(MagDataRates.Hz_100);
            Task.Run(() => registers.writeRange(registers.cra, registers.mr)).Wait();
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
            await registers.readRange(registers.outX, registers.outZ).ConfigureAwait(false);
            await registers.tempOut.read().ConfigureAwait(false);

            var gain = getGainXYZ();
            _magnetometer.X = registers.outX.value / gain.Item1 * 100f / 16f;
            _magnetometer.Y = registers.outY.value / gain.Item1 * 100f / 16f;
            _magnetometer.Z = registers.outZ.value / gain.Item2 * 100f / 16f;

            celsius = registers.tempOut.value;

            RaisePropertyChanged(this);
            RaisePropertyChanged(nameof(Magnetometer));
        }

        private Tuple<float, float> getGainXYZ()
        {
            switch(registers.crb.getGainConfiguration())
            {
                case GainConfigurations.gauss_1_9:
                    return new Tuple<float, float>(855, 760);
                case GainConfigurations.gauss_2_5:
                    return new Tuple<float, float>(670, 600);
                case GainConfigurations.gauss_4_0:
                    return new Tuple<float, float>(450, 400);
                case GainConfigurations.gauss_4_7:
                    return new Tuple<float, float>(400, 355);
                case GainConfigurations.gauss_5_6:
                    return new Tuple<float, float>(330, 295);
                case GainConfigurations.gauss_8_1:
                    return new Tuple<float, float>(230, 205);
                default:
                case GainConfigurations.gauss_1_3:
                    return new Tuple<float, float>(1100, 980);
            }
        }
    }
}
