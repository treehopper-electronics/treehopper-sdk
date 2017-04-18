using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Displays
{
    /// <summary>
    ///     Represents a single Seven-Segment digit, comprised of 7 LEDs, plus a decimal point LED
    /// </summary>
    public class SevenSegmentDigit
    {
        private static readonly byte[] CharTable = new byte[128]
        {
            //0x00  0x01  0x02  0x03  0x04  0x05  0x06  0x07  0x08  0x09  0x0A  0x0B  0x0C  0x0D  0x0E  0x0F
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // 0x00
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // 0x10
            0x00, 0x82, 0x21, 0x00, 0x00, 0x00, 0x00, 0x02, 0x39, 0x0F, 0x00, 0x00, 0x00, 0x40, 0x80, 0x00, // 0x20
            0x3F, 0x06, 0x5B, 0x4F, 0x66, 0x6D, 0x7D, 0x07, 0x7f, 0x6f, 0x00, 0x00, 0x00, 0x48, 0x00, 0x53, // 0x30
            0x00, 0x77, 0x7C, 0x39, 0x5E, 0x79, 0x71, 0x6F, 0x76, 0x06, 0x1E, 0x00, 0x38, 0x00, 0x54, 0x3F, // 0x40
            0x73, 0x67, 0x50, 0x6D, 0x78, 0x3E, 0x00, 0x00, 0x00, 0x6E, 0x00, 0x39, 0x00, 0x0F, 0x00, 0x08, // 0x50 
            0x63, 0x5F, 0x7C, 0x58, 0x5E, 0x7B, 0x71, 0x6F, 0x74, 0x02, 0x1E, 0x00, 0x06, 0x00, 0x54, 0x5C, // 0x60
            0x73, 0x67, 0x50, 0x6D, 0x78, 0x1C, 0x00, 0x00, 0x00, 0x6E, 0x00, 0x39, 0x30, 0x0F, 0x00, 0x00 // 0x70
        };

        private char _character;

        private bool _decimalPoint;

        /// <summary>
        ///     Construct a Seven-Segment digit with the specified list of LEDs
        /// </summary>
        /// <param name="leds">The LEDs to use for this digit</param>
        public SevenSegmentDigit(IList<Led> leds)
        {
            Leds = leds;
            foreach (var led in Leds)
            {
                // disable auto-flushing to increase speed
                led.Driver.AutoFlush = false;

                if (!Drivers.Contains(led.Driver))
                    Drivers.Add(led.Driver);
            }

            Character = ' ';
            Flush(true).Wait();
        }

        /// <summary>
        ///     The LEDs that comprise this digit
        /// </summary>
        public IList<Led> Leds { get; protected set; }

        internal Collection<ILedDriver> Drivers { get; set; } = new Collection<ILedDriver>();

        /// <summary>
        ///     Gets or sets the currently-displayed character
        /// </summary>
        public char Character
        {
            get { return _character; }
            set
            {
                if (_character == value) return;
                _character = value;
                var leds = CharTable[_character];
                for (var i = 0; i < 7; i++)
                    if (((leds >> i) & 0x01) == 1)
                        Leds[i].State = true;
                    else
                        Leds[i].State = false;

                if (AutoFlush)
                    Flush().Wait();
            }
        }

        /// <summary>
        ///     Gets or sets whether to automatically flush data to the board
        /// </summary>
        public bool AutoFlush { get; set; } = true;

        /// <summary>
        ///     Gets or sets whether the decimal point is illuminated
        /// </summary>
        public bool DecimalPoint
        {
            get { return _decimalPoint; }
            set
            {
                if (_decimalPoint == value) return;
                _decimalPoint = value;

                Leds[7].State = _decimalPoint;
            }
        }


        /// <summary>
        ///     Flush display data to the driver
        /// </summary>
        /// <param name="force">Force a full update, even if data doesn't appear to have changed.</param>
        public async Task Flush(bool force = false)
        {
            foreach (var driver in Drivers)
                await driver.Flush(force).ConfigureAwait(false);
        }
    }
}