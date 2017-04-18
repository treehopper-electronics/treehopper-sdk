using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Treehopper.Libraries.Interface;
using Treehopper.Utilities;

namespace Treehopper.Libraries.Displays
{
    /// <summary>
    /// Efficiently update multiple LED display widgets simultaneously.
    /// </summary>
    public class LedDisplayCollection : Collection<LedDisplay>, IFlushable
    {
        /// <summary>
        /// Construct a new LED display collection
        /// </summary>
        public LedDisplayCollection() : base()
        {

        }
        private bool autoFlush = false;

        /// <summary>
        /// Gets / sets whether the drivers should be auto-flushed or not.
        /// </summary>
        public bool AutoFlush {
            get
            {
                return autoFlush;
            }
            set
            {
                autoFlush = value;
                foreach (var item in Items)
                    item.AutoFlush = autoFlush;
            }
        }

        /// <summary>
        /// The parent of this Flushable interface. This property is unused.
        /// </summary>
        public IFlushable Parent { get; set; }

        /// <summary>
        /// Flush every display in this collection to the LED drivers
        /// </summary>
        /// <param name="force">Whether to force an update, even if the driver doesn't appear to have a different state</param>
        /// <returns></returns>
        public async Task Flush(bool force = false)
        {
            if (autoFlush) return; // if autoflush is on, individual displays manage their LEDs; no need to write out other updates

            // program the LEDs in each driver
            Items.ForEach(disp => disp.WriteLeds());

            // Write out the drivers over the bus
            foreach(var driver in Items.SelectMany(x => x.Leds.Drivers).Distinct())
            {
                await driver.Flush(true);
            }
        }

        /// <summary>
        /// Insert a new LED display widget
        /// </summary>
        /// <param name="index">The index to insert the item at</param>
        /// <param name="item">The LED display widget to insert</param>
        protected override void InsertItem(int index, LedDisplay item)
        {
            base.InsertItem(index, item);
            item.AutoFlush = autoFlush;
        }

        /// <summary>
        /// Set the LED display widget at the given index
        /// </summary>
        /// <param name="index">The index to set</param>
        /// <param name="item">The item to set</param>
        protected override void SetItem(int index, LedDisplay item)
        {
            base.SetItem(index, item);
            item.AutoFlush = autoFlush;
        }

        /// <summary>
        /// Remove the LED display widget at the given index
        /// </summary>
        /// <param name="index">The index of the item to remove</param>
        protected override void RemoveItem(int index)
        {
            Items[index].AutoFlush = true;
            base.RemoveItem(index);
        }

        /// <summary>
        /// Remove all LED display widgets from this collection
        /// </summary>
        protected override void ClearItems()
        {
            Items.ForEach(x => x.AutoFlush = true);
            base.ClearItems();
        }
    }
}
