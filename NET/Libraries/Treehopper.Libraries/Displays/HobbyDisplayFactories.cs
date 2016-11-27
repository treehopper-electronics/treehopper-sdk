using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper.Libraries.Interface;
using Treehopper.Libraries.Interface.PcfSeries;

namespace Treehopper.Libraries.Displays
{
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
        /// This factory method takes a Pcf8574, creates a <see cref="FlushableParallelInterface{PortExpanderPin}"/>
        /// parallel interface for it, assigns the correct pinout to this interface, and constructs a <see cref="Hd44780"/>
        /// display using this interface, using the slower 4-bit mode for operation.
        /// </para>
        /// <para>
        /// If you wish to produce custom hardware compatiable with this factory method, the correct pinout is:
        /// </para>
        /// <list>
        /// <item>Register Select (RS): Pin 0</item>
        /// <item>Read/Write (R/W): Pin 1</item>
        /// <item>Enable (E): Pin 2</item>
        /// <item>Backlight Enable: Pin 3</item>
        /// <item>Data 4: Pin 4</item>
        /// <item>Data 5: Pin 5</item>
        /// <item>Data 6: Pin 6</item>
        /// <item>Data 7: Pin 7</item>
        /// </list>
        /// <para>
        /// Note that when the Hd44780 is operated in 4-bit mode, the high-nibble (D4-D7) is used, not the low-nibble.
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
    }
}
