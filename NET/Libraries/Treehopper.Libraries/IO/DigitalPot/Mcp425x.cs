namespace Treehopper.Libraries.IO.DigitalPot
{
    [Supports("Microchip", "MCP4251")]
    [Supports("Microchip", "MCP4252")]
    [Supports("Microchip", "MCP4261")]
    [Supports("Microchip", "MCP4262")]
    public class Mcp425x : Mcp423x
    {
        public Mcp425x(Spi spi, SpiChipSelectPin cs) : base(spi, cs)
        {
            scale = 256;
        }
    }
}