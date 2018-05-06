namespace Treehopper.Libraries.IO.DigitalPot
{
    [Supports("Microchip", "MCP4661")]
    public class Mcp466x : Mcp463x
    {
        public Mcp466x(I2C i2c, bool a0 = false, bool a1 = false, bool a2 = false, int rateKhz = 100) : base(i2c, a0, a1, a2, rateKhz)
        {
            scale = 256;
        }
    }
}