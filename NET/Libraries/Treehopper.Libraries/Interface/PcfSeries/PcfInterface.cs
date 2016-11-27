using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Interface.PcfSeries
{
    public class PcfInterface : PortExpander
    {
        private SMBusDevice dev;
        int numBytes;
        public PcfInterface(II2c i2c, int numPins, bool Address0, bool Address1, bool Address2, byte baseAddress) : base(numPins)
        {
            byte address = (byte)(baseAddress | (Address0 ? 1 : 0) | (Address1 ? 1 : 0) << 1 | (Address2 ? 1 : 0) << 2);
            dev = new SMBusDevice(address, i2c);

            numBytes = numPins / 8;

            oldValues = new byte[numBytes];
            newValues = new byte[numBytes];
            
            // make all pins inputs by default
            AutoFlush = false;
            foreach (var pin in Pins)
                pin.Mode = PinMode.DigitalInput;
            AutoFlush = true;
            Flush(true).Wait();

        }

        byte[] oldValues;
        byte[] newValues;

        public override async Task Flush(bool force = false)
        {
            for (int i = 0; i < Pins.Count; i++)
            {
                // recall that we make a pin a digital input by setting it high and reading from it
                if (Pins[i].DigitalValue == true || Pins[i].Mode == PinMode.DigitalInput)
                    newValues[i / 8] |= (byte)(1 << (i % 8));
                else
                    newValues[i / 8] &= (byte)~(1 << (i % 8));
            }

            bool shouldResend = false;

            for (int i = 0; i < oldValues.Length; i++)
            {
                if (oldValues[i] != newValues[i])
                    shouldResend = true;
            }

            if (shouldResend || force)
            {
                newValues.CopyTo(oldValues, 0);

                await dev.WriteData(newValues);
            }
                
        }

        protected override void outputValueChanged(PortExpanderPin portExpanderPin)
        {
            Flush().Wait();
        }

        protected override void outputModeChanged(PortExpanderPin portExpanderPin)
        {
            // we set the I/O mode by writing a 1 or 0 to the output port, so just
            // flush out the data
            Flush().Wait();
        }

        protected override async Task readPort()
        {
            var data = await dev.ReadData((byte)numBytes);
            for(int i = 0;i<Pins.Count;i++)
            {
                int bank = i / 8;
                int bit = i % 8;
                if (data[bank] >> bit == 0x01)
                    Pins[i].UpdateInputValue(true);
                else
                    Pins[i].UpdateInputValue(false);
            }
        }
    }
}
