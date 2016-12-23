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
        /// <summary>
        /// Construct an LedDriver
        /// </summary>
        /// <param name="numLeds">The number of LEDs to construct</param>
        /// <param name="HasGlobalBrightnessControl">Whether the controller can globally adjust the LED brightness</param>
        /// <param name="HasIndividualBrightnessControl">Whether the controller has individual LED brightness control</param>
        public LedDriver(int numLeds, bool HasGlobalBrightnessControl, bool HasIndividualBrightnessControl)
        {
            for (int i = 0; i < numLeds; i++)
            {
                var led = new Led(this, i, HasIndividualBrightnessControl);
                Leds.Add(led);
            }
        }

        /// <summary>
        /// Gets or sets whether the display should auto-flush whenever an LED state is changed
        /// </summary>
        public bool AutoFlush { get; set; } = true;

        /// <summary>
        /// Gets or sets whether this controller supports global brightness control.
        /// </summary>
        public bool HasGlobalBrightnessControl { get; protected set; }

        /// <summary>
        /// Gets or sets whether this controller's LEDs have individual brightness control (through PWM or otherwise).
        /// </summary>
        public bool HasIndividualBrightnessControl { get; private set; }

        private double brightness = 0.0;

        /// <summary>
        /// The brightness, from 0.0-1.0, of the LED.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This property is meaningless when <see cref="HasGlobalBrightnessControl"/>  is false
        /// </para>
        /// </remarks>
        public double Brightness
        {
            get { return brightness; }

            set
            {
                if (Math.Abs(brightness - value) < 0.0005) return;
                if (value < 0 || value > 1)
                    throw new ArgumentOutOfRangeException("Valid brightness is from 0 to 1");
                brightness = value;

                setBrightness(brightness);
            }
        }

        internal abstract void setBrightness(double brightness);

        /// <summary>
        /// The collection of LEDs that belong to this driver.
        /// </summary>
        public IList<Led> Leds { get; set; } = new Collection<Led>();


        /// <summary>
        /// Called by the LED when the state changes
        /// </summary>
        /// <param name="led">The LED whose state changed</param>
        internal abstract void LedStateChanged(Led led);
        void ILedDriver.LedStateChanged(Led led) { LedStateChanged(led); }


        /// <summary>
        /// Called by the LED when the brightness changed
        /// </summary>
        /// <param name="led">The LED whose brightness changed</param>
        internal abstract void LedBrightnessChanged(Led led);
        void ILedDriver.LedBrightnessChanged(Led led) { LedBrightnessChanged(led); }

        /// <summary>
        /// Clear the display
        /// </summary>
        /// <returns>An awaitable task that completes when the display is cleared</returns>
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
