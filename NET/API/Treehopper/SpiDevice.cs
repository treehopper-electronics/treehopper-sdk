using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper
{
    public class SpiDevice
    {
        private Pin cs;
        private Spi spi;
        private double speed;
        public SpiDevice(Spi SpiModule, Pin ChipSelect, double SpeedMHz = 1)
        {
            this.cs = ChipSelect;
            this.spi = SpiModule;
            this.speed = SpeedMHz;
            spi.Enabled = true;
        }

        protected async Task<byte[]> SendReceive(byte[] dataToSend, BurstMode burst = BurstMode.NoBurst)
        {
            spi.ChipSelect = cs;
            spi.Frequency = speed;
            return await spi.SendReceive(dataToSend, burst);
        }
    }
}
