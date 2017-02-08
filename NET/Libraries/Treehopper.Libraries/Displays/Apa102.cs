using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper.Libraries.Interface;
using Treehopper.Libraries.Interface.ShiftRegister;
using Treehopper.Utilities;

namespace Treehopper.Libraries.Displays
{
    /// <summary>
    /// APA102 Smart RGB LED library
    /// </summary>
    /// <remarks><para>
    /// The APA102
    /// </para>
    /// </remarks>
    public class Apa102 : IFlushable
    {
        private Spi spi;

        public IList<Led> Leds { get; private set; } = new List<Led>();

        public bool AutoFlush { get; set; } = true;

        public IFlushable Parent { get { return null; } }

        public double Brightness { get; set; } = 1.0;

        /// <summary>
        /// Construct a new APA102
        /// </summary>
        /// <param name="spi"></param>
        /// <param name="numLeds"></param>
        public Apa102(Spi spi, int numLeds)
        {
            this.spi = spi;
            spi.Enabled = true;
            for (int i = 0; i < numLeds; i++)
                Leds.Add(new Led(this));
        }
        
        /// <summary>
        /// Clear the display immediately, resetting all LEDs' values.
        /// </summary>
        /// <returns>An awaitable task</returns>
        public async Task Clear()
        {
            bool oldAutoFlush = AutoFlush;
            AutoFlush = false;
            Leds.ForEach(led => led.SetRgb(0, 0, 0));
            await Flush().ConfigureAwait(false);
            AutoFlush = oldAutoFlush;
        }

        /// <summary>
        /// Flush current LED values to the SPI bus to update the LEDs.
        /// </summary>
        /// <param name="force">Unused for this method</param>
        /// <returns>An awaitable task</returns>
        public async Task Flush(bool force = false)
        {
            var header = new byte[] { 0x00, 0x00, 0x00, 0x00};
            List<byte> bytes = new List<byte>();
            foreach (var led in Leds)
            {
                byte global = (byte)(0xE0 | (byte)Math.Round(Brightness * 31));

                bytes.Add(global);
                bytes.Add((byte)Math.Round(255.0 * Utility.BrightnessToCieLuminance(led.blue / 255.0)));
                bytes.Add((byte)Math.Round(255.0 * Utility.BrightnessToCieLuminance(led.green / 255.0)));
                bytes.Add((byte)Math.Round(255.0 * Utility.BrightnessToCieLuminance(led.red / 255.0)));
            }

            // still experimenting with this
            for (int i = 0; i < 5 + Leds.Count/15; i++)
            {
                bytes.Add(0x00);
            }

            var message = header.Concat(bytes).ToArray();

            int chunkCount = 0;
            while(true)
            {
                var chunk = message.Skip(chunkCount * 255).Take(255).ToArray();
                if (chunk.Length == 0)
                    break;

                await spi.SendReceive(chunk, null, ChipSelectMode.SpiActiveLow, 8, BurstMode.BurstTx, SpiMode.Mode11);
                chunkCount++;
                
            }
        }

        /// <summary>
        /// Represents a single APA102 LED's value
        /// </summary>
        public class Led : IRgbLed
        {
            private Apa102 driver;
            internal float red, green, blue;

            public float RedGain { get; set; }

            public float GreenGain { get; set; }

            public float BlueGain { get; set; }

            internal Led(Apa102 driver)
            {
                this.driver = driver;
            }

            public void SetRgb(float red, float green, float blue)
            {
                this.red = red;
                this.green = green;
                this.blue = blue;

                if(driver.AutoFlush)
                    driver.Flush().Wait();
            }

            public void SetHsl(float hue, float saturation, float luminance)
            {
                SetRgb(Color.FromHsl(hue, saturation, luminance));
            }

            public void SetRgb(Color color)
            {
                SetRgb(color.R, color.G, color.B);
            }
        }
    }
}
