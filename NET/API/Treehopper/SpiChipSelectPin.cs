namespace Treehopper
{
    public interface SpiChipSelectPin : DigitalOutPin
    {
        int PinNumber { get; }

        Spi SpiModule { get; }
    }
}
