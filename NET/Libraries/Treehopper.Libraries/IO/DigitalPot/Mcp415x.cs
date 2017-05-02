namespace Treehopper.Libraries.Interface.DigitalPot
{
    public class Mcp415x : Mcp413x
    {
        public Mcp415x(Spi spi, SpiChipSelectPin cs) : base(spi, cs)
        {
            scale = 256;
        }
    }
}