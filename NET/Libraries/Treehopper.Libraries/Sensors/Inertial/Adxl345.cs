using System;
using System.Diagnostics;
using System.Numerics;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Sensors.Inertial
{
    public class Adxl345 : IAccelerometer
    {
        private Vector3 _accelerometer;
        private readonly SMBusDevice _dev;
        private readonly RegisterManager registers = new RegisterManager
        {
            { "POWER_CTL", 0x2D, RegisterAccess.WriteOnly, 
                new RegisterValue("Sleep", 2),
                new RegisterValue("Measure", 3) },

            { "DATAX", 0x32, RegisterAccess.ReadOnly,
                new RegisterValue("X", 0, 16, RegisterDepth.SignedShort) },

            { "DATAY", 0x34, RegisterAccess.ReadOnly,
                new RegisterValue("Y", 0, 16, RegisterDepth.SignedShort) },

            { "DATAZ", 0x36, RegisterAccess.ReadOnly,
                new RegisterValue("Z", 0, 16, RegisterDepth.SignedShort) },

            { "DATA_FORMAT", 0x31, RegisterAccess.WriteOnly,
                new RegisterValue("Range", 0, 2) },
        };

        public Adxl345(I2C i2c, bool altAddress = false, int rate = 100)
        {
            _dev = !altAddress ? new SMBusDevice(0x53, i2c, rate) : new SMBusDevice(0x1D, i2c, rate);
            registers.Dev = _dev;
            registers["POWER_CTL", "Sleep"] = 0;
            registers["POWER_CTL", "Measure"] = 1;
            registers["DATA_FORMAT", "Range"] = 0x03;
        }

        public bool AutoUpdateWhenPropertyRead { get; set; } = true;
        public int AwaitPollingInterval { get; set; }

        public async Task Update()
        {
            await registers.ReadRange("DATAX", "DATAZ");
            _accelerometer.X = registers["DATAX", "X"] * 0.04f;
            _accelerometer.Y = registers["DATAY", "Y"] * 0.04f;
            _accelerometer.Z = registers["DATAZ", "Z"] * 0.04f;
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
