using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Displays
{
    public class LedGraphicDisplay : MonoGraphicDisplay
    {
        private IList<Led> leds;

        private IList<ILedDriver> drivers = new List<ILedDriver>();

        public LedGraphicDisplay(IList<Led> leds, int width, int height) : base(width, height)
        {
            this.leds = leds;



            foreach(var led in leds)
            {
                if (!drivers.Contains(led.Driver))
                    drivers.Add(led.Driver);
            }

            foreach (var driver in drivers)
                driver.AutoFlush = false;
        }

        protected override async Task flush()
        {
            for(int i=0;i< RawBuffer.Length;i++)
            {
                byte data = RawBuffer[i];
                for (int j = 0; j < 8; j++)
                {
                    int idx = (8*i) + j;
                    leds[idx].State = (((data >> j) & 0x01) == 0x01) ? true : false;
                }
                    
            }
            foreach(var driver in drivers)
            {
                await driver.Flush();
            }
        }

        protected override void setBrightness(double brightness)
        {
            foreach (var driver in drivers)
                driver.Brightness = brightness;
        }
    }
}
