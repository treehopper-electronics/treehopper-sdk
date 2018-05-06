using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Treehopper.Utilities;

namespace Treehopper.Libraries.IO.DigitalPot
{
    public class Mcp463x : Mcp453x
    {
        public Mcp463x(I2C i2c, bool a0 = false, bool a1 = false, bool a2 = false, int rateKhz = 100) : base(i2c, a0, a1, a2, rateKhz)
        {
        }

        private double wiper1 = -1;

        public double Wiper1
        {
            get { return wiper1; }
            set
            {
                if (wiper1.CloseTo(value))
                    return;

                wiper1 = value;
                int data = (int)Math.Round(wiper1 * scale);

                if (scale == 128)
                {
                    Task.Run(() => dev.WriteByteDataAsync(((byte)Registers.Wiper1) << 4, (byte)data)).Wait();
                }
                else
                {
                    Task.Run(() => dev.WriteByteDataAsync((byte)(((byte)Registers.Wiper1) << 4 | (data >> 8)), (byte)data)).Wait();
                }
            }
        }
    }
}
