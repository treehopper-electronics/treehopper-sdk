using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper.Libraries.Interface;

namespace Treehopper.Libraries.Displays
{
    public class RgbLed : IFlushable
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


            r.Brightness = 0.0;
            r.State = true;

            g.Brightness = 0.0;
            g.State = true;

            b.Brightness = 0.0;
            b.State = true;
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

        public double RedGain { get; set; } = 1.0;
        public double GreenGain { get; set; } = 1.0;
        public double BlueGain { get; set; } = 1.0;

        public IFlushable Parent { get; private set; }

        public void SetRgb(double red, double green, double blue)
        {
            r.Brightness = Utilities.Constrain(red / 255.0 * RedGain);
            g.Brightness = Utilities.Constrain(green / 255.0 * GreenGain);
            b.Brightness = Utilities.Constrain(blue / 255.0 * BlueGain);

            if (AutoFlush) Flush().Wait();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hue"></param>
        /// <param name="saturation"></param>
        /// <param name="luminance"></param>
        /// <remarks><para>This is adapted from http://axonflux.com/handy-rgb-to-hsl-and-rgb-to-hsv-color-model-c </para></remarks>
        public void SetHsl(double hue, double saturation, double luminance)
        {
            var h = hue / 360.0;
            var s = saturation / 100.0;
            var l = luminance / 100.0;

            double r, g, b;

            if (s == 0)
            {
                r = g = b = l; // achromatic
            } else
            {
                var q = l < 0.5 ? l * (1 + s) : l + s - l * s;

                var p = 2 * l - q;
                r = hue2rgb(p, q, h + (1d/3));
                g = hue2rgb(p, q, h);
                b = hue2rgb(p, q, h - (1d/3));
            }


            SetRgb(255.0*r, 255.0*g, 255.0*b);
        }

        private double hue2rgb(double p, double q, double t)
        {
            if (t < 0) t += 1;
            if (t > 1) t -= 1;
            if (t < 1d/6) return p + (q - p) * 6d * t;
            if (t < 1d/2) return q;
            if (t < 2d/3) return p + (q - p) * (2d/3 - t) * 6;
            return p;
        }
    }
}
