using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Interface
{
    /// <summary>
    /// A class that represents a WriteOnlyParallelInterface constructed from a <see cref="IFlushableOutputPort{TDigitalPin}"/>
    /// </summary>
    /// <typeparam name="T">A type that implements <see cref="DigitalOut"/></typeparam>
    public class FlushableParallelInterface<T> : WriteOnlyParallelInterface where T : DigitalOut
    {
        private IFlushableOutputPort<T> port;

        /// <summary>
        /// A collection of pins to use for the data bus
        /// </summary>
        public Collection<DigitalOut> DataBus { get; set; } = new Collection<DigitalOut>();

        /// <summary>
        /// The Register Select (RS) pin to use
        /// </summary>
        public DigitalOut RegisterSelectPin { get; set; }

        /// <summary>
        /// The Read/Write (R/W) pin to use
        /// </summary>
        public DigitalOut ReadWritePin { get; set; }

        /// <summary>
        /// The enable (E) pin to use
        /// </summary>
        public DigitalOut EnablePin { get; set; }

        /// <summary>
        /// The number of microseconds to delay between transactions.
        /// </summary>
        public int DelayMicroseconds { get; set; }

        /// <summary>
        /// Construct a FlushableParallelInterface from a <see cref="IFlushableOutputPort{TDigitalPin}"/>. 
        /// </summary>
        /// <param name="outputPort"></param>
        public FlushableParallelInterface(IFlushableOutputPort<T> outputPort)
        {
            this.port = outputPort;
        }

        private bool enabled;
        private bool oldAutoflushSettings;

        /// <summary>
        /// Whether this parallel interface is enabled or disabled
        /// </summary>
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

        /// <summary>
        /// Gets the width of the bus
        /// </summary>
        public int Width
        {
            get
            {
                return DataBus.Count;
            }
        }

        /// <summary>
        /// Write one or more bytes to the command register
        /// </summary>
        /// <param name="command">The command data to write</param>
        /// <returns>An awaitable task that completes when the write operation finishes</returns>
        public Task WriteCommand(uint[] command)
        {
            return WriteDataOrCommand(command, false);
        }

        private async Task Pulse()
        {
            EnablePin.DigitalValue = false;
            await port.Flush().ConfigureAwait(false);
            EnablePin.DigitalValue = true;
            await port.Flush().ConfigureAwait(false);
            EnablePin.DigitalValue = false;
            await port.Flush().ConfigureAwait(false);
        }

        private void SetDataBus(uint value)
        {
            for (int i = 0; i < Width; i++)
            {
                if (((value >> i) & 0x01) == 0x01)
                    DataBus[i].DigitalValue = true;
                else
                    DataBus[i].DigitalValue = false;
            }
        }

        /// <summary>
        /// Write one or more bytes to the data register
        /// </summary>
        /// <param name="data">The data to write</param>
        /// <returns>An awaitable task that completes when the write operation finishes</returns>
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
