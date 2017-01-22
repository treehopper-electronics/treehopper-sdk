using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper.Libraries.Interface;
using Treehopper.Utilities;

namespace Treehopper.Libraries.Displays
{
    public class LedGraphicDisplay : MonoGraphicDisplay, LedDisplay
    {
        public LedCollection Leds { get; private set; }

        public IFlushable Parent { get; set; }
        public bool AutoFlush { get; set; } = true;

        public LedGraphicDisplay(IList<Led> leds, int width, int height) : base(width, height)
        {
            Leds = new LedCollection(leds);
        }

        protected override async Task flush()
        {
            if (AutoFlush)
                await Flush().ConfigureAwait(false);
        }

        protected override void setBrightness(double brightness)
        {
            Leds.Select(x => x.Driver).Distinct().ForEach(drv => drv.Brightness = brightness);
        }

        public void WriteLeds()
        {
            for (int i = 0; i < RawBuffer.Length; i++)
            {
                byte data = RawBuffer[i];
                for (int j = 0; j < 8; j++)
                {
                    int idx = (8 * i) + j;
                    Leds[idx].State = (((data >> j) & 0x01) == 0x01) ? true : false;
                }
            }
        }

        public Task Flush(bool force = false)
        {
            WriteLeds();
            return Leds.Flush(force);
        }
    }
}
