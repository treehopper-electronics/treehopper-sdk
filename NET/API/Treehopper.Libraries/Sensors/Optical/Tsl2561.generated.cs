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
    public partial class Tsl2561
    {
        internal enum Powers
        {
            powerDown = 0,
            powerUp = 3
        }

        public enum IntegrationTimings
        {
            Time_13_7ms = 0,
            Time_101ms = 1,
            Time_402ms = 2,
            Time_Manual = 3
        }

        public enum IntrControlSelects
        {
            InterruptOutputDisabled = 0,
            LevelInterrupt = 1,
            SMBAlertCompliant = 2,
            TestMode = 3
        }

        protected class Tsl2561Registers : RegisterManager
        {
            internal Tsl2561Registers(IRegisterManagerAdapter adapter) : base(adapter, true)
            {
                control = new ControlRegister(this);
                _registers.Add(control);
                timing = new TimingRegister(this);
                _registers.Add(timing);
                interruptThresholdLow = new InterruptThresholdLowRegister(this);
                _registers.Add(interruptThresholdLow);
                interruptThresholdHigh = new InterruptThresholdHighRegister(this);
                _registers.Add(interruptThresholdHigh);
                interruptControl = new InterruptControlRegister(this);
                _registers.Add(interruptControl);
                id = new IdRegister(this);
                _registers.Add(id);
                data0 = new Data0Register(this);
                _registers.Add(data0);
                data1 = new Data1Register(this);
                _registers.Add(data1);
            }

            internal ControlRegister control;
            internal TimingRegister timing;
            internal InterruptThresholdLowRegister interruptThresholdLow;
            internal InterruptThresholdHighRegister interruptThresholdHigh;
            internal InterruptControlRegister interruptControl;
            internal IdRegister id;
            internal Data0Register data0;
            internal Data1Register data1;

            internal class ControlRegister : Register
            {
                internal ControlRegister(RegisterManager regManager) : base(regManager, 0x80, 1, false) { }

                public int power { get; set; }
                public Powers getPower() { return (Powers)power; }
                public void setPower(Powers enumVal) { power = (int)enumVal; }

                internal override long getValue() { return ((power & 0x3) << 0); }
                internal override void setValue(long _value)
                {
                    power = (int)((_value >> 0) & 0x3);
                }

                public override string ToString()
                {
                    string retVal = "";
                    retVal += $"Power: { power } (offset: 0, width: 2)\r\n";
                    return retVal;
                }
            }
            internal class TimingRegister : Register
            {
                internal TimingRegister(RegisterManager regManager) : base(regManager, 0x81, 1, false) { }

                public int integrationTiming { get; set; }
                public int manual { get; set; }
                public int gain { get; set; }
                public IntegrationTimings getIntegrationTiming() { return (IntegrationTimings)integrationTiming; }
                public void setIntegrationTiming(IntegrationTimings enumVal) { integrationTiming = (int)enumVal; }

                internal override long getValue() { return ((integrationTiming & 0x3) << 0) | ((manual & 0x1) << 3) | ((gain & 0x1) << 4); }
                internal override void setValue(long _value)
                {
                    integrationTiming = (int)((_value >> 0) & 0x3);
                    manual = (int)((_value >> 3) & 0x1);
                    gain = (int)((_value >> 4) & 0x1);
                }

                public override string ToString()
                {
                    string retVal = "";
                    retVal += $"IntegrationTiming: { integrationTiming } (offset: 0, width: 2)\r\n";
                    retVal += $"Manual: { manual } (offset: 3, width: 1)\r\n";
                    retVal += $"Gain: { gain } (offset: 4, width: 1)\r\n";
                    return retVal;
                }
            }
            internal class InterruptThresholdLowRegister : Register
            {
                internal InterruptThresholdLowRegister(RegisterManager regManager) : base(regManager, 0x82, 2, false) { }

                public int value { get; set; }

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
            internal class InterruptThresholdHighRegister : Register
            {
                internal InterruptThresholdHighRegister(RegisterManager regManager) : base(regManager, 0x84, 2, false) { }

                public int value { get; set; }

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
            internal class InterruptControlRegister : Register
            {
                internal InterruptControlRegister(RegisterManager regManager) : base(regManager, 0x86, 1, false) { }

                public int persist { get; set; }
                public int intrControlSelect { get; set; }
                public IntrControlSelects getIntrControlSelect() { return (IntrControlSelects)intrControlSelect; }
                public void setIntrControlSelect(IntrControlSelects enumVal) { intrControlSelect = (int)enumVal; }

                internal override long getValue() { return ((persist & 0xF) << 0) | ((intrControlSelect & 0x3) << 4); }
                internal override void setValue(long _value)
                {
                    persist = (int)((_value >> 0) & 0xF);
                    intrControlSelect = (int)((_value >> 4) & 0x3);
                }

                public override string ToString()
                {
                    string retVal = "";
                    retVal += $"Persist: { persist } (offset: 0, width: 4)\r\n";
                    retVal += $"IntrControlSelect: { intrControlSelect } (offset: 4, width: 2)\r\n";
                    return retVal;
                }
            }
            internal class IdRegister : Register
            {
                internal IdRegister(RegisterManager regManager) : base(regManager, 0x8A, 1, false) { }

                public int revNumber { get; set; }
                public int partNumber { get; set; }

                internal override long getValue() { return ((revNumber & 0xF) << 0) | ((partNumber & 0xF) << 4); }
                internal override void setValue(long _value)
                {
                    revNumber = (int)((_value >> 0) & 0xF);
                    partNumber = (int)((_value >> 4) & 0xF);
                }

                public override string ToString()
                {
                    string retVal = "";
                    retVal += $"RevNumber: { revNumber } (offset: 0, width: 4)\r\n";
                    retVal += $"PartNumber: { partNumber } (offset: 4, width: 4)\r\n";
                    return retVal;
                }
            }
            internal class Data0Register : Register
            {
                internal Data0Register(RegisterManager regManager) : base(regManager, 0x8C, 2, false) { }

                public int value { get; set; }

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
            internal class Data1Register : Register
            {
                internal Data1Register(RegisterManager regManager) : base(regManager, 0x8E, 2, false) { }

                public int value { get; set; }

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