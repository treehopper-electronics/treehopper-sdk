using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Interface.DigitalPot
{
    public class Mcp413x
    {
        protected SpiDevice dev;

        protected int scale;

        protected enum Registers
        {
            Wiper0 = 0x00,
            Wiper1 = 0x01,
            Tcon = 0x04
        }

        public Mcp413x(Spi spi, SpiChipSelectPin cs)
        {
            this.dev = new SpiDevice(spi, cs);
            scale = 128;
        }

        private double wiper1;

        public double Wiper1
        {
            get { return wiper1; }
            set
            {
                wiper1 = value;
                int data = (int) Math.Round(wiper1 * scale);
                dev.SendReceive(new byte[] {(byte) (0x00 | (data >> 8)), (byte) (data & 0xff)}, SpiBurstMode.BurstTx);
            }
        }
    }
}
