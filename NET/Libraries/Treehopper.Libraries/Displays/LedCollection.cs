using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper.Libraries.Interface;

namespace Treehopper.Libraries.Displays
{
    public class LedCollection : List<Led>
    {
        public List<IFlushable> Drivers { get; private set; } = new List<IFlushable>();
        public LedCollection(IList<Led> Leds) : base(Leds)
        {
            var ledDrivers = Leds.Select(x => x.Driver).Distinct();
            foreach (var driver in ledDrivers)
                driver.AutoFlush = false;

            foreach(var driver in ledDrivers)
            {
                if (driver.Parent == null) // stand-alone or head of chain
                {
                    Drivers.Add(driver);
                }
                else if(!Drivers.Contains(driver.Parent))
                {
                    Drivers.Add(driver.Parent);
                }
            }
        }

        public async Task Flush(bool force = false)
        {
            // eventually, we should only write out LEDs that changed value, but for now, just write out all the LEDs
            foreach (var driver in Drivers)
                await driver.Flush(true); // we have to force because shift registers will otherwise ignore our writes
        }
    }
}
