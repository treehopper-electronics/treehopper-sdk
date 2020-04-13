/// This file was auto-generated by RegisterGenerator. Any changes to it will be overwritten!
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper;
using Treehopper.Libraries.Utilities;

namespace Treehopper.Libraries.Sensors.Optical
{
    public partial class Tsl2591
    {
        public enum AlsTimes
        {
            Time_100ms = 0,
            Time_200ms = 1,
            Time_300ms = 2,
            Time_400ms = 3,
            Time_500ms = 4,
            Time_600ms = 5
        }

        public enum AlsGains
        {
            Low = 0,
            Medium = 1,
            High = 2,
            Max = 3
        }

        public enum InterruptPersistanceFilters
        {
            EveryAlsCycle = 0,
            AnyValueOutsideThreshold = 1,
            Consecutive_2 = 2,
            Consecutive_3 = 3,
            Consecutive_5 = 4,
            Consecutive_10 = 5,
            Consecutive_15 = 6,
            Consecutive_20 = 7,
            Consecutive_25 = 8,
            Consecutive_30 = 9,
            Consecutive_35 = 10,
            Consecutive_40 = 11,
            Consecutive_45 = 12,
            Consecutive_50 = 13,
            Consecutive_55 = 14,
            Consecutive_60 = 15
        }

        protected class Tsl2591Registers : RegisterManager
        {
            internal Tsl2591Registers(IRegisterManagerAdapter adapter) : base(adapter, true)
            {
                enable = new EnableRegister(this);
                _registers.Add(enable);
                config = new ConfigRegister(this);
                _registers.Add(config);
                interruptLowThreshold = new InterruptLowThresholdRegister(this);
                _registers.Add(interruptLowThreshold);
                interruptHighThreshold = new InterruptHighThresholdRegister(this);
                _registers.Add(interruptHighThreshold);
                noPersistLowThreshold = new NoPersistLowThresholdRegister(this);
                _registers.Add(noPersistLowThreshold);
                noPersistHighThreshold = new NoPersistHighThresholdRegister(this);
                _registers.Add(noPersistHighThreshold);
                persist = new PersistRegister(this);
                _registers.Add(persist);
                packageId = new PackageIdRegister(this);
                _registers.Add(packageId);
                deviceId = new DeviceIdRegister(this);
                _registers.Add(deviceId);
                status = new StatusRegister(this);
                _registers.Add(status);
                ch0 = new Ch0Register(this);
                _registers.Add(ch0);
                ch1 = new Ch1Register(this);
                _registers.Add(ch1);
            }

            internal EnableRegister enable;
            internal ConfigRegister config;
            internal InterruptLowThresholdRegister interruptLowThreshold;
            internal InterruptHighThresholdRegister interruptHighThreshold;
            internal NoPersistLowThresholdRegister noPersistLowThreshold;
            internal NoPersistHighThresholdRegister noPersistHighThreshold;
            internal PersistRegister persist;
            internal PackageIdRegister packageId;
            internal DeviceIdRegister deviceId;
            internal StatusRegister status;
            internal Ch0Register ch0;
            internal Ch1Register ch1;

            internal class EnableRegister : Register
            {
                internal EnableRegister(RegisterManager regManager) : base(regManager, 0xA0, 1, false) { }

                public int powerOn { get; set; }
                public int alsEnable { get; set; }
                public int alsInterruptEnable { get; set; }
                public int sleepAfterInterrupt { get; set; }
                public int noPersistInterruptEnable { get; set; }

                public async Task<EnableRegister> read()
                {
                    await manager.read(this).ConfigureAwait(false);
                    return this;
                }
                internal override long getValue() { return ((powerOn & 0x1) << 0) | ((alsEnable & 0x1) << 1) | ((alsInterruptEnable & 0x1) << 4) | ((sleepAfterInterrupt & 0x1) << 6) | ((noPersistInterruptEnable & 0x1) << 7); }
                internal override void setValue(long _value)
                {
                    powerOn = (int)((_value >> 0) & 0x1);
                    alsEnable = (int)((_value >> 1) & 0x1);
                    alsInterruptEnable = (int)((_value >> 4) & 0x1);
                    sleepAfterInterrupt = (int)((_value >> 6) & 0x1);
                    noPersistInterruptEnable = (int)((_value >> 7) & 0x1);
                }

                public override string ToString()
                {
                    string retVal = "";
                    retVal += $"PowerOn: { powerOn } (offset: 0, width: 1)\r\n";
                    retVal += $"AlsEnable: { alsEnable } (offset: 1, width: 1)\r\n";
                    retVal += $"AlsInterruptEnable: { alsInterruptEnable } (offset: 4, width: 1)\r\n";
                    retVal += $"SleepAfterInterrupt: { sleepAfterInterrupt } (offset: 6, width: 1)\r\n";
                    retVal += $"NoPersistInterruptEnable: { noPersistInterruptEnable } (offset: 7, width: 1)\r\n";
                    return retVal;
                }
            }
            internal class ConfigRegister : Register
            {
                internal ConfigRegister(RegisterManager regManager) : base(regManager, 0xA1, 1, false) { }

                public int alsTime { get; set; }
                public int alsGain { get; set; }
                public int systemReset { get; set; }
                public AlsTimes getAlsTime() { return (AlsTimes)alsTime; }
                public void setAlsTime(AlsTimes enumVal) { alsTime = (int)enumVal; }
                public AlsGains getAlsGain() { return (AlsGains)alsGain; }
                public void setAlsGain(AlsGains enumVal) { alsGain = (int)enumVal; }

                public async Task<ConfigRegister> read()
                {
                    await manager.read(this).ConfigureAwait(false);
                    return this;
                }
                internal override long getValue() { return ((alsTime & 0x7) << 0) | ((alsGain & 0x3) << 3) | ((systemReset & 0x1) << 7); }
                internal override void setValue(long _value)
                {
                    alsTime = (int)((_value >> 0) & 0x7);
                    alsGain = (int)((_value >> 3) & 0x3);
                    systemReset = (int)((_value >> 7) & 0x1);
                }

                public override string ToString()
                {
                    string retVal = "";
                    retVal += $"AlsTime: { alsTime } (offset: 0, width: 3)\r\n";
                    retVal += $"AlsGain: { alsGain } (offset: 3, width: 2)\r\n";
                    retVal += $"SystemReset: { systemReset } (offset: 7, width: 1)\r\n";
                    return retVal;
                }
            }
            internal class InterruptLowThresholdRegister : Register
            {
                internal InterruptLowThresholdRegister(RegisterManager regManager) : base(regManager, 0xA4, 2, false) { }

                public int value { get; set; }

                public async Task<InterruptLowThresholdRegister> read()
                {
                    await manager.read(this).ConfigureAwait(false);
                    return this;
                }
                internal override long getValue() { return ((value & 0xFFFF) << 0); }
                internal override void setValue(long _value)
                {
                    value = (int)((_value >> 0) & 0xFFFF);
                }

                public override string ToString()
                {
                    string retVal = "";
                    retVal += $"Value: { value } (offset: 0, width: 16)\r\n";
                    return retVal;
                }
            }
            internal class InterruptHighThresholdRegister : Register
            {
                internal InterruptHighThresholdRegister(RegisterManager regManager) : base(regManager, 0xA6, 2, false) { }

                public int value { get; set; }

                public async Task<InterruptHighThresholdRegister> read()
                {
                    await manager.read(this).ConfigureAwait(false);
                    return this;
                }
                internal override long getValue() { return ((value & 0xFFFF) << 0); }
                internal override void setValue(long _value)
                {
                    value = (int)((_value >> 0) & 0xFFFF);
                }

                public override string ToString()
                {
                    string retVal = "";
                    retVal += $"Value: { value } (offset: 0, width: 16)\r\n";
                    return retVal;
                }
            }
            internal class NoPersistLowThresholdRegister : Register
            {
                internal NoPersistLowThresholdRegister(RegisterManager regManager) : base(regManager, 0xA8, 2, false) { }

                public int value { get; set; }

                public async Task<NoPersistLowThresholdRegister> read()
                {
                    await manager.read(this).ConfigureAwait(false);
                    return this;
                }
                internal override long getValue() { return ((value & 0xFFFF) << 0); }
                internal override void setValue(long _value)
                {
                    value = (int)((_value >> 0) & 0xFFFF);
                }

                public override string ToString()
                {
                    string retVal = "";
                    retVal += $"Value: { value } (offset: 0, width: 16)\r\n";
                    return retVal;
                }
            }
            internal class NoPersistHighThresholdRegister : Register
            {
                internal NoPersistHighThresholdRegister(RegisterManager regManager) : base(regManager, 0xAa, 2, false) { }

                public int value { get; set; }

                public async Task<NoPersistHighThresholdRegister> read()
                {
                    await manager.read(this).ConfigureAwait(false);
                    return this;
                }
                internal override long getValue() { return ((value & 0xFFFF) << 0); }
                internal override void setValue(long _value)
                {
                    value = (int)((_value >> 0) & 0xFFFF);
                }

                public override string ToString()
                {
                    string retVal = "";
                    retVal += $"Value: { value } (offset: 0, width: 16)\r\n";
                    return retVal;
                }
            }
            internal class PersistRegister : Register
            {
                internal PersistRegister(RegisterManager regManager) : base(regManager, 0xAc, 1, false) { }

                public int interruptPersistanceFilter { get; set; }
                public InterruptPersistanceFilters getInterruptPersistanceFilter() { return (InterruptPersistanceFilters)interruptPersistanceFilter; }
                public void setInterruptPersistanceFilter(InterruptPersistanceFilters enumVal) { interruptPersistanceFilter = (int)enumVal; }

                public async Task<PersistRegister> read()
                {
                    await manager.read(this).ConfigureAwait(false);
                    return this;
                }
                internal override long getValue() { return ((interruptPersistanceFilter & 0xF) << 0); }
                internal override void setValue(long _value)
                {
                    interruptPersistanceFilter = (int)((_value >> 0) & 0xF);
                }

                public override string ToString()
                {
                    string retVal = "";
                    retVal += $"InterruptPersistanceFilter: { interruptPersistanceFilter } (offset: 0, width: 4)\r\n";
                    return retVal;
                }
            }
            internal class PackageIdRegister : Register
            {
                internal PackageIdRegister(RegisterManager regManager) : base(regManager, 0xB1, 1, false) { }

                public int value { get; set; }

                public async Task<PackageIdRegister> read()
                {
                    await manager.read(this).ConfigureAwait(false);
                    return this;
                }
                internal override long getValue() { return ((value & 0xFF) << 0); }
                internal override void setValue(long _value)
                {
                    value = (int)((_value >> 0) & 0xFF);
                }

                public override string ToString()
                {
                    string retVal = "";
                    retVal += $"Value: { value } (offset: 0, width: 8)\r\n";
                    return retVal;
                }
            }
            internal class DeviceIdRegister : Register
            {
                internal DeviceIdRegister(RegisterManager regManager) : base(regManager, 0xB2, 1, false) { }

                public int value { get; set; }

                public async Task<DeviceIdRegister> read()
                {
                    await manager.read(this).ConfigureAwait(false);
                    return this;
                }
                internal override long getValue() { return ((value & 0xFF) << 0); }
                internal override void setValue(long _value)
                {
                    value = (int)((_value >> 0) & 0xFF);
                }

                public override string ToString()
                {
                    string retVal = "";
                    retVal += $"Value: { value } (offset: 0, width: 8)\r\n";
                    return retVal;
                }
            }
            internal class StatusRegister : Register
            {
                internal StatusRegister(RegisterManager regManager) : base(regManager, 0xB3, 1, false) { }

                public int alsValud { get; set; }
                public int alsInterrupt { get; set; }
                public int noPersistInterrupt { get; set; }

                public async Task<StatusRegister> read()
                {
                    await manager.read(this).ConfigureAwait(false);
                    return this;
                }
                internal override long getValue() { return ((alsValud & 0x1) << 0) | ((alsInterrupt & 0x1) << 4) | ((noPersistInterrupt & 0x1) << 5); }
                internal override void setValue(long _value)
                {
                    alsValud = (int)((_value >> 0) & 0x1);
                    alsInterrupt = (int)((_value >> 4) & 0x1);
                    noPersistInterrupt = (int)((_value >> 5) & 0x1);
                }

                public override string ToString()
                {
                    string retVal = "";
                    retVal += $"AlsValud: { alsValud } (offset: 0, width: 1)\r\n";
                    retVal += $"AlsInterrupt: { alsInterrupt } (offset: 4, width: 1)\r\n";
                    retVal += $"NoPersistInterrupt: { noPersistInterrupt } (offset: 5, width: 1)\r\n";
                    return retVal;
                }
            }
            internal class Ch0Register : Register
            {
                internal Ch0Register(RegisterManager regManager) : base(regManager, 0xB4, 2, false) { }

                public int value { get; set; }

                public async Task<Ch0Register> read()
                {
                    await manager.read(this).ConfigureAwait(false);
                    return this;
                }
                internal override long getValue() { return ((value & 0xFFFF) << 0); }
                internal override void setValue(long _value)
                {
                    value = (int)((_value >> 0) & 0xFFFF);
                }

                public override string ToString()
                {
                    string retVal = "";
                    retVal += $"Value: { value } (offset: 0, width: 16)\r\n";
                    return retVal;
                }
            }
            internal class Ch1Register : Register
            {
                internal Ch1Register(RegisterManager regManager) : base(regManager, 0xB6, 2, false) { }

                public int value { get; set; }

                public async Task<Ch1Register> read()
                {
                    await manager.read(this).ConfigureAwait(false);
                    return this;
                }
                internal override long getValue() { return ((value & 0xFFFF) << 0); }
                internal override void setValue(long _value)
                {
                    value = (int)((_value >> 0) & 0xFFFF);
                }

                public override string ToString()
                {
                    string retVal = "";
                    retVal += $"Value: { value } (offset: 0, width: 16)\r\n";
                    return retVal;
                }
            }
        }
    }
}