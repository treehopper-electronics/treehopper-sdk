using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Sensors.Magnetic
{
    [Supports("AKM", "AK8975")]
    [Supports("AKM", "AK8975C")]
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
                    Task.Run(UpdateAsync).Wait();

                return _magnetometer;
            }
        }
        public bool AutoUpdateWhenPropertyRead { get; set; }
        public int AwaitPollingInterval { get; set; }

        public async Task UpdateAsync()
        {
            _registers.control.mode = 1;
            Task.Run(_registers.control.write).Wait();
            while(true)
            {
                await _registers.status1.read().ConfigureAwait(false);
                if (_registers.status1.drdy == 1)
                    break;
            }

            await _registers.readRange(_registers.hx, _registers.hz).ConfigureAwait(false);
            _magnetometer.X = _registers.hx.value;
            _magnetometer.Y = _registers.hy.value;
            _magnetometer.Z = _registers.hz.value;
        }
    }
}
