using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper.Libraries.Interface;
using Treehopper.Utilities;

namespace Treehopper.Libraries.Displays
{
    public class RgbLed : IFlushable, IRgbLed
    {
        private Led b;
        private Led g;
        private Led r;

        private List<ILedDriver> drivers = new List<ILedDriver>();

        public RgbLed(Led red, Led green, Led blue)
        {
            this.r = red;
            this.g = green;
            this.b = blue;

            if (!drivers.Contains(red.Driver))
                drivers.Add(red.Driver);
            if (!drivers.Contains(green.Driver))
                drivers.Add(green.Driver);
            if (!drivers.Contains(blue.Driver))
                drivers.Add(blue.Driver);

            // if all the drivers have autoflush disabled, we'll disable it, too
            if (drivers.Where(drv => drv.AutoFlush == true).Count() == 0)
                AutoFlush = false;

            r.Brightness = 0.0;
            r.State = true;

            g.Brightness = 0.0;
            g.State = true;

            b.Brightness = 0.0;
            b.State = true;
        }

        public RgbLed(IList<Led> Leds) : this(Leds[0], Leds[1], Leds[2])
        {

        }

        public bool AutoFlush { get; set; } = true;

        public async Task Flush(bool force = false)
        {
            foreach(var driver in drivers)
            {
                if (driver.AutoFlush) break; // already flushed out
                await driver.Flush();
            }
        }

        public float RedGain { get; set; } = 1f;
        public float GreenGain { get; set; } = 1f;
        public float BlueGain { get; set; } = 1f;

        public IFlushable Parent { get; private set; }

        public void SetRgb(float red, float green, float blue)
        {
            r.Brightness = Numbers.Constrain(red / 255f * RedGain);
            g.Brightness = Numbers.Constrain(green / 255f * GreenGain);
            b.Brightness = Numbers.Constrain(blue / 255f * BlueGain);

            if (AutoFlush) Flush().Wait();
        }

        public void SetRgb(Color color)
        {
            SetRgb(color.R, color.G, color.B);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hue"></param>
        /// <param name="saturation"></param>
        /// <param name="luminance"></param>
        /// <remarks><para>This is adapted from http://axonflux.com/handy-rgb-to-hsl-and-rgb-to-hsv-color-model-c </para></remarks>
        public void SetHsl(float hue, float saturation, float luminance)
        {
            SetRgb(Color.FromHsl(hue, saturation, luminance));
        }
    }
}
