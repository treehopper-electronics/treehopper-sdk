namespace Treehopper.Libraries.Interface.DigitalPot
{
    public class Mcp425x : Mcp423x
    {
        public Mcp425x(Spi spi, SpiChipSelectPin cs) : base(spi, cs)
        {
            scale = 256;
        }
    }
}