namespace Treehopper.Libraries.IO.PortExpander
{
    /// This class supports all standard 8-bit serial-input, parallel-output shift registers similar to the 74HC595.
    /// 
    /// Once constructed, this object provides a #Pins list that can be used just as any other digital output pins. 
    /// By default, changing the value of one of these pins will immediately flush to the shift register:
    /// \code
    /// var shift1 = new Hc595(board.Spi, board.Pins[7]);
    /// shift1.Pins[3].DigitalValue = true; // immediately sets the output high
    /// shift1.Pins[2].DigitalValue = true; // performs a second SPI transaction to immediately set the output high
    /// \endcode
    /// If you plan on updating many pins at once, set the #AutoFlush property to false and then call FlushAsync() whenever you wish your pin changes to be written to the shift register:
    /// \code
    /// var shift1 = new Hc595(board.Spi, board.Pins[7]);
    /// shift1.AutoFlush = false; // disable automatically flushing changes to the shift register
    /// shift1.Pins[3].DigitalValue = true; // nothing happens (yet)
    /// shift1.Pins[2].DigitalValue = true; // nothing happens (yet)
    /// await shift1.FlushAsync(); // both pins are updated in a single transaction
    /// \endcode
    /// Note that you can chain multiple shift registers together by using the second constructor listed.
    /// \code
    /// var shift1 = new Hc595(board.Spi, board.Pins[7]); // construct a shift register attached to our SPI interface
    /// var shift2 = new Hc595(shift1); // we have a second shift register attached to the output of the first.
    /// \endcode
    [Supports("Generic", "74HC595")]
    public class Hc595 : ShiftOut
    {
        /// <summary>
        ///     Construct a new 74HC595-type shift register that is directly connected to a Treehopper's SPI port
        /// </summary>
        public Hc595(Spi spiModule, SpiChipSelectPin latchPin, double speedMhz = 6) : base(spiModule, latchPin, 8,
            SpiMode.Mode00, ChipSelectMode.PulseHighAtEnd, speedMhz)
        {
        }

        /// <summary>
        ///     Construct a 595-type shift register connected to the output of another 595-type shift register.
        /// </summary>
        /// <param name="UpstreamDevice"></param>
        public Hc595(ShiftOut UpstreamDevice) : base(UpstreamDevice, 8)
        {
        }
    }
}