using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper.Libraries.Interface;
using Treehopper.Utilities;

namespace Treehopper.Libraries.Displays
{
    /// <summary>
    /// An RGB LED connected to three discrete channels of LED driver(s).
    /// </summary>
    /// <remarks>
    /// <para>
    /// Note the RGB LED may be connected across multiple drivers with no impact on functionality. This is common for large arrays of RGB LEDs (which require multiple-of-3 number of channels) interfacing with multiple 8-, 16-, or 24-channel drivers.
    /// </para>
    /// </remarks>
    public class RgbLed : IFlushable, IRgbLed
    {
        private Led b;
        private Led g;
        private Led r;

        private List<ILedDriver> drivers = new List<ILedDriver>();

        /// <summary>
        /// Construct a new RGB LED from the specified LED driver channels
        /// </summary>
        /// <param name="red">The red LED</param>
        /// <param name="green">The green LED</param>
        /// <param name="blue">The blue LED</param>
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

        /// <summary>
        /// Construct a new RGB LED from a three-item list of LEDs
        /// </summary>
        /// <param name="Leds"></param>
        public RgbLed(IList<Led> Leds) : this(Leds[0], Leds[1], Leds[2])
        {

        }

        /// <summary>
        /// Whether calls to <see cref="SetRgb(Color)"/>, <see cref="SetHsl(float, float, float)"/>, or <see cref="SetRgb(float, float, float)"/> should flush to the drivers immediately
        /// </summary>
        public bool AutoFlush { get; set; } = true;

        /// <summary>
        /// Flush this LED's color value to the driver(s)
        /// </summary>
        /// <param name="force">Whether to force an update</param>
        /// <returns>An awaitable task</returns>
        public async Task Flush(bool force = false)
        {
            foreach(var driver in drivers)
            {
                if (driver.AutoFlush) break; // already flushed out
                await driver.Flush();
            }
        }

        /// <summary>
        /// The red gain to use with this RGB LED
        /// </summary>
        public float RedGain { get; set; } = 1f;

        /// <summary>
        /// The green gain to use with this RGB LED
        /// </summary>
        public float GreenGain { get; set; } = 1f;

        /// <summary>
        /// The blue gain to use with this RGB LED
        /// </summary>
        public float BlueGain { get; set; } = 1f;

        /// <summary>
        /// The parent of this instance. Unused.
        /// </summary>
        public IFlushable Parent { get; private set; }

        /// <summary>
        /// Set the RGB value of this RGB LED
        /// </summary>
        /// <param name="red">The red intensity, from 0-255</param>
        /// <param name="green">The green intensity, from 0-255</param>
        /// <param name="blue">The blue intensity, from 0-255</param>
        public void SetRgb(float red, float green, float blue)
        {
            r.Brightness = Numbers.Constrain(red / 255f * RedGain);
            g.Brightness = Numbers.Constrain(green / 255f * GreenGain);
            b.Brightness = Numbers.Constrain(blue / 255f * BlueGain);

            if (AutoFlush) Flush().Wait();
        }

        /// <summary>
        /// Set the color of the RGB LED
        /// </summary>
        /// <param name="color">The desired color</param>
        public void SetRgb(Color color)
        {
            SetRgb(color.R, color.G, color.B);
        }

        /// <summary>
        /// Set the hue, saturation, and luminance of this RGB LED
        /// </summary>
        /// <param name="hue">The hue, from 0-360 degrees, of the desired color</param>
        /// <param name="saturation">The saturation, from 0-100, of the desired color</param>
        /// <param name="luminance">The luminance, from 0-100, of the desired color</param>
        public void SetHsl(float hue, float saturation, float luminance)
        {
            SetRgb(Color.FromHsl(hue, saturation, luminance));
        }
    }
}
