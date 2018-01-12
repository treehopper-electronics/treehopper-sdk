using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Treehopper.Libraries.Utilities
{
    static class ColorConverter
    {
        /// <summary> 
        ///     Construct a color from HSL values 
        /// </summary> 
        /// <param name="hue">The hue, in degrees (0-360 mod)</param> 
        /// <param name="saturation">The saturation percentage, from 0 to 100</param> 
        /// <param name="luminance">The luminance percentage, from 0 to 100</param> 
        /// <returns></returns> 
        public static Color FromHsl(float hue, float saturation, float luminance)
        {
            var h = hue % 360 / 360.0f;
            var s = saturation / 100.0f;
            var l = luminance / 100.0f;

            float r, g, b;

            if (s == 0)
            {
                r = g = b = l; // achromatic 
            }
            else
            {
                var q = l < 0.5 ? l * (1 + s) : l + s - l * s;

                var p = 2 * l - q;
                r = hue2rgb(p, q, h + 1f / 3f);
                g = hue2rgb(p, q, h);
                b = hue2rgb(p, q, h - 1f / 3f);
            }

            return Color.FromArgb((int)Math.Round(r * 255), (int)Math.Round(g * 255), (int)Math.Round(b * 255));
        }

        private static float hue2rgb(float p, float q, float t)
        {
            if (t < 0) t += 1;
            if (t > 1) t -= 1;
            if (t < 1d / 6) return p + (q - p) * 6f * t;
            if (t < 1d / 2) return q;
            if (t < 2d / 3) return p + (q - p) * (2f / 3f - t) * 6f;
            return p;
        }
    }
}
