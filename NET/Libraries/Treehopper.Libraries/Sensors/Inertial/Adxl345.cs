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
            registers.PowerCtl.Sleep = 0;
            registers.PowerCtl.Measure = 1;
            registers.DataFormat.Range = 0x03;
            registers.DataFormat.FullRes = 1;
            registers.WriteRange(registers.PowerCtl, registers.DataFormat);
        }

        public bool AutoUpdateWhenPropertyRead { get; set; } = true;
        public int AwaitPollingInterval { get; set; }

        public async Task Update()
        {
            await registers.ReadRange(registers.DataX, registers.DataZ).ConfigureAwait(false);
            _accelerometer.X = registers.DataX.Value / 255f;
            _accelerometer.Y = registers.DataY.Value / 255f;
            _accelerometer.Z = registers.DataZ.Value / 255f;
        }

        public Vector3 Accelerometer
        {
            get
            {
                if (AutoUpdateWhenPropertyRead) Update().Wait();
                return _accelerometer;
            }
        }
    }
}
