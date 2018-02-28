using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Treehopper.Libraries.Sensors.Magnetic;

namespace Treehopper.Libraries.Sensors.Inertial
{
    public class Lsm303dlhc : IPollable
    {
        public Lsm303dlhcAccel Accel { get; set; }
        public Lsm303dlhcMag Mag { get; set; }

        private bool autoUpdateWhenPropertyRead = true;
        public bool AutoUpdateWhenPropertyRead
        {
            get
            {
                return autoUpdateWhenPropertyRead;
            }
            set
            {
                autoUpdateWhenPropertyRead = value;
                Accel.AutoUpdateWhenPropertyRead = value;
                Mag.AutoUpdateWhenPropertyRead = value;
            }
        }
        public int AwaitPollingInterval { get; set; }

        public Lsm303dlhc(I2C i2c, int rate=100)
        {
            Accel = new Lsm303dlhcAccel(i2c, rate);
            Mag = new Lsm303dlhcMag(i2c, rate);
            Accel.AutoUpdateWhenPropertyRead = true;
            Mag.AutoUpdateWhenPropertyRead = true;
        }

        public async Task UpdateAsync()
        {
            await Accel.UpdateAsync().ConfigureAwait(false);
            await Mag.UpdateAsync().ConfigureAwait(false);
        }
    }
}
