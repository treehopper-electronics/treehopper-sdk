using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper;

namespace Treehopper.Libraries.Max7219
{
    public class Max7219 : SpiDevice
    {
        /// <summary>
        /// Construct a new instance of a Max7219 device
        /// </summary>
        /// <param name="SpiModule">A reference to the Treehopper SPI module</param>
        /// <param name="LoadPin">The pin attached tot he LOAD input</param>
        /// <param name="Address">The index of the Max7219 device attached to this bus</param>
        /// <param name="SpeedMHz">The SPI speed to use. The maximum is 10 MHz.</param>
        public Max7219(Spi SpiModule, Pin LoadPin, int Address = 0, double SpeedMHz = 1) : base(SpiModule, LoadPin, SpeedMHz)
        {
            if (SpeedMHz > 10)
                throw new ArgumentOutOfRangeException("SpeedMhz", "The MAX7219 supports a maximum clock rate of 10 MHz.");
            this.address = Address;
            Test = false;
            ScanLimit = 7;
            DecodeMode = 0;
            Clear().Wait();
            Shutdown = false;
            Intensity = 1;
        }

        int address;

        private bool test = true;
        /// <summary>
        /// Gets or sets whether the display is in test mode or not.
        /// </summary>
        public bool Test
        {
            get { return test; }
            set
            {
                if (test == value) return;
                test = value;
                Send(Opcode.DisplayTest, (byte)(test ? 0x01 : 0x00)).Wait();
            }
        }
        
        private byte decodeMode = 0;
        /// <summary>
        /// Gets or sets a bit pattern indicating whether each digit should be decoded on-chip or not.
        /// </summary>
        public byte DecodeMode
        {
            get { return decodeMode; }
            set
            {
                if (decodeMode == value) return;
                decodeMode = value;
                Send(Opcode.DecodeMode, decodeMode).Wait();
            }
        }

        private bool shutdown = true;
        /// <summary>
        /// Gets or sets whether the display should be shut down or not
        /// </summary>
        public bool Shutdown
        {
            get { return shutdown; }
            set
            {
                if (shutdown == value) return;
                shutdown = value;
                Send(Opcode.Shutdown, (byte)(shutdown ? 0x00 : 0x01)).Wait();
            }
        }

        private byte scanLimit = 0;
        /// <summary>
        /// Gets or sets the maximum digit index (from 0 to 7) that should be scanned
        /// </summary>
        public byte ScanLimit
        {
            get { return scanLimit; }
            set
            {
                if (scanLimit == value) return;
                scanLimit = value;
                Send(Opcode.ScanLimit, scanLimit).Wait();
            }
        }


        private double intensity = 0;
        /// <summary>
        /// Gets or sets the intensity (0-1) of the display.
        /// </summary>
        public double Intensity
        {
            get { return intensity; }
            set
            {
                if (intensity == value) return;
                if (intensity < 0 || intensity > 1)
                    throw new ArgumentOutOfRangeException("Valid Intensity is from 0 to 1");
                intensity = value;
                Send(Opcode.Intensity, (byte)(intensity*15.0)).Wait();
            }
        }

        /// <summary>
        /// Clears the display by setting all LEDs to off.
        /// </summary>
        /// <returns>An awaitable task</returns>
        public async Task Clear()
        {
            for (byte i = 0; i < 8; i++)
            {
                await Send((Opcode)(i + 1), 0);
            }
        }

        private static int numDevices = 0;
        protected async Task Send(Opcode opcode, byte data)
        {
            if (numDevices < address + 1)
                numDevices = address + 1;
            //Create an array with the data to shift out
            int offset = address * 2;
            int maxbytes = numDevices * 2;

            var spiData = new byte[maxbytes];

            for (int i = 0; i < maxbytes; i++)
                spiData[i] = (byte)0;

            //put our device data into the array
            spiData[offset + 1] = (byte)opcode;
            spiData[offset] = data;

            Array.Reverse(spiData);

            await SendReceive(spiData);
        }
    }

    public enum Opcode
    {
        Noop = 0,
        Digit0 = 1,
        Digit1 = 2,
        Digit2 = 3,
        Digit3 = 4,
        Digit4 = 5,
        Digit5 = 6,
        Digit6 = 7,
        Digit7 = 8,
        DecodeMode = 9,
        Intensity = 10,
        ScanLimit = 11,
        Shutdown = 12,
        DisplayTest = 15
    }
}
