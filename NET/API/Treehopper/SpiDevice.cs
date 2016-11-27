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
        private SPIMode mode;
        private double speed;
        private ChipSelectMode csMode;

        public SpiDevice(Spi SpiModule, Pin ChipSelect, double SpeedMHz = 1, SPIMode mode = SPIMode.Mode00, ChipSelectMode csMode = ChipSelectMode.SpiActiveLow)
        {
            this.csMode = csMode;
            this.cs = ChipSelect;
            this.spi = SpiModule;
            this.speed = SpeedMHz;
            this.mode = mode;

            spi.Enabled = true;
        }

        public async Task<byte[]> SendReceive(byte[] dataToSend, BurstMode burst = BurstMode.NoBurst)
        {
            spi.ChipSelectMode = csMode;
            spi.ChipSelect = cs;
            spi.Frequency = speed;
            return await spi.SendReceive(dataToSend, burst);
        }
    }
}
