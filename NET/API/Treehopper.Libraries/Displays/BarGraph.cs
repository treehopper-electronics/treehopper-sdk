using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Treehopper.Libraries.IO;
using Treehopper.Utilities;

namespace Treehopper.Libraries.Displays
{
    /// <summary>
    ///     Represents a collection of LEDs visually arranged in a line or circle that represent a real-valued number from 0.0
    ///     to 1.0.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This class can be used to construct a wide variety of dynamic displays, including LED rings around encoders,
    ///         linear displays, and animation effects.
    ///     </para>
    /// </remarks>
    public class BarGraph : LedDisplay
    {
        private bool fill = true;

        private double val;

        /// <summary>
        ///     Construct a bar graph from a list of LEDs.
        /// </summary>
        /// <param name="Leds">The list of LEDs to use in the bar graph</param>
        public BarGraph(IList<Led> Leds)
        {
            this.Leds = new LedCollection(Leds);
        }

        /// <summary>
        ///     THe value -- from 0.0 to 1.0 -- of the bar graph
        /// </summary>
        public double Value
        {
            get { return val; }
            set
            {
                if (val.CloseTo(value)) return;
                if (val < 0 || val > 1)
                    throw new ArgumentOutOfRangeException("Value must be between 0 and 1");

                val = value;
                if (AutoFlush)
                    FlushAsync().Wait();
            }
        }

        /// <summary>
        ///     Gets or sets whether the display should fill. When false, the display's current value is only indicated by a single
        ///     LED.
        /// </summary>
        public bool Fill
        {
            get { return fill; }
            set
            {
                if (fill == value) return;
                fill = value;

                if (AutoFlush)
                    FlushAsync().Wait();
            }
        }

        /// <summary>
        ///     The collection of LEDs belonging to this bar graph
        /// </summary>
        public LedCollection Leds { get; }

        /// <summary>
        ///     Write the LEDs without flushing the drivers
        /// </summary>
        public void WriteLeds()
        {
            var number = (int) Math.Round(val * Leds.Count);
            for (var i = 0; i < Leds.Count; i++)
                if (fill & (i <= number - 1) || i == number - 1)
                    Leds[i].State = true;
                else
                    Leds[i].State = false;
        }

        /// <summary>
        ///     Flush data to the drivers
        /// </summary>
        /// <param name="force">Whether to force all data to flush, even if it appears to be unchanged</param>
        /// <returns>An awaitable task</returns>
        public Task FlushAsync(bool force = false)
        {
            WriteLeds();
            return Leds.Flush(force);
        }

        /// <summary>
        ///     The parent object. Unused, returns null.
        /// </summary>
        public IFlushable Parent => null;

        /// <summary>
        ///     Whether to automatically flush this LED widget whenever a value is written to it
        /// </summary>
        public bool AutoFlush { get; set; } = true;
    }
}