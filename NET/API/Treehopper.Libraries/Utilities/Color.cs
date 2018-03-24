using System;
using System.Collections.Generic;
using System.Text;

namespace Treehopper.Libraries.Utilities
{
    public class Color
    {
        [Flags]
        internal enum ColorType : short
        {
            Empty = 0,
            Known = 1,
            ARGB = 2,
            Named = 4,
            System = 8
        }

        internal short state;
        internal short knownColor;

        internal long Value { get; set; }

        public static Color FromArgb(int alpha, Color baseColor)
        {
            return FromArgb(alpha, baseColor.R, baseColor.G, baseColor.B);
        }

        public static Color FromArgb(int argb)
        {
            return FromArgb((argb >> 24) & 0x0FF, (argb >> 16) & 0x0FF, (argb >> 8) & 0x0FF, argb & 0x0FF);
        }

        public static Color FromArgb(int red, int green, int blue)
        {
            return FromArgb(255, red, green, blue);
        }

        public static Color FromArgb(int alpha, int red, int green, int blue)
        {
            CheckARGBValues(alpha, red, green, blue);
            Color color = new Color();
            color.state = (short)ColorType.ARGB;
            color.Value = (int)((uint)alpha << 24) + (red << 16) + (green << 8) + blue;
            return color;
        }

        /// <summary> 
        ///     Construct a color from HSL values 
        /// </summary> 
        /// <param name="hue">The hue, in degrees (0-360 mod)</param> 
        /// <param name="saturation">The saturation percentage, from 0 to 100</param> 
        /// <param name="luminance">The luminance percentage, from 0 to 100</param> 
        /// <returns>A Color with the HSL values</returns> 
        public static Color FromHsl(float hue, float saturation, float luminance)
        {
            while (hue < 0)
                hue += 360;

            var h = Math.Max((hue % 360) / 360.0f, 0);
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

            return FromArgb((int)Math.Round(r * 255), (int)Math.Round(g * 255), (int)Math.Round(b * 255));
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

        public byte A
        {
            get { return (byte)(Value >> 24); }
        }

        public byte R
        {
            get { return (byte)(Value >> 16); }
        }

        public byte G
        {
            get { return (byte)(Value >> 8); }
        }

        public byte B
        {
            get { return (byte)Value; }
        }


        public int ToArgb()
        {
            return (int)Value;
        }

        /// <summary>
        ///	Equality Operator
        /// </summary>
        ///
        /// <remarks>
        ///	Compares two Color objects. The return value is
        ///	based on the equivalence of the A,R,G,B properties 
        ///	of the two Colors.
        /// </remarks>

        public static bool operator ==(Color left, Color right)
        {
            if (left.Value != right.Value)
                return false;
            return true;
        }

        /// <summary>
        ///	Inequality Operator
        /// </summary>
        ///
        /// <remarks>
        ///	Compares two Color objects. The return value is
        ///	based on the equivalence of the A,R,G,B properties 
        ///	of the two colors.
        /// </remarks>

        public static bool operator !=(Color left, Color right)
        {
            return !(left == right);
        }

        public float GetBrightness()
        {
            byte minval = Math.Min(R, Math.Min(G, B));
            byte maxval = Math.Max(R, Math.Max(G, B));

            return (float)(maxval + minval) / 510;
        }

        public float GetSaturation()
        {
            byte minval = (byte)Math.Min(R, Math.Min(G, B));
            byte maxval = (byte)Math.Max(R, Math.Max(G, B));

            if (maxval == minval)
                return 0.0f;

            int sum = maxval + minval;
            if (sum > 255)
                sum = 510 - sum;

            return (float)(maxval - minval) / sum;
        }

        public float GetHue()
        {
            int r = R;
            int g = G;
            int b = B;
            byte minval = (byte)Math.Min(r, Math.Min(g, b));
            byte maxval = (byte)Math.Max(r, Math.Max(g, b));

            if (maxval == minval)
                return 0.0f;

            float diff = (float)(maxval - minval);
            float rnorm = (maxval - r) / diff;
            float gnorm = (maxval - g) / diff;
            float bnorm = (maxval - b) / diff;

            float hue = 0.0f;
            if (r == maxval)
                hue = 60.0f * (6.0f + bnorm - gnorm);
            if (g == maxval)
                hue = 60.0f * (2.0f + rnorm - bnorm);
            if (b == maxval)
                hue = 60.0f * (4.0f + gnorm - rnorm);
            if (hue > 360.0f)
                hue = hue - 360.0f;

            return hue;
        }

        private static void CheckARGBValues(int alpha, int red, int green, int blue)
        {
            if ((alpha > 255) || (alpha < 0))
                throw CreateColorArgumentException(alpha, "alpha");
            CheckRGBValues(red, green, blue);
        }

        private static void CheckRGBValues(int red, int green, int blue)
        {
            if ((red > 255) || (red < 0))
                throw CreateColorArgumentException(red, "red");
            if ((green > 255) || (green < 0))
                throw CreateColorArgumentException(green, "green");
            if ((blue > 255) || (blue < 0))
                throw CreateColorArgumentException(blue, "blue");
        }

        private static ArgumentException CreateColorArgumentException(int value, string color)
        {
            return new ArgumentException(string.Format("'{0}' is not a valid"
                + " value for '{1}'. '{1}' should be greater or equal to 0 and"
                + " less than or equal to 255.", value, color));
        }
    }
}
