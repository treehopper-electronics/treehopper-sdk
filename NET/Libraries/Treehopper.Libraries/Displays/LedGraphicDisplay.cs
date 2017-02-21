using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper.Libraries.Interface;
using Treehopper.Utilities;

namespace Treehopper.Libraries.Displays
{
    /// <summary>
    /// A mono (on/off) graphic display comprised of LEDs
    /// </summary>
    public class LedGraphicDisplay : MonoGraphicDisplay, LedDisplay
    {
        /// <summary>
        /// The collection of LEDs to use, ordered by column and in ascending row
        /// </summary>
        public LedCollection Leds { get; private set; }

        /// <summary>
        /// The parent object
        /// </summary>
        public IFlushable Parent { get; set; }

        /// <summary>
        /// Whether this display should automatically flush whenever a pixel is updated
        /// </summary>
        public bool AutoFlush { get; set; } = true;

        /// <summary>
        /// Construct a new LED graphic display
        /// </summary>
        /// <param name="leds">The LEDs to use with this display</param>
        /// <param name="width">The number of LEDs wide this display is</param>
        /// <param name="height">THe number of LEDs tall this display is</param>
        public LedGraphicDisplay(IList<Led> leds, int width, int height) : base(width, height)
        {
            Leds = new LedCollection(leds);
        }

        /// <summary>
        /// Write out all the pixels' values to the corresponding LEDs
        /// </summary>
        /// <returns>An awaitable task</returns>
        protected override async Task flush()
        {
            if (AutoFlush)
                await Flush().ConfigureAwait(false);
        }

        /// <summary>
        /// Set the global brightness of the display
        /// </summary>
        /// <param name="brightness">The brightness, from 0-1, to set</param>
        /// <remarks>
        /// <para>
        /// Note that this function will write out this brightness setting to all underlying controllers; if the controller doesn't support brightness control, this setting is ignored.
        /// </para>
        /// </remarks>
        protected override void setBrightness(double brightness)
        {
            Leds.Select(x => x.Driver).Distinct().ForEach(drv => drv.Brightness = brightness);
        }

        /// <summary>
        /// Set the LED states based on the graphic buffer. This function does not actually flush the LED states to the driver(s)
        /// </summary>
        public void WriteLeds()
        {
            for (int i = 0; i < RawBuffer.Length; i++)
            {
                byte data = RawBuffer[i];
                for (int j = 0; j < 8; j++)
                {
                    int idx = (8 * i) + j;
                    Leds[idx].State = (((data >> j) & 0x01) == 0x01) ? true : false;
                }
            }
        }

        /// <summary>
        /// Flush the LED states to the driver(s)
        /// </summary>
        /// <param name="force">Whether to force an update</param>
        /// <returns>An awaitable task</returns>
        public Task Flush(bool force = false)
        {
            WriteLeds();
            return Leds.Flush(force);
        }
    }
}
