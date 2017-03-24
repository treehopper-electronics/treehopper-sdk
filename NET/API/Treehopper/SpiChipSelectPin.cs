namespace Treehopper
{
    public interface SpiChipSelectPin : DigitalOut
    {
        int PinNumber { get; }

        Spi SpiModule { get; }
    }
}
