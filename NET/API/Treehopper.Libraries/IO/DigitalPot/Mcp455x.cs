using System;
using System.Collections.Generic;
using System.Text;

namespace Treehopper.Libraries.IO.DigitalPot
{
    public class Mcp455x : Mcp453x
    {
        public Mcp455x(I2C i2c, bool a0 = false, bool a1 = false, bool a2 = false, int rateKhz = 100) : base(i2c, a0, a1, a2, rateKhz)
        {
            scale = 256;
        }
    }
}
