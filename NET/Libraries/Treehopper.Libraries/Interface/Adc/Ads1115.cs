using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Interface.Adc
{
    public class Ads1115 : IAdcPeripheral
    {
        private SMBusDevice dev;

        public Ads1115(I2c i2c, ChannelMode channelMode = ChannelMode.SingleEnded, bool addr = false, double refVoltage = 3.3, int speed = 400)
        {
            dev = new SMBusDevice((byte)(0x48 | (addr ? 1 : 0)), i2c, speed);
            Mode = channelMode;

            dev.WriteBufferData(0x01, new byte[] { 0x01, 0x83 }).Wait();

            for (int i = 0; i < (channelMode == ChannelMode.SingleEnded ? 4 : 2); i++)
                Pins.Add(new AdsPin(this));
        }

        public bool AutoUpdateWhenPropertyRead { get; set; }

        public ChannelMode Mode { get; private set; }

        public IList<AdsPin> Pins { get; set; } = new List<AdsPin>();

        public async Task Update()
        {
            for (int i = 0; i < 4; i++)
            {
                int config = ((int)Pins[i].GainControl << 1) | 0x80;
                if (Mode == ChannelMode.SingleEnded)
                {
                    config |= (4 | i) << 4;
                }
                else
                {
                    if (i == 1)
                        config |= 3 << 4;
                }
                await dev.WriteBufferData(0x01, new byte[] { (byte)config, 0xE3 });
                await Task.Delay(1);
                // data is stored in big-endian format
                Pins[i].AdcValue = await dev.ReadWordDataBE(0x00);
            }
        }

        public enum ChannelMode
        {
            SingleEnded,
            Differential
        }
    }
}
