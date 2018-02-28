using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Numerics;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Sensors.Inertial
{
    [Supports("Analog Devices", "ADXL-345")]
    public partial class Adxl345 : IAccelerometer
    {
        private Vector3 _accelerometer;
        private readonly SMBusDevice _dev;
        private readonly Adxl345Registers registers;

        public Adxl345(I2C i2c, bool altAddress = false, int rate = 100)
        {
            _dev = new SMBusDevice((byte)(!altAddress ? 0x53 : 0x1D), i2c, rate);
            registers = new Adxl345Registers(_dev);
            registers.powerCtl.sleep = 0;
            registers.powerCtl.measure = 1;
            registers.dataFormat.range = 0x03;
            registers.dataFormat.fullRes = 1;
            Task.Run(() => registers.writeRange(registers.powerCtl, registers.dataFormat)).Wait();
        }

        public bool AutoUpdateWhenPropertyRead { get; set; } = true;
        public int AwaitPollingInterval { get; set; }

        public async Task UpdateAsync()
        {
            await registers.readRange(registers.dataX, registers.dataZ).ConfigureAwait(false);
            _accelerometer.X = registers.dataX.value / 255f;
            _accelerometer.Y = registers.dataY.value / 255f;
            _accelerometer.Z = registers.dataZ.value / 255f;
        }

        public Vector3 Accelerometer
        {
            get
            {
                if (AutoUpdateWhenPropertyRead) UpdateAsync().Wait();
                return _accelerometer;
            }
        }
    }
}
