using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Treehopper.Libraries.IO;

namespace Treehopper.Libraries.Displays
{
    /// <summary>
    ///     Manages efficient updates of a collection of LEDs that may be attached to multiple drivers.
    /// </summary>
    public class LedCollection : List<Led>
    {
        /// <summary>
        ///     Construct a new LED collection from the provided list of LEDs
        /// </summary>
        /// <param name="Leds">The LEDs to build the collection on</param>
        public LedCollection(IList<Led> Leds) : base(Leds)
        {
            var ledDrivers = Leds.Select(x => x.Driver).Distinct();
            foreach (var driver in ledDrivers)
                driver.AutoFlush = false;

            foreach (var driver in ledDrivers)
                if (driver.Parent == null && !Drivers.Contains(driver)) // stand-alone or head of chain
                    Drivers.Add(driver);
                else if (!Drivers.Contains(driver.Parent))
                    Drivers.Add(driver.Parent);
        }

        /// <summary>
        ///     The drivers that belong to this LED collection
        /// </summary>
        public List<IFlushable> Drivers { get; } = new List<IFlushable>();

        /// <summary>
        ///     Flush the values of the LEDs to the driver(s) the LEDs are attached to
        /// </summary>
        /// <param name="force">Whether to force an update</param>
        /// <returns></returns>
        public async Task Flush(bool force = false)
        {
            // eventually, we should only write out LEDs that changed value, but for now, just write out all the LEDs
            foreach (var driver in Drivers)
                await driver.FlushAsync(true); // we have to force because shift registers will otherwise ignore our writes
        }
    }
}