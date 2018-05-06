using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper.Utilities;

namespace Treehopper.Libraries.IO.DigitalPot
{
    [Supports("Microchip", "MCP4531")]
    [Supports("Microchip", "MCP4532")]
    public class Mcp453x
    {
        protected SMBusDevice dev;

        protected int scale;

        protected enum Registers
        {
            Wiper0 = 0x00,
            Wiper1 = 0x01,
            NvWiper0 = 0x02,
            NvWiper1 = 0x03,
            Tcon = 0x04,
            Status = 0x05
        }

        public Mcp453x(I2C i2c, bool a0=false, bool a1=false, bool a2=false, int rateKhz = 100)
        {
            this.dev = new SMBusDevice((byte)(0x28 | (a0 ? 1 : 0) | ((a1 ? 1 : 0) << 1) | ((a2 ? 1 : 0) << 2)), i2c, rateKhz);
            scale = 128;
        }

        private double wiper0 = -1;

        public double Wiper0
        {
            get { return wiper0; }
            set
            {
                if (wiper0.CloseTo(value))
                    return;

                wiper0 = value;
                int data = (int)Math.Round(wiper0 * scale);

                if(scale == 128)
                {
                    Task.Run(() => dev.WriteByteDataAsync(((byte)Registers.Wiper0) << 4, (byte)data)).Wait();
                }
                else
                {
                    Task.Run(() => dev.WriteByteDataAsync((byte)(((byte)Registers.Wiper0) << 4 | (data >> 8)), (byte)data)).Wait();
                }
            }
        }
    }
}
