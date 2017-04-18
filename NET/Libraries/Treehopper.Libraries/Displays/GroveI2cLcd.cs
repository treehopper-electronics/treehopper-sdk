using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper;
using Treehopper.Libraries.Interface.PortExpander;

namespace Treehopper.Libraries.Displays
{
    /// <summary>
    /// Seeed Studio Grove I2c LCD with RGB backlight
    /// </summary>
    public class GroveI2cLcd : Hd44780
    {
        Pca9632 backlight;

        public GroveI2cLcd(I2c i2c) : base(new I2cParallelInterface(i2c), 16, 2)
        {
            // It's undocumented, but the Grove I2C LCD uses a PCA9632 for the RGB backlight
            backlight = new Pca9632(i2c);
        }

        public Task SetBacklight(Color color)
        {
            return SetBacklight(color.R, color.G, color.B);
        }

        public Task SetBacklight(int red, int green, int blue)
        {
            byte redFixed = (byte)Math.Round(Utility.BrightnessToCieLuminance(red / 255d) * 255);
            byte greenFixed = (byte)Math.Round(Utility.BrightnessToCieLuminance(green / 255d) * 255);
            byte blueFixed = (byte)Math.Round(Utility.BrightnessToCieLuminance(blue / 255d) * 255);
            return backlight.SetOutputs(new byte[] { (byte)blueFixed, (byte)greenFixed, (byte)redFixed, 0x00 });
        }

        class I2cParallelInterface : WriteOnlyParallelInterface
        {
            private SMBusDevice dev;

            public I2cParallelInterface(I2c i2c)
            {
                this.dev = new SMBusDevice(0x3e, i2c);
            }

            public int DelayMicroseconds { get; set; }

            public bool Enabled { get; set; }

            public int Width => 8;

            public Task WriteCommand(uint[] command)
            {
                var bytes = new byte[command.Length];

                for (int i = 0; i < command.Length; i++)
                    bytes[i] = (byte)command[i];

                return dev.WriteBufferData(0x80, bytes);
            }

            public Task WriteData(uint[] data)
            {
                var bytes = new byte[data.Length];

                for (int i = 0; i < data.Length; i++)
                    bytes[i] = (byte)data[i];

                return dev.WriteBufferData(0x40, bytes);
            }
        }
    }
}
