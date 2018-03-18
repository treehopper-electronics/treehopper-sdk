namespace Treehopper.Libraries.IO.DigitalPot
{
    [Supports("Microchip", "MCP4151")]
    [Supports("Microchip", "MCP4152")]
    [Supports("Microchip", "MCP4161")]
    [Supports("Microchip", "MCP4162")]
    public class Mcp415x : Mcp413x
    {
        public Mcp415x(Spi spi, SpiChipSelectPin cs) : base(spi, cs)
        {
            scale = 256;
        }
    }
}