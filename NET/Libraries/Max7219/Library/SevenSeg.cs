using System;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Max7219
{
    public class SevenSeg : Max7219
    {
        static readonly byte[] charTable = new byte[128] { 0x7E, 0x30, 0x6D, 0x79, 0x33, 0x5B, 0x5F, 0x70, 0x7F, 0x7B, 0x77, 0x1F, 0xD, 0x3D, 0x4F, 0x47, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x80, 0x1, 0x80, 0x0, 0x7E, 0x30, 0x6D, 0x79, 0x33, 0x5B, 0x5F, 0x70, 0x7F, 0x7B, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x77, 0x1F, 0xD, 0x3D, 0x4F, 0x47, 0x0, 0x37, 0x0, 0x0, 0x0, 0xE, 0x0, 0x0, 0x0, 0x67, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x8, 0x0, 0x77, 0x1F, 0xD, 0x3D, 0x4F, 0x47, 0x0, 0x37, 0x0, 0x0, 0x0, 0xE, 0x0, 0x0, 0x0, 0x67, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 };

        /// <summary>
        /// Constructs an 8-digit 7-segment display powered by a Max7219 SPI LED driver.
        /// </summary>
        /// <param name="module">The Treehopper SPI module that this display is attached to.</param>
        /// <param name="LoadPin">The Treehopper pin that this display's LOAD pin is attached to.</param>
        /// <param name="address">The zero-indexed address of the display.</param>
        /// <param name="speedMhz">The frequency to use to communicate with the display.</param>
        public SevenSeg(Spi module, Pin LoadPin, int address = 0, double speedMhz = 1) : base(module, LoadPin, address, speedMhz)
        {
            if (address < 0 || address > 8)
                throw new Exception("This library supports 1 to 8 displays.");
        }

        private string text;
        /// <summary>
        /// Gets or sets the currently-displayed text.
        /// </summary>
        public dynamic Text
        {
            get { return text; }
            set
            {
                if(!string.Equals(text, value.ToString()))
                {
                    text = value.ToString();
                    printString(text).ConfigureAwait(false);
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

        private byte[] oldData = new byte[8];
        private byte[] newData = new byte[8];
        private async Task printString(string text)
        {
            if(text.Length > 8)
                text = text.Substring(0, 8);

            newData = new byte[8];

            int k = 0;
            for(int i=0; i<text.Length; i++)
            {
                char c = text[i];

                // print decimal points as part of the previous character
                if (c == '.' && i != 0) continue;
                var v = charTable[c];
                // peak at the next character to look for a decimal point
                if (i+1 < text.Length)
                {
                    char next = text[i + 1];
                    if (next == '.')
                        v |= 0x80;
                }
                newData[k] = v;
                k++;
            }

            if(!leftAlignt)
            {
                var temp = new byte[8];
                int size = 0;

                for(int i=0;i<8;i++)
                {
                    if(newData[i] != 0)
                    {
                        size = i+1;
                        temp[i] = newData[i];
                    }
                }

                newData = new byte[8];

                for (int i=0;i<size;i++)
                {
                    newData[8-size+i] = temp[i];
                }
            }

            for(int i=0;i<8;i++)
            {
                if (newData[i] != oldData[i])
                    await Send((Opcode)(8-i), newData[i]);
            }

            newData.CopyTo(oldData, 0);

        }
    }
}
