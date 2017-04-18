namespace Treehopper.Libraries.Displays
{
    using System.Collections.Generic;
    using System.Linq;
    using Interface;
    using Interface.PortExpander;

    /// <summary>
    /// Contains static methods useful for constructing commonly-available display modules.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Treehopper's display libraries are highly configurable, which allows mixing and matching
    /// display arrays, controllers, and interface expanders. However, this configurability can require
    /// quite a bit of initialization code to be written. These factory methods
    /// are designed to make it easy to construct commonly-sourced display modules.
    /// </para>
    /// </remarks>
    public class HobbyDisplayFactories
    {
        /// <summary>
        /// Construct a standard Hd44780-compatible character display driven from the commonly-available Pcf8574 backpack.
        /// </summary>
        /// <param name="ioExpander">A Pcf8574 instance to use.</param>
        /// <param name="columns">The number of columns in the display (defaults to 16)</param>
        /// <param name="rows">The number of rows in the display (defaults to 2)</param>
        /// <returns>An Hd44780 object supporting standard CharacterDisplay methods.</returns>
        /// <remarks>
        /// <para>
        /// This factory method takes an already-created <see cref="Pcf8574"/>, creates a <see cref="FlushableParallelInterface{PortExpanderPin}"/>
        /// parallel interface for it, assigns the correct pinout to this interface, and constructs a <see cref="Hd44780"/>
        /// display using this interface, using the slower 4-bit mode for operation.
        /// </para>
        /// <para>
        /// If you wish to produce custom hardware compatiable with this factory method, the correct pinout is:
        /// <list type="number">
        /// <item><term>Register Select (RS)</term><description>Pin 0</description></item>
        /// <item><term>Read/Write (RW)</term><description>Pin 1</description></item>
        /// <item><term>Enable (E)</term><description>Pin 2</description></item>
        /// <item><term>Backlight Enable</term><description>Pin 3</description></item>
        /// <item><term>Data 4</term><description>Pin 4</description></item>
        /// <item><term>Data 5</term><description>Pin 5</description></item>
        /// <item><term>Data 6</term><description>Pin 6</description></item>
        /// <item><term>Data 7</term><description>Pin 7</description></item>
        /// </list>
        /// 
        /// Note that when the Hd44780 is operated in 4-bit mode, only the high-nibble (D4-D7) is used, not the low-nibble.
        /// </para>
        /// </remarks>
        public static Hd44780 GetCharacterDisplayFromPcf8574(Pcf8574 ioExpander, int columns = 16, int rows = 2)
        {
            var parallelInterface = new FlushableParallelInterface<PortExpanderPin>(ioExpander);

            parallelInterface.RegisterSelectPin = ioExpander.Pins[0];
            parallelInterface.ReadWritePin = ioExpander.Pins[1];
            parallelInterface.EnablePin = ioExpander.Pins[2];

            parallelInterface.DataBus.Add(ioExpander.Pins[4]);
            parallelInterface.DataBus.Add(ioExpander.Pins[5]);
            parallelInterface.DataBus.Add(ioExpander.Pins[6]);
            parallelInterface.DataBus.Add(ioExpander.Pins[7]);

            parallelInterface.Enabled = true;

            return new Hd44780(parallelInterface, columns, rows, ioExpander.Pins[3]);
        }

        /// <summary>
        /// Get a GraphicDisplay built from one or more 8x8 MAX7219-powered LED displays commonly available from hobbyist vendors.
        /// </summary>
        /// <param name="port">The SPI port these displays are attached to</param>
        /// <param name="latch">The latch pin to use with these displays</param>
        /// <param name="numDevices">The number of 8x8 units that are daisy-chained together</param>
        /// <returns></returns>
        public static LedGraphicDisplay GetMax7219GraphicLedDisplay(Spi port, Treehopper.Pin latch, int numDevices)
        {
            IEnumerable<Led> finalList = new List<Led>();
            for (int i = 0; i < numDevices; i++)
            {
                Max7219 driver;

                driver = new Max7219(port, latch, i);

                var tempList = new List<Led>();

                // We have to re-order the LEDs from the Max7219
                {
                    for (int k = 62; k >= 56; k--)
                    {
                        for (int m = k; m >= 0; m -= 8)
                        {
                            tempList.Add(driver.Leds[m]);
                        }
                    }

                    for (int k = 63; k >= 0; k -= 8)
                    {
                        tempList.Add(driver.Leds[k]);
                    }
                }

                finalList = finalList.Concat(tempList);
            }

            var display = new LedGraphicDisplay(finalList.ToList(), 8 * numDevices, 8);

            return display;
        }
    }
}
