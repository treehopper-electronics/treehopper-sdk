using System.Collections.Generic;
using System.Threading.Tasks;
using Treehopper.Libraries.Interface;

namespace Treehopper.Libraries.Displays
{
    /// <summary>
    ///     Represents an LedDriver
    /// </summary>
    public interface ILedDriver : IFlushable
    {
        /// <summary>
        ///     Gets or sets whether this controller supports global brightness control.
        /// </summary>
        bool HasGlobalBrightnessControl { get; }

        /// <summary>
        ///     Gets or sets whether this controller's LEDs have individual brightness control (through PWM or otherwise).
        /// </summary>
        bool HasIndividualBrightnessControl { get; }

        /// <summary>
        ///     The brightness, from 0.0-1.0, of the LED.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         This property is meaningless when <see cref="HasGlobalBrightnessControl" />  is false
        ///     </para>
        /// </remarks>
        double Brightness { get; set; }

        /// <summary>
        ///     The collection of LEDs that belong to this driver.
        /// </summary>
        IList<Led> Leds { get; }

        /// <summary>
        ///     This is called by the LEDs to force the driver to update the LED state
        /// </summary>
        /// <param name="led">The LED to update</param>
        void LedStateChanged(Led led);

        /// <summary>
        ///     This is called by the LEDs to force the driver to update the LED brightness
        /// </summary>
        /// <param name="led">The LED to update</param>
        void LedBrightnessChanged(Led led);

        /// <summary>
        ///     Clear (turn off) all LEDs
        /// </summary>
        /// <returns></returns>
        Task Clear();
    }
}