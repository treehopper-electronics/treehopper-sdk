using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Displays
{
    public class Utility
    {
        /// <summary>
        /// Returns the CIE 1976 relative luminance calculated from an input intensity (brightness)
        /// </summary>
        /// <param name="brightness">The input intensity, from 0.0-1.0, to convert</param>
        /// <returns>The CIE 1976 relative luminance, normalized to a 0-1 scale</returns>
        /// <remarks>
        /// <para>"Brightness" is perception oriented for intuitiveness. This function can be used to convert the perceptual value to a normalized luminance value that an LED driver can use to determine the correct PWM duty cycle or analog current drive strength needed to produce the associated brightness.</para>
        /// </remarks>
        public static double BrightnessToCieLuminance(double brightness)
        {
            if (brightness > 0.008856)
                return ((15625 * Math.Pow(brightness, 3)) + (7500 * Math.Pow(brightness, 2)) + (1200 * brightness) + 64) / 24389.0;
            else
                return 1000 * brightness / 9033.0;
        }
    }
}
