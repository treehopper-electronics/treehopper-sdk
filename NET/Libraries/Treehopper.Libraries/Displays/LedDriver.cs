using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Displays
{
    /// <summary>
    /// Base class that all LED drivers inherit from.
    /// </summary>
    public abstract class LedDriver : ILedDriver
    {
        public LedDriver(int numLeds, bool HasGlobalBrightnessControl, bool HasIndividualBrightnessControl)
        {
            for (int i = 0; i < numLeds; i++)
            {
                var led = new Led(this, i, HasIndividualBrightnessControl);
                Leds.Add(led);
            }
        }
        public bool AutoFlush { get; set; } = true;

        /// <summary>
        /// Gets or sets whether this controller supports global brightness control.
        /// </summary>
        public bool HasGlobalBrightnessControl { get; protected set; }

        /// <summary>
        /// Gets or sets whether this controller's LEDs have individual brightness control (through PWM or otherwise).
        /// </summary>
        public bool HasIndividualBrightnessControl { get; private set; }

        /// <summary>
        /// The brightness, from 0.0-1.0, of the LED.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This property is meaningless when <see cref="HasGlobalBrightnessControl"/>  is false
        /// </para>
        /// </remarks>
        public virtual double Brightness { get; set; }

        /// <summary>
        /// The collection of LEDs that belong to this driver.
        /// </summary>
        public IList<Led> Leds { get; set; } = new Collection<Led>();

        public abstract void LedStateChanged(Led led);
        public abstract void LedBrightnessChanged(Led led);
        public async Task Clear()
        {
            bool autoflushSave = AutoFlush;
            AutoFlush = false;
            foreach(var led in Leds)
            {
                led.State = false;
            }
            await Flush(true);
            AutoFlush = autoflushSave;
        }

        /// <summary>
        /// Write out the current values of the LEDs immediately.
        /// </summary>
        /// <param name="force">Whether to write out all values, even if they appear unchanged.</param>
        /// <returns>An awaitable <see cref="Task"/></returns>
        public abstract Task Flush(bool force = false);


    }
}
