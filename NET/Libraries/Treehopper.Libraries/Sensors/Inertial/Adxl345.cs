using System;
using System.Diagnostics;
using System.Numerics;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Sensors.Inertial
{
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
            registers.WriteRange(registers.PowerCtl, registers.DataFormat).Wait();
        }

        public bool AutoUpdateWhenPropertyRead { get; set; } = true;
        public int AwaitPollingInterval { get; set; }

        public async Task Update()
        {
            await registers.ReadRange(registers.DataX, registers.DataZ);
            _accelerometer.X = registers.DataX.Value * 0.04f;
            _accelerometer.Y = registers.DataY.Value * 0.04f;
            _accelerometer.Z = registers.DataZ.Value * 0.04f;
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
