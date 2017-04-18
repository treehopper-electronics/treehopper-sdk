using System.Collections.Generic;
using System.Threading.Tasks;
using Treehopper.Libraries.Interface;

namespace Treehopper.Libraries.Displays
{
    /// <summary>
    ///     An LedDriver implemented using plain digital output pins
    /// </summary>
    /// <typeparam name="TPwm">The <see cref="Pwm" />-implementing type that should be used.</typeparam>
    public class PwmLedDriver<TPwm> : LedDriver where TPwm : Pwm
    {
        private readonly IFlushable controller;
        private readonly IList<TPwm> Pins;
        private readonly bool useActiveLowOutput;

        /// <summary>
        ///     Construct a new LedDriver using a collection of GPIO output pins
        /// </summary>
        /// <param name="Pins">The list of digital output pins to use</param>
        /// <param name="useActiveLowOutput">Whether a digital LOW value should be used to turn the LED on</param>
        /// <param name="controller">An IFlushable controller where the GPIO resides</param>
        public PwmLedDriver(IList<TPwm> Pins, bool useActiveLowOutput = false,
            IFlushable controller = null) : base(Pins.Count, false, true)
        {
            this.Pins = Pins;
            this.controller = controller;
            this.useActiveLowOutput = useActiveLowOutput;
        }

        /// <summary>
        ///     Flush the data from LED driver to the GPIO bus
        /// </summary>
        /// <param name="force">
        ///     Whether all data should be flushed, even if it doesn't appear to have changed from the current
        ///     value
        /// </param>
        /// <returns>An awaitable task that completes upon success</returns>
        public override async Task Flush(bool force = false)
        {
            if (controller != null)
                await controller.Flush().ConfigureAwait(false);
        }

        /// <summary>
        ///     Called by an LED when its brightness has changed
        /// </summary>
        /// <param name="led">The LED whose brightness has changed</param>
        internal override void LedBrightnessChanged(Led led)
        {
            update(led);
        }

        /// <summary>
        ///     Called by an LED when its state has changed
        /// </summary>
        /// <param name="led">The LED whose state has changed</param>
        internal override void LedStateChanged(Led led)
        {
            update(led);
        }

        private void update(Led led)
        {
            if (useActiveLowOutput)
                Pins[led.Channel].DutyCycle = led.State ? 1.0 - Utility.BrightnessToCieLuminance(led.Brightness) : 1.0;
            else
                Pins[led.Channel].DutyCycle = led.State ? Utility.BrightnessToCieLuminance(led.Brightness) : 0.0;
        }

        /// <summary>
        ///     Unused, since this driver doesn't implement global brightness control
        /// </summary>
        /// <param name="brightness"></param>
        internal override void SetGlobalBrightness(double brightness)
        {
        }
    }
}