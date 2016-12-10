using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Displays
{
    /// <summary>
    /// An ILI9341-based display
    /// </summary>
    public class Ili9341 : GraphicDisplay
    {
        /// <summary>
        /// Construct an ILI9341-based display
        /// </summary>
        /// <param name="Width">The width of the display, in pixels</param>
        /// <param name="Height">The height of the display, in pixels</param>
        public Ili9341(int Width, int Height) : base(Width, Height, 2*Width)
        {
        }

        /// <summary>
        /// Flush the data to the display
        /// </summary>
        /// <returns>Completes when the display is flushed</returns>
        protected override Task flush()
        {
            throw new NotImplementedException();
        }

        protected override void setBrightness(double brightness)
        {
            
        }
    }
}
