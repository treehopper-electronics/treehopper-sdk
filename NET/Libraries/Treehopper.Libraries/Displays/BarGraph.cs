using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Displays
{
    /// <summary>
    /// Represents a collection of LEDs visually arranged in a line or circle that represent a real-valued number from 0.0 to 1.0.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class can be used to construct a wide variety of dynamic displays, including LED rings around encoders, linear displays, and animation effects.
    /// </para>
    /// </remarks>
    public class BarGraph
    {
        private IList<Led> leds;
        private Collection<ILedDriver> drivers = new Collection<ILedDriver>();

        /// <summary>
        /// Construct a bar graph from a list of LEDs.
        /// </summary>
        /// <param name="Leds">The list of LEDs to use in the bar graph</param>
        public BarGraph(IList<Led> Leds)
        {
            this.leds = Leds;
            foreach (var led in Leds)
            {
                if (!drivers.Contains(led.Driver))
                    drivers.Add(led.Driver);
            }

            foreach(var driver in drivers)
            {
                driver.AutoFlush = false; // disable autoflush to speed things up
            }
        }

        private double val = 0;

        /// <summary>
        /// THe value -- from 0.0 to 1.0 -- of the bar graph
        /// </summary>
        public double Value
        {
            get { return val; }
            set
            {
                if (val == value) return;
                if (val < 0 || val > 1)
                    throw new ArgumentOutOfRangeException("Value must be between 0 and 1");

                val = value;

                Flush();
            }
        }

        private void Flush()
        {
            int number = (int)Math.Round(val * leds.Count);
            for(int i=0;i<leds.Count;i++)
            {
                if ((fill & i < number) || i == (number - 1))
                    leds[i].State = true;
                else
                    leds[i].State = false;
            }
            foreach(var driver in drivers)
            {
                if (!driver.AutoFlush)
                    driver.Flush().Wait();
            }
        }

        private bool fill = true;

        /// <summary>
        /// Gets or sets whether the display should fill. When false, the display's current value is only indicated by a single LED.
        /// </summary>
        public bool Fill
        {
            get { return fill; }
            set
            {
                if (fill == value) return;
                fill = value;

                Flush();
            }
        }
    }
}
