using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries
{
    public enum Registers
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
    };

    public enum IntegrationTime
    {
        Time_100ms,
        Time_200ms,
        Time_300ms,
        Time_400ms,
        Time_500ms,
        Time_600ms
    };

    public enum Gain
    {
        Low,
        Medium,
        High,
        Maximum
    };

    public class TSL2591 : SMBusDevice
    {
        const byte CommandBit = 0xA0;
        const byte ReadBit = 0x01;

        public TSL2591(I2c I2cModule) : base(0x29, I2cModule, 100)
        {
            byte retVal = AsyncHelpers.RunSync<byte>(() => ReadByteData(CommandBit | (byte)Registers.DeviceId));
            if(retVal != 0x50)
            {
                throw new Exception("TSL2591 not found on bus. Check your connections");
            }
        }
        private IntegrationTime integrationTime = IntegrationTime.Time_100ms;
        public IntegrationTime IntegrationTime
        {
            get
            {
                return integrationTime;
            }
            set
            {
                if (integrationTime == value)
                    return;
                integrationTime = value;
                AsyncHelpers.RunSync(() => sendConfig());
            }
        }

        private Gain gain = Gain.Low;
        public Gain Gain
        {
            get
            {
                return gain;
            }
            set
            {
                if (gain == value)
                    return;
                gain = value;
                AsyncHelpers.RunSync(() => sendConfig());
            }
        }

        private async Task sendConfig()
        {
            byte config = (byte)((byte)integrationTime | ((byte)gain << 4));
            await WriteByteData((byte)Registers.Control, config);
        }
    }
}
