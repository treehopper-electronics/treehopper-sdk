using Treehopper.Libraries.Interface;

namespace Treehopper.Libraries.Displays
{
    /// <summary>
    /// Interface for a display widget made of multiple LEDs.
    /// </summary>
    public interface LedDisplay : IFlushable
    {
        /// <summary>
        /// The collection of LEDs that belong to this display widget
        /// </summary>
        LedCollection Leds { get; }

        /// <summary>
        /// Write the LEDs without flushing the drivers
        /// </summary>
        void WriteLeds();
    }
}
