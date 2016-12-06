using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper.Libraries.Interface;

namespace Treehopper.Libraries.Displays
{
    public class GpioLedDriver<TDigitalOutPin> : LedDriver where TDigitalOutPin : DigitalOutPin
    {
        private readonly IFlushable controller;
        private IList<TDigitalOutPin> Pins;
        private readonly bool useActiveLowOutput;

        public GpioLedDriver(IList<TDigitalOutPin> Pins, bool useActiveLowOutput = false, IFlushable controller = null) : base(Pins.Count, false, false)
        {
            this.Pins = Pins;
            this.controller = controller;
            this.useActiveLowOutput = useActiveLowOutput;
        }

        public override async Task Flush(bool force = false)
        {
            if (controller != null)
                await controller.Flush().ConfigureAwait(false);
        }

        public override void LedBrightnessChanged(Led led)
        {
            
        }

        public override void LedStateChanged(Led led)
        {
            int idx = led.Channel;
            if(useActiveLowOutput)
                Pins[idx].DigitalValue = !led.State;
            else
                Pins[idx].DigitalValue = led.State;
        }
    }
}
