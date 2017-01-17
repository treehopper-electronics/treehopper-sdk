using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper.Libraries.Interface;

namespace Treehopper.Libraries.Displays
{
    /// <summary>
    /// Efficiently update multiple LED displays
    /// </summary>
    public class LedDisplayCollection : Collection<LedDisplay>, IFlushable
    {
        public LedDisplayCollection() : base()
        {

        }
        private bool autoFlush = false;
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
        public IFlushable Parent { get; set; }

        public async Task Flush(bool force = false)
        {
            if (autoFlush) return;

            // program the LEDs in each driver
            Items.ForEach(disp => disp.WriteLeds());

            // Write out the drivers over the bus
            Items.SelectMany(x => x.Leds.Drivers).Distinct().ForEach(async driver => await driver.Flush(true));
        }

        protected override void InsertItem(int index, LedDisplay item)
        {
            base.InsertItem(index, item);
            item.AutoFlush = autoFlush;
        }

        protected override void SetItem(int index, LedDisplay item)
        {
            base.SetItem(index, item);
            item.AutoFlush = autoFlush;
        }

        protected override void RemoveItem(int index)
        {
            Items[index].AutoFlush = true;
            base.RemoveItem(index);
        }

        protected override void ClearItems()
        {
            Items.ForEach(x => x.AutoFlush = true);
            base.ClearItems();
        }
    }
}
