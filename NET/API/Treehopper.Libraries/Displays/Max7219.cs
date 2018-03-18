using System;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Displays
{
    /// <summary>
    ///     Maxim MAX7219 SPI 8x8 matrix LED driver
    /// </summary>
    [Supports("Maxim", "MAX7219")]
    public class Max7219 : LedDriver
    {
        private static int numDevices;

        private readonly int address;

        private readonly SpiDevice dev;

        private readonly byte[] state = new byte[8];

        private byte decodeMode;

        private byte scanLimit;

        private bool shutdown = true;

        private bool test = true;

        /// <summary>
        ///     Construct a new instance of a Max7219 device
        /// </summary>
        /// <param name="SpiModule">A reference to the Treehopper SPI module</param>
        /// <param name="LoadPin">The pin attached tot he LOAD input</param>
        /// <param name="Address">The index of the Max7219 device attached to this bus</param>
        /// <param name="SpeedMHz">The SPI speed to use. The maximum is 10 MHz.</param>
        public Max7219(Spi SpiModule, SpiChipSelectPin LoadPin, int Address = 0, double SpeedMHz = 6) : base(64, true,
            false)
        {
            if (SpeedMHz > 10)
                throw new ArgumentOutOfRangeException("SpeedMhz",
                    "The MAX7219 supports a maximum clock rate of 10 MHz.");

            dev = new SpiDevice(SpiModule, LoadPin, speedMhz: SpeedMHz, chipSelectMode: ChipSelectMode.PulseHighAtEnd);

            address = Address;
            sendTest(false);

            ScanLimit = 7;
            sendDecodeMode(0);
            Clear().Wait();
            Shutdown = false;
            SetGlobalBrightness(1);
        }

        /// <summary>
        ///     Gets or sets whether the display is in test mode or not.
        /// </summary>
        public bool Test
        {
            get { return test; }
            set
            {
                if (test == value) return;
                test = value;
                sendTest(test);
            }
        }

        /// <summary>
        ///     Gets or sets a bit pattern indicating whether each digit should be decoded on-chip or not.
        /// </summary>
        public byte DecodeMode
        {
            get { return decodeMode; }
            set
            {
                if (decodeMode == value) return;
                decodeMode = value;
                sendDecodeMode(decodeMode);
            }
        }

        /// <summary>
        ///     Gets or sets whether the display should be shut down or not
        /// </summary>
        public bool Shutdown
        {
            get { return shutdown; }
            set
            {
                if (shutdown == value) return;
                shutdown = value;
                Send(Opcode.Shutdown, (byte) (shutdown ? 0x00 : 0x01)).Wait();
            }
        }

        /// <summary>
        ///     Gets or sets the maximum digit index (from 0 to 7) that should be scanned
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

        /// <summary>
        ///     If true, LED segments are ordered G-A, DP. If False, this library will
        ///     use segment ordering of A-DP.
        /// </summary>
        /// <remarks>
        ///     All Treehopper LED driver libraries are, by default, standardized
        ///     so that the LSB of the driver corresponds with segment "A",
        ///     the 7th bit corresponds with "G" and the MSB corresponds to the "DP" segment.
        ///     This allows easy consumption of display libraries that require ordered collections
        ///     of segments.
        ///     However, you may choose to work with this library using the native LED ordering,
        ///     where the LSB corresponds with segment "G", the 7th bit corresponds to segment "A"
        ///     and the MSB corresponds to segment "DP".
        /// </remarks>
        public bool UseNativeLedOrder { get; set; } = false;

        private void sendTest(bool test)
        {
            Send(Opcode.DisplayTest, (byte) (test ? 0x01 : 0x00)).Wait();
        }

        private void sendDecodeMode(byte decodeMode)
        {
            Send(Opcode.DecodeMode, decodeMode).Wait();
        }

        private async Task Send(Opcode opcode, byte data)
        {
            if (numDevices < address + 1)
                numDevices = address + 1;
            //Create an array with the data to shift out
            var offset = address * 2;
            var maxbytes = numDevices * 2;

            var spiData = new byte[maxbytes];

            for (var i = 0; i < maxbytes; i++)
                spiData[i] = 0;

            //put our device data into the array
            spiData[offset + 1] = (byte) opcode;
            spiData[offset] = data;

            Array.Reverse(spiData);

            await dev.SendReceiveAsync(spiData, SpiBurstMode.BurstTx).ConfigureAwait(false);
        }

        internal override void LedStateChanged(Led led)
        {
            var digit = led.Channel / 8;
            var segment = led.Channel % 8;

            if (!UseNativeLedOrder)
                if (segment < 7)
                    segment = 6 - segment;

            if (led.State)
                state[digit] |= (byte) (1 << segment);
            else
                state[digit] &= (byte) ~(1 << segment);

            if (AutoFlush)
                Send((Opcode) (digit + 1), state[digit]).Wait();
        }

        internal override void LedBrightnessChanged(Led led)
        {
        }

        /// <summary>
        ///     Flush data to the display
        /// </summary>
        /// <param name="force">Whether to force all data to be sent, even if it appears unchanged</param>
        /// <returns>An awaitable task that completes when finished</returns>
        public override async Task FlushAsync(bool force = false)
        {
            if (AutoFlush && !force) return;

            for (var i = 0; i < 8; i++)
                await Send((Opcode) (i + 1), state[i]).ConfigureAwait(false);
        }

        internal override void SetGlobalBrightness(double brightness)
        {
            Send(Opcode.Intensity, (byte) (brightness * 15.0)).Wait();
        }

        private enum Opcode
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
}