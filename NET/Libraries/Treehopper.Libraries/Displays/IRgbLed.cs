using System.Drawing;

namespace Treehopper.Libraries.Displays
{
    internal interface IRgbLed
    {
        /// <summary>
        ///     The red gain to use with this RGB LED
        /// </summary>
        float RedGain { get; set; }

        /// <summary>
        ///     The green gain to use with this RGB LED
        /// </summary>
        float GreenGain { get; set; }

        /// <summary>
        ///     The blue gain to use with this RGB LED
        /// </summary>
        float BlueGain { get; set; }

        /// <summary>
        ///     Set the color of the RGB LED
        /// </summary>
        /// <param name="color">The desired color</param>
        void SetRgb(Color color);

        /// <summary>
        ///     Set the RGB value of this RGB LED
        /// </summary>
        /// <param name="red">The red intensity, from 0-255</param>
        /// <param name="green">The green intensity, from 0-255</param>
        /// <param name="blue">The blue intensity, from 0-255</param>
        void SetRgb(float red, float green, float blue);

        /// <summary>
        ///     Set the hue, saturation, and luminance of this RGB LED
        /// </summary>
        /// <param name="hue">The hue, from 0-360 degrees, of the desired color</param>
        /// <param name="saturation">The saturation, from 0-100, of the desired color</param>
        /// <param name="luminance">The luminance, from 0-100, of the desired color</param>
        void SetHsl(float hue, float saturation, float luminance);
    }
}