using System;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Sensors.Optical
{
    /// <summary>
    ///     ams TSL2591 High-dynamic range digital light sensor
    /// </summary>
    public class Tsl2591 : SMBusDevice
    {
        /// <summary>
        ///     Gain settings
        /// </summary>
        public enum Gain
        {
            /// <summary>
            ///     Low
            /// </summary>
            Low,

            /// <summary>
            ///     Medium
            /// </summary>
            Medium,

            /// <summary>
            ///     High
            /// </summary>
            High,

            /// <summary>
            ///     Maximum
            /// </summary>
            Maximum
        }

        /// <summary>
        ///     Integration times
        /// </summary>
        public enum IntegrationTime
        {
            /// <summary>
            ///     100 ms
            /// </summary>
            Time_100ms,

            /// <summary>
            ///     200 ms
            /// </summary>
            Time_200ms,

            /// <summary>
            ///     300 ms
            /// </summary>
            Time_300ms,

            /// <summary>
            ///     400 ms
            /// </summary>
            Time_400ms,

            /// <summary>
            ///     500 ms
            /// </summary>
            Time_500ms,

            /// <summary>
            ///     600 ms
            /// </summary>
            Time_600ms
        }

        private const byte CommandBit = 0xA0;
        private const byte ReadBit = 0x01;

        private Gain gain = Gain.Low;
        private IntegrationTime integrationTime = IntegrationTime.Time_100ms;


        /// <summary>
        ///     Construct a TSL2591 ambient light sensor
        /// </summary>
        /// <param name="I2cModule">The I2c module this sensor is attached to</param>
        public Tsl2591(I2c I2cModule) : base(0x29, I2cModule, 100)
        {
            var t = ReadByteData(CommandBit | (byte) Registers.DeviceId);
            t.Wait();
            var retVal = t.Result;
            if (retVal != 0x50)
                throw new Exception("TSL2591 not found on bus. Check your connections");
        }

        /// <summary>
        ///     Get or set the integration time of this sensor
        /// </summary>
        public IntegrationTime IntegrationTimeSetting
        {
            get { return integrationTime; }
            set
            {
                if (integrationTime == value)
                    return;
                integrationTime = value;
                sendConfig().Wait();
            }
        }

        /// <summary>
        ///     Get or set the gain setting
        /// </summary>
        public Gain GainSetting
        {
            get { return gain; }
            set
            {
                if (gain == value)
                    return;
                gain = value;
                sendConfig().Wait();
            }
        }

        private async Task sendConfig()
        {
            var config = (byte) ((byte) integrationTime | ((byte) gain << 4));
            await WriteByteData((byte) Registers.Control, config);
        }

        private enum Registers
        {
            Enable = 0x00,
            Control = 0x01,
            InterruptLowThresholdLowByte = 0x04,
            InterruptLowThresholdHighByte = 0x05,
            InterruptHighThresholdLowByte = 0x06,
            InterruptHighThresholdHighByte = 0x07,
            NoPersistInterruptLowThresholdLowByte = 0x08,
            NoPersistInterruptLowThresholdHighByte = 0x09,
            NoPersistInterruptHighThresholdLowByte = 0x0A,
            NoPersistInterruptHighThresholdHighByte = 0x0B,
            InterruptPersistenceFilter = 0x0C,
            PackageId = 0x11,
            DeviceId = 0x12,
            DeviceStatus = 0x13,
            Ch0LowByte = 0x14,
            Ch0HighByte = 0x15,
            Ch1LowByte = 0x16,
            Ch1HighByte = 0x17
        }
    }
}