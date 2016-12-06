using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Displays
{
    /// <summary>
    /// Represents a single LED that may or may not have brightness control
    /// </summary>
    public class Led
    {
        /// <summary>
        /// The driver this LED is attached to
        /// </summary>
        internal ILedDriver Driver { get; private set; }

        /// <summary>
        /// Construct a new LED
        /// </summary>
        /// <param name="driver">The driver this LED is attached to</param>
        /// <param name="channel">The channel to use</param>
        /// <param name="hasBrightnessControl">Whether the LED has brightness control</param>
        internal Led(ILedDriver driver, int channel = 0, bool hasBrightnessControl = false)
        {
            Driver = driver;
            HasBrightnessControl = hasBrightnessControl;
            Channel = channel;
        }

        /// <summary>
        /// Gets or sets the channel this LED belongs to
        /// </summary>
        public int Channel { get; private set; }

        private double brightness = 1.0;

        /// <summary>
        /// Gets or sets the brightness (0.0 - 1.0) of the LED (if it has brightness control)
        /// </summary>
        public double Brightness
        {
            get { return brightness; }
            set
            {
                if (brightness == value) return;
                if (!HasBrightnessControl) return;
                brightness = value;
                Driver.LedBrightnessChanged(this);
            }
        }

        /// <summary>
        /// Gets whether the LED has brightness control
        /// </summary>
        public bool HasBrightnessControl { get; private set; }


        private bool state = false;

        /// <summary>
        /// Gets or sets the state of the LED
        /// </summary>
        public bool State
        {
            get { return state; }
            set
            {
                if (state == value) return;
                state = value;
                Driver.LedStateChanged(this);
            }
        }
    }
}
