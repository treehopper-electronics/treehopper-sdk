using System.Collections.Generic;
using System.Threading.Tasks;

namespace Treehopper.Libraries.IO.Adc
{
    /// <summary>
    ///     Texas Instruments ADS1115 16-bit 4-channel ADC
    /// </summary>
    [Supports("Texas Instruments", "ADS1115")]
    public partial class Ads1115 : IAdcPeripheral
    {
        /// <summary>
        ///     The channel modes of the ADS1115
        /// </summary>
        public enum ChannelMode
        {
            /// <summary>
            ///     Four independent single-ended ADC inputs
            /// </summary>
            SingleEnded,

            /// <summary>
            ///     Two differential inputs
            /// </summary>
            Differential
        }

        private readonly SMBusDevice dev;

        /// <summary>
        ///     Construct a new ADS1115
        /// </summary>
        /// <param name="i2c">The i2C port this ADC is attached to</param>
        /// <param name="channelMode">Whether this ADC is single-ended or differential</param>
        /// <param name="addr">The address pin of this module</param>
        /// <param name="refVoltage">The supply (reference) voltage of this module</param>
        /// <param name="speed">The speed to use when communicating with this module, in kHz</param>
        public Ads1115(I2C i2c, ChannelMode channelMode = ChannelMode.SingleEnded, bool addr = false,
            double refVoltage = 3.3, int speed = 400)
        {
            dev = new SMBusDevice((byte) (0x48 | (addr ? 1 : 0)), i2c, speed);
            Mode = channelMode;

            dev.WriteBufferData(0x01, new byte[] {0x01, 0x83}).Wait();

            for (var i = 0; i < (channelMode == ChannelMode.SingleEnded ? 4 : 2); i++)
                Pins.Add(new AdsPin(this));
        }

        /// <summary>
        ///     The channel mode this ADC is using
        /// </summary>
        public ChannelMode Mode { get; }

        /// <summary>
        ///     The collection of ADC pins
        /// </summary>
        public IList<AdsPin> Pins { get; set; } = new List<AdsPin>();

        /// <summary>
        ///     Whether this ADC should update whenever a pin is read
        /// </summary>
        public bool AutoUpdateWhenPropertyRead { get; set; } = true;

        public int AwaitPollingInterval { get; set; } = 25;

        /// <summary>
        ///     Read all channels of the ADC and update the values of the pins
        /// </summary>
        /// <returns></returns>
        public async Task UpdateAsync()
        {
            for (var i = 0; i < 4; i++)
            {
                var config = ((int) Pins[i].GainControl << 1) | 0x80;
                if (Mode == ChannelMode.SingleEnded)
                {
                    config |= (4 | i) << 4;
                }
                else
                {
                    if (i == 1)
                        config |= 3 << 4;
                }
                await dev.WriteBufferData(0x01, new byte[] {(byte) config, 0xE3});
                await Task.Delay(1);
                // data is stored in big-endian format
                Pins[i].AdcValue = await dev.ReadWordDataBE(0x00);
            }
        }


        /// <summary>
        ///     Represents an ADS1115 pin
        /// </summary>
        public class AdsPin : AdcPeripheralPin
        {
            /// <summary>
            ///     Gain settings
            /// </summary>
            public enum GainControlSetting
            {
                /// <summary>
                ///     6.144 V
                /// </summary>
                mV_6144,

                /// <summary>
                ///     4.096 V
                /// </summary>
                mV_4096,

                /// <summary>
                ///     2.048 V
                /// </summary>
                mV_2048,

                /// <summary>
                ///     1.024 V
                /// </summary>
                mV_1024,

                /// <summary>
                ///     0.512 V
                /// </summary>
                mV_512,

                /// <summary>
                ///     0.256 V
                /// </summary>
                mV_256
            }

            private GainControlSetting gainControl = GainControlSetting.mV_6144;

            internal AdsPin(IAdcPeripheral parent) : base(parent, 15, 6.144)
            {
            }

            /// <summary>
            ///     Gets or sets the gain used by this pin
            /// </summary>
            public GainControlSetting GainControl
            {
                get { return gainControl; }

                set
                {
                    gainControl = value;
                    switch (gainControl)
                    {
                        case GainControlSetting.mV_256:
                            ReferenceVoltage = 0.256;
                            break;

                        case GainControlSetting.mV_512:
                            ReferenceVoltage = 0.512;
                            break;

                        case GainControlSetting.mV_1024:
                            ReferenceVoltage = 1.024;
                            break;

                        case GainControlSetting.mV_2048:
                            ReferenceVoltage = 2.048;
                            break;

                        case GainControlSetting.mV_4096:
                            ReferenceVoltage = 4.096;
                            break;

                        case GainControlSetting.mV_6144:
                            ReferenceVoltage = 6.144;
                            break;
                    }
                }
            }
        }
    }
}