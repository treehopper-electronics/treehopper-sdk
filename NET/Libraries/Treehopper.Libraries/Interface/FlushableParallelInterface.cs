using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Interface
{
    public class FlushableParallelInterface<T> : IWriteOnlyParallelInterface where T : DigitalOutPin
    {
        private IFlushableOutputPort<T> port;

        public Collection<DigitalOutPin> DataBus { get; set; } = new Collection<DigitalOutPin>();
        public DigitalOutPin RegisterSelectPin { get; set; }
        public DigitalOutPin ReadWritePin { get; set; }

        public DigitalOutPin EnablePin { get; set; }

        public int DelayMicroseconds { get; set; }

        public FlushableParallelInterface(IFlushableOutputPort<T> outputPort)
        {
            this.port = outputPort;
        }

        private bool enabled;
        private bool oldAutoflushSettings;
        public bool Enabled
        {
            get
            {
                return enabled;
            }

            set
            {
                if (enabled == value) return;
                enabled = value;
                if(enabled)
                {
                    oldAutoflushSettings = port.AutoFlush; // save old autoflush settings
                    port.AutoFlush = false; // disable autoflush for speed

                    // make sure pins are outputs
                    foreach (var pin in DataBus)
                        pin.MakeDigitalPushPullOut();

                    RegisterSelectPin.MakeDigitalPushPullOut();
                    if(ReadWritePin != null) // R/W is optional, and is often tied to "write"
                        ReadWritePin.MakeDigitalPushPullOut();
                    EnablePin.MakeDigitalPushPullOut();

                    port.Flush().Wait(); // write out port settings

                } else
                {
                    port.AutoFlush = oldAutoflushSettings; // restore old autoflush settings
                }
            }
        }

        public int Width
        {
            get
            {
                return DataBus.Count;
            }
        }

        public Task WriteCommand(uint[] command)
        {
            return WriteDataOrCommand(command, false);
        }

        public async Task Pulse()
        {
            EnablePin.DigitalValue = false;
            await port.Flush().ConfigureAwait(false);
            EnablePin.DigitalValue = true;
            await port.Flush().ConfigureAwait(false);
            EnablePin.DigitalValue = false;
            await port.Flush().ConfigureAwait(false);
        }

        public void SetDataBus(uint value)
        {
            for (int i = 0; i < Width; i++)
            {
                if (((value >> i) & 0x01) == 0x01)
                    DataBus[i].DigitalValue = true;
                else
                    DataBus[i].DigitalValue = false;
            }
        }

        public Task WriteData(uint[] data)
        {
             return WriteDataOrCommand(data, true);
        }

        private async Task WriteDataOrCommand(uint[] busValues, bool isData = false)
        {
            RegisterSelectPin.DigitalValue = isData;
            if (ReadWritePin != null)
                ReadWritePin.DigitalValue = false;

            foreach(var val in busValues)
            {
                SetDataBus(val);
                await Pulse().ConfigureAwait(false);
            }
            
        }
    }
}
