using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Sensors.Magnetic
{
    public partial class Ak8975 : IMagnetometer
    {
        SMBusDevice dev;
        private Ak8975Registers _registers;
        public Vector3 _magnetometer;

        public Ak8975(I2C i2c)
        {
            dev = new SMBusDevice(0x0c, i2c);
            _registers = new Ak8975Registers(dev);
        }

        public Vector3 Magnetometer
        {
            get
            {
                if (AutoUpdateWhenPropertyRead)
                    Task.Run(Update).Wait();

                return _magnetometer;
            }
        }
        public bool AutoUpdateWhenPropertyRead { get; set; }
        public int AwaitPollingInterval { get; set; }

        public async Task Update()
        {
            _registers.Control.Mode = 1;
            Task.Run(_registers.Control.Write).Wait();
            while(true)
            {
                await _registers.Status1.Read().ConfigureAwait(false);
                if (_registers.Status1.Drdy == 1)
                    break;
            }

            await _registers.ReadRange(_registers.Hx, _registers.Hz).ConfigureAwait(false);
            _magnetometer.X = _registers.Hx.Value;
            _magnetometer.Y = _registers.Hy.Value;
            _magnetometer.Z = _registers.Hz.Value;
        }
    }
}
