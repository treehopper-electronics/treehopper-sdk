using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Treehopper.Utilities;

namespace Treehopper.Libraries.Displays
{
    /// <summary>
    ///     A mono (on/off) graphic display comprised of LEDs
    /// </summary>
    public class LedGraphicDisplay : MonoGraphicDisplay, LedDisplay
    {
        /// <summary>
        ///     Construct a new LED graphic display
        /// </summary>
        /// <param name="leds">The LEDs to use with this display</param>
        /// <param name="width">The number of LEDs wide this display is</param>
        /// <param name="height">THe number of LEDs tall this display is</param>
        public LedGraphicDisplay(IList<Led> leds, int width, int height) : base(width, height)
        {
            Leds = new LedCollection(leds);
        }

        /// <summary>
        ///     The collection of LEDs to use, ordered by column and in ascending row
        /// </summary>
        public LedCollection Leds { get; }

        public void WriteLeds()
        {
            for (var i = 0; i < RawBuffer.Length; i++)
            {
                var data = RawBuffer[i];
                for (var j = 0; j < 8; j++)
                {
                    var idx = 8 * i + j;
                    Leds[idx].State = ((data >> j) & 0x01) == 0x01 ? true : false;
                }
            }
        }

        /// <summary>
        ///     Write out all the pixels' values to the corresponding LEDs
        /// </summary>
        /// <returns>An awaitable task</returns>
        protected override Task flush()
        {
            WriteLeds();
            return Leds.Flush();
        }

        /// <summary>
        ///     Set the global brightness of the display
        /// </summary>
        /// <param name="brightness">The brightness, from 0-1, to set</param>
        /// <remarks>
        ///     <para>
        ///         Note that this function will write out this brightness setting to all underlying controllers; if the controller
        ///         doesn't support brightness control, this setting is ignored.
        ///     </para>
        /// </remarks>
        protected override void setBrightness(double brightness)
        {
            Leds.Select(x => x.Driver).Distinct().ForEach(drv => drv.Brightness = brightness);
        }
    }
}