using System.Collections.Generic;
using System.Threading.Tasks;
using Treehopper.Libraries.Interface;

namespace Treehopper.Libraries.Displays
{
    /// <summary>
    /// An LedDriver implemented using plain digital output pins
    /// </summary>
    /// <typeparam name="TDigitalOutPin">The DigitalOutPin type that should be used.</typeparam>
    public class GpioLedDriver<TDigitalOutPin> : LedDriver where TDigitalOutPin : DigitalOut
    {
        private readonly IFlushable controller;
        private readonly IList<TDigitalOutPin> Pins;
        private readonly bool useActiveLowOutput;

        /// <summary>
        /// Construct a new LedDriver using a collection of GPIO output pins
        /// </summary>
        /// <param name="Pins">The list of digital output pins to use</param>
        /// <param name="useActiveLowOutput">Whether a digital LOW value should be used to turn the LED on</param>
        /// <param name="controller">An IFlushable controller where the GPIO resides</param>
        public GpioLedDriver(IList<TDigitalOutPin> Pins, bool useActiveLowOutput = false, IFlushable controller = null) : base(Pins.Count, false, false)
        {
            this.Pins = Pins;
            this.controller = controller;
            this.useActiveLowOutput = useActiveLowOutput;
        }

        /// <summary>
        /// Flush the data from LED driver to the GPIO bus
        /// </summary>
        /// <param name="force">Whether all data should be flushed, even if it doesn't appear to have changed from the current value</param>
        /// <returns>An awaitable task that completes upon success</returns>
        public override async Task Flush(bool force = false)
        {
            if (controller != null)
                await controller.Flush().ConfigureAwait(false);
        }

        /// <summary>
        /// Called by an LED when its brightness has changed
        /// </summary>
        /// <param name="led">The LED whose brightness has changed</param>
        internal override void LedBrightnessChanged(Led led)
        {
            
        }

        /// <summary>
        /// Called by an LED when its state has changed
        /// </summary>
        /// <param name="led">The LED whose state has changed</param>
        internal override void LedStateChanged(Led led)
        {
            int idx = led.Channel;
            if(useActiveLowOutput)
                Pins[idx].DigitalValue = !led.State;
            else
                Pins[idx].DigitalValue = led.State;
        }

        internal override void SetGlobalBrightness(double brightness)
        {
            
        }
    }
}
