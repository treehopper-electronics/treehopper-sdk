using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper.Libraries.Interface;

namespace Treehopper.Libraries.Displays
{
    /// <summary>
    /// A class representing pixel-addressed displays
    /// </summary>
    public abstract class GraphicDisplay : IFlushable
    {
        /// <summary>
        /// The height of the display, in pixels
        /// </summary>
        public int Height { get; protected set; }

        /// <summary>
        /// The width of the display, in pixels.
        /// </summary>
        public int Width { get; protected set; }

        /// <summary>
        /// The raw byte buffer used by the display in its native format
        /// </summary>
        /// <remarks>
        /// <para>The format of this raw buffer depends on the particular implementation of this class</para>
        /// </remarks>
        public byte[] RawBuffer { get; protected set; }

        /// <summary>
        /// Construct a graphic display of given width and height
        /// </summary>
        /// <param name="Width">The width, in pixels, of the display</param>
        /// <param name="Height">The height, in pixels, of the display</param>
        /// <param name="bytesPerRow">The number of bytes per row (stride) of the display</param>
        public GraphicDisplay(int Width, int Height, double bytesPerPixel)
        {
            this.Width = Width;
            this.Height = Height;

            RawBuffer = new byte[(int)Math.Round(Width * Height * bytesPerPixel)];
        }

        /// <summary>
        /// Clear the display
        /// </summary>
        /// <returns>An awaitable task</returns>
        public Task Clear()
        {
            for (int i = 0; i < RawBuffer.Length; i++)
                RawBuffer[i] = 0;

            return Flush();
        }

        private double brightness = 1.0;

        /// <summary>
        /// Gets or sets the global brightness of the display
        /// </summary>
        public double Brightness
        {
            get { return brightness; }
            set {
                if (Math.Abs(brightness - value) < 0.005) return;

                brightness = value;

                setBrightness(brightness);

            }
        }

        public bool AutoFlush { get; set; } = true;

        public IFlushable Parent { get; }

        /// <summary>
        /// Sets the global brightness of the graphic display
        /// </summary>
        /// <param name="brightness">The brightness of the display</param>
        protected abstract void setBrightness(double brightness);

        /// <summary>
        /// Flush any updates to the RawBuffer to the display
        /// </summary>
        /// <returns>An awaitable task that completes upon sending the update to the display</returns>

        /// <summary>
        /// Internal function called by the implementation to flush the RawBuffer to the display
        /// </summary>
        /// <returns>An awaitable task that completes upon success</returns>
        protected abstract Task flush();

        public Task Flush(bool force = false)
        {
            return flush();
        }
    }
}
