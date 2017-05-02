using System;

namespace Treehopper.Libraries.Interface.DigitalPot
{
    public class Mcp423x : Mcp413x
    {
        public Mcp423x(Spi spi, SpiChipSelectPin cs) : base(spi, cs)
        {

        }

        private double wiper2;

        public double Wiper2
        {
            get { return wiper2; }
            set
            {
                wiper2 = value;
                var data = (int) Math.Round(wiper2 * scale);
                dev.SendReceive(new byte[] {(byte) (0x10 | (data >> 8)), (byte) (data & 0xff)});
            }
        }
    }
}