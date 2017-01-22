using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper.Utilities;

namespace Treehopper.Libraries.Displays
{
    /// <summary>
    /// Represents a single LED that may or may not have brightness control
    /// </summary>
    /// <remarks>
    /// <para>The default <see cref="State"/> of a newly-constructed LED is "false" (off) with a <see cref="Brightness"/> of 1.0 (even if this LED does not support brightness control). This allows LEDs to be used with many non-dimmable display classes (such as <see cref="SevenSegmentDigit"/>, or <see cref="BarGraph"/>) that only control the LED state. Note that for an LED to be on, its state must be "true" and its brightness must be non-zero. </para>
    /// <para>Do not confuse the <see cref="State"/>'s value with the electrical value on the pin; most LED drivers are open-drain, thus a "true" state — i.e., when the LED is on — actually corresponds to the pin being driven to 0 (logic false); this is handled by the individual LED drivers to remove ambiguity. If your driver supports driving LEDs in either common-anode (open-drain) or common-cathode configurations, ensure the driver itself is configured to match your circuit.</para>
    /// <para>You can also attach LEDs to pins that don't belong to LED drivers; see <see cref="GpioLedDriver{TDigitalOutPin}"/> and <see cref="PwmLedDriver{TPwm}"/>, which perform the necessary conversions.</para>
    /// </remarks>
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
        /// <remarks>
        /// <para>The brightness is perceptual on a linear scale; i.e., a brightness of 0.5 will be perceived as half as bright as a brightness of 1.0.</para>
        /// <para>Note that the LED will only illuminate if the <see cref="State"/> is true, regardless of the value of this property.</para>
        /// </remarks>
        public double Brightness
        {
            get { return brightness; }
            set
            {
                if (brightness.CloseTo(value)) return;
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
        /// <remarks>
        /// <para>Note that if the LED has brightness control, it will only illuminate if the <see cref="Brightness"/> of the LED is also non-zero.</para>
        /// </remarks>
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

        public override string ToString()
        {
            return string.Format("{0} ({1:0.00})", state, brightness);
        }
    }
}
