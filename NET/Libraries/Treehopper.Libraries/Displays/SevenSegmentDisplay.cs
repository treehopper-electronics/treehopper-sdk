using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Linq;

namespace Treehopper.Libraries.Displays
{
    public class SevenSegmentDisplay : CharacterDisplay
    {
        public SevenSegmentDisplay(IList<Led> Leds, bool flipDigits = false) : base(Leds.Count/8, 1)
        {
            if (Leds.Count % 8 != 0)
                throw new ArgumentException("Leds should contain a multiple of 8 segments when using this constructor", "leds");

            int numDigits = Leds.Count / 8;

            digits = new Collection<SevenSegmentDigit>();

            for(int i=0;i<numDigits;i++)
            {
                var leds = Leds.Skip(i * 8).Take(8);
                var digit = new SevenSegmentDigit(leds);
                digits.Add(digit);
            }

            if (flipDigits)
                digits = digits.Reverse().ToList();

            setupDigits();
        }
        public SevenSegmentDisplay(IList<SevenSegmentDigit> Digits) : base(Digits.Count, 1)
        {
            this.digits = Digits;

            setupDigits();
        }

        void setupDigits()
        {
            foreach (var digit in digits)
            {
                // disable per-character auto-flushing for performance
                digit.AutoFlush = false;

                foreach (var driver in digit.Drivers)
                {
                    if (!drivers.Contains(driver))
                        drivers.Add(driver);
                }
            }
        }

        Collection<ILedDriver> drivers = new Collection<ILedDriver>();

        private string text;
        /// <summary>
        /// Gets or sets the currently-displayed text.
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

        private bool leftAlignt = false;
        /// <summary>
        /// Controls whether the display should be left-aligned or not.
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

        private IList<SevenSegmentDigit> digits;

        private async Task printString(string text)
        {
            if(text.Length > digits.Count)
                text = text.Substring(0, digits.Count);

            int k = 0;
            for(int i=0; i<text.Length; i++)
            {
                char c = text[i];

                // print decimal points as part of the previous character
                if (c == '.' && i != 0) continue;
                digits[k].Character = c;
                // peak at the next character to look for a decimal point
                if (i+1 < text.Length)
                {
                    char next = text[i + 1];
                    if (next == '.')
                        digits[k].DecimalPoint = true;
                }
                k++;
            }

            foreach(var driver in drivers)
            {
                await driver.Flush().ConfigureAwait(false);
            }                
        }

        protected override async Task clear()
        {
            
        }

        protected override async Task updateCursorPosition()
        {

        }

        protected override Task write(dynamic value)
        {
            throw new NotImplementedException();
        }
    }
}
