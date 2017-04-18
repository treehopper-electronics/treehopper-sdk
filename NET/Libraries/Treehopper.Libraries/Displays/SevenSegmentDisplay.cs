using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Displays
{
    /// <summary>
    ///     A display comprised of one or more SevenSegment characters
    /// </summary>
    public class SevenSegmentDisplay : CharacterDisplay
    {
        private readonly IList<SevenSegmentDigit> digits;

        private readonly Collection<ILedDriver> drivers = new Collection<ILedDriver>();

        private bool leftAlignt;

        private string text;

        /// <summary>
        ///     Construct a SevenSegment display from a list of LEDs.
        /// </summary>
        /// <param name="Leds">The list of LEDs to use</param>
        /// <param name="rightToLeftDigits">Whether the digits are right-to-left digits</param>
        public SevenSegmentDisplay(IList<Led> Leds, bool rightToLeftDigits = false) : base(Leds.Count / 8, 1)
        {
            if (Leds.Count % 8 != 0)
                throw new ArgumentException("Leds should contain a multiple of 8 segments when using this constructor",
                    "leds");

            var numDigits = Leds.Count / 8;

            digits = new Collection<SevenSegmentDigit>();

            for (var i = 0; i < numDigits; i++)
            {
                var leds = Leds.Skip(i * 8).Take(8).ToList();
                var digit = new SevenSegmentDigit(leds);
                digits.Add(digit);
            }

            if (rightToLeftDigits)
                digits = digits.Reverse().ToList();

            setupDigits();
        }

        /// <summary>
        ///     Construct a SevenSegmentDisplay from a collection of already-created digits
        /// </summary>
        /// <param name="Digits"></param>
        public SevenSegmentDisplay(IList<SevenSegmentDigit> Digits) : base(Digits.Count, 1)
        {
            digits = Digits;

            setupDigits();
        }

        /// <summary>
        ///     Gets or sets the currently-displayed text.
        /// </summary>
        public dynamic Text
        {
            get { return text; }
            set
            {
                if (!string.Equals(text, value.ToString()))
                {
                    text = value.ToString();
                    printString(text).Wait();
                }
            }
        }

        /// <summary>
        ///     Controls whether the display should be left-aligned or not.
        /// </summary>
        public bool LeftAlignt
        {
            get { return leftAlignt; }
            set
            {
                if (leftAlignt == value) return;
                leftAlignt = value;
            }
        }

        private void setupDigits()
        {
            foreach (var digit in digits)
            {
                // disable per-character auto-flushing for performance
                digit.AutoFlush = false;

                foreach (var driver in digit.Drivers)
                    if (!drivers.Contains(driver))
                        drivers.Add(driver);
            }
        }

        private async Task printString(string text)
        {
            if (text.Length > digits.Count)
                text = text.Substring(0, digits.Count);

            var k = 0;
            for (var i = 0; i < text.Length; i++)
            {
                var c = text[i];

                // print decimal points as part of the previous character
                if (c == '.' && i != 0) continue;
                digits[k].Character = c;
                // peak at the next character to look for a decimal point
                if (i + 1 < text.Length)
                {
                    var next = text[i + 1];
                    if (next == '.')
                        digits[k].DecimalPoint = true;
                }
                k++;
            }

            foreach (var driver in drivers)
                await driver.Flush().ConfigureAwait(false);
        }

        /// <summary>
        ///     clear the display
        /// </summary>
        /// <returns>An awaitable task that completes when finished</returns>
        protected override async Task clear()
        {
        }

        /// <summary>
        ///     update the cursor position
        /// </summary>
        /// <returns>An awaitable task that completes when finished</returns>
        protected override async Task updateCursorPosition()
        {
        }

        /// <summary>
        ///     write a value to the current position of the display
        /// </summary>
        /// <param name="value">the value to write</param>
        /// <returns>An awaitable task that completes when finished</returns>
        protected override Task write(dynamic value)
        {
            throw new NotImplementedException();
        }
    }
}