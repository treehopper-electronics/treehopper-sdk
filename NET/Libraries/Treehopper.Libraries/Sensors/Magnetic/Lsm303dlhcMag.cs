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
    /// Magnetic portion of the LSM303DLHC IMU
    /// </summary>
    public partial class Lsm303dlhcMag : TemperatureSensor, IMagnetometer
    {
        Lsm303dlhcMagRegisters registers;
        Vector3 _magnetometer;

        public override event PropertyChangedEventHandler PropertyChanged;

        public Vector3 Magnetometer
        {
            get
            {
                if (AutoUpdateWhenPropertyRead)
                    Task.Run(UpdateAsync);

                return _magnetometer;
            }
        }

        public Lsm303dlhcMag(I2C i2c, int rate=100)
        {
            registers = new Lsm303dlhcMagRegisters(new SMBusDevice(0x1E, i2c, rate));
            registers.mr.setMagSensorMode(MagSensorModes.ContinuousConversion);
            registers.crb.setGainConfiguration(GainConfigurations.gauss_1_3);
            registers.cra.tempEnable = 1;
            registers.cra.setMagDataRate(MagDataRates.Hz_100);
            Task.Run(() => registers.writeRange(registers.cra, registers.mr)).Wait();
        }

        public override async Task UpdateAsync()
        {
            await registers.readRange(registers.outX, registers.outZ).ConfigureAwait(false);
            await registers.tempOut.read().ConfigureAwait(false);

            var gain = getGainXYZ();
            _magnetometer.X = registers.outX.value / gain.Item1 * 100f / 16f;
            _magnetometer.Y = registers.outY.value / gain.Item1 * 100f / 16f;
            _magnetometer.Z = registers.outZ.value / gain.Item2 * 100f / 16f;

            Celsius = registers.tempOut.value;
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
