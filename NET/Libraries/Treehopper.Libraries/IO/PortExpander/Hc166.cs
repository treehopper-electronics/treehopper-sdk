using System.Collections.ObjectModel;

namespace Treehopper.Libraries.IO.PortExpander
{
    /// <summary>
    ///     74HC166 parallel-in, serial-out shift register
    /// </summary>
    [Supports("Generic", "74HC166")]
    public class Hc166
    {
        /// <summary>
        ///     Construct a new instance of the Sn74hc166
        /// </summary>
        /// <param name="spiModule">SPI module to use</param>
        /// <param name="loadPin">latch pin</param>
        public Hc166(Spi spiModule, DigitalOut loadPin)
        {
            loadPin.MakeDigitalPushPullOut();
            spiModule.Enabled = true;
        }

        /// <summary>
        ///     Collection of pins
        /// </summary>
        public Collection<DigitalIn> Pins { get; set; } = new Collection<DigitalIn>();
    }
}