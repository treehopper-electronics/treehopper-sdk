/// This file was auto-generated by RegisterGenerator. Any changes to it will be overwritten!
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper;
using Treehopper.Libraries.Utilities;

namespace Treehopper.Libraries.Sensors.Magnetic
{
    public partial class Lsm303dlhcMag
    {
        internal enum MagDataRates
        {
            Hz_0_75 = 0,
            Hz_1_5 = 1,
            Hz_3_0 = 2,
            Hz_7_5 = 3,
            Hz_15 = 4,
            Hz_30 = 5,
            Hz_75 = 6,
            Hz_100 = 7
        }

        internal enum GainConfigurations
        {
            gauss_1_3 = 1,
            gauss_1_9 = 2,
            gauss_2_5 = 3,
            gauss_4_0 = 4,
            gauss_4_7 = 5,
            gauss_5_6 = 6,
            gauss_8_1 = 7
        }

        internal enum MagSensorModes
        {
            ContinuousConversion = 0,
            SingleConversion = 1,
            PowerDown = 2
        }

        protected class Lsm303dlhcMagRegisters : RegisterManager
        {
            internal Lsm303dlhcMagRegisters(IRegisterManagerAdapter adapter) : base(adapter, true)
            {
                tempOut = new TempOutRegister(this);
                _registers.Add(tempOut);
                sr = new SrRegister(this);
                _registers.Add(sr);
                cra = new CraRegister(this);
                _registers.Add(cra);
                crb = new CrbRegister(this);
                _registers.Add(crb);
                mr = new MrRegister(this);
                _registers.Add(mr);
                outX = new OutXRegister(this);
                _registers.Add(outX);
                outY = new OutYRegister(this);
                _registers.Add(outY);
                outZ = new OutZRegister(this);
                _registers.Add(outZ);
            }

            internal TempOutRegister tempOut;
            internal SrRegister sr;
            internal CraRegister cra;
            internal CrbRegister crb;
            internal MrRegister mr;
            internal OutXRegister outX;
            internal OutYRegister outY;
            internal OutZRegister outZ;

            internal class TempOutRegister : Register
            {
                internal TempOutRegister(RegisterManager regManager) : base(regManager, 0x05, 2, false) { }

                public int value { get; set; }

                internal override long getValue() { return ((value & 0xFFF) << 4); }
                internal override void setValue(long _value)
                {
                    value = (int)(((_value >> 4) & 0xFFF) << (32 - 12)) >> (32 - 12);
                }

                public override string ToString()
                {
                    string retVal = "";
                    retVal += $"Value: { value } (offset: 4, width: 12)\r\n";
                    return retVal;
                }
            }
            internal class SrRegister : Register
            {
                internal SrRegister(RegisterManager regManager) : base(regManager, 0x09, 1, false) { }

                public int drdy { get; set; }
                public int registerLock { get; set; }

                internal override long getValue() { return ((drdy & 0x1) << 0) | ((registerLock & 0x1) << 1); }
                internal override void setValue(long _value)
                {
                    drdy = (int)((_value >> 0) & 0x1);
                    registerLock = (int)((_value >> 1) & 0x1);
                }

                public override string ToString()
                {
                    string retVal = "";
                    retVal += $"Drdy: { drdy } (offset: 0, width: 1)\r\n";
                    retVal += $"RegisterLock: { registerLock } (offset: 1, width: 1)\r\n";
                    return retVal;
                }
            }
            internal class CraRegister : Register
            {
                internal CraRegister(RegisterManager regManager) : base(regManager, 0x80, 1, false) { }

                public int magDataRate { get; set; }
                public int tempEnable { get; set; }
                public MagDataRates getMagDataRate() { return (MagDataRates)magDataRate; }
                public void setMagDataRate(MagDataRates enumVal) { magDataRate = (int)enumVal; }

                internal override long getValue() { return ((magDataRate & 0x7) << 2) | ((tempEnable & 0x1) << 7); }
                internal override void setValue(long _value)
                {
                    magDataRate = (int)((_value >> 2) & 0x7);
                    tempEnable = (int)((_value >> 7) & 0x1);
                }

                public override string ToString()
                {
                    string retVal = "";
                    retVal += $"MagDataRate: { magDataRate } (offset: 2, width: 3)\r\n";
                    retVal += $"TempEnable: { tempEnable } (offset: 7, width: 1)\r\n";
                    return retVal;
                }
            }
            internal class CrbRegister : Register
            {
                internal CrbRegister(RegisterManager regManager) : base(regManager, 0x81, 1, false) { }

                public int gainConfiguration { get; set; }
                public GainConfigurations getGainConfiguration() { return (GainConfigurations)gainConfiguration; }
                public void setGainConfiguration(GainConfigurations enumVal) { gainConfiguration = (int)enumVal; }

                internal override long getValue() { return ((gainConfiguration & 0x7) << 5); }
                internal override void setValue(long _value)
                {
                    gainConfiguration = (int)((_value >> 5) & 0x7);
                }

                public override string ToString()
                {
                    string retVal = "";
                    retVal += $"GainConfiguration: { gainConfiguration } (offset: 5, width: 3)\r\n";
                    return retVal;
                }
            }
            internal class MrRegister : Register
            {
                internal MrRegister(RegisterManager regManager) : base(regManager, 0x82, 1, false) { }

                public int magSensorMode { get; set; }
                public MagSensorModes getMagSensorMode() { return (MagSensorModes)magSensorMode; }
                public void setMagSensorMode(MagSensorModes enumVal) { magSensorMode = (int)enumVal; }

                internal override long getValue() { return ((magSensorMode & 0x3) << 0); }
                internal override void setValue(long _value)
                {
                    magSensorMode = (int)((_value >> 0) & 0x3);
                }

                public override string ToString()
                {
                    string retVal = "";
                    retVal += $"MagSensorMode: { magSensorMode } (offset: 0, width: 2)\r\n";
                    return retVal;
                }
            }
            internal class OutXRegister : Register
            {
                internal OutXRegister(RegisterManager regManager) : base(regManager, 0x83, 2, false) { }

                public int value { get; set; }

                internal override long getValue() { return ((value & 0xFFFF) << 0); }
                internal override void setValue(long _value)
                {
                    value = (int)(((_value >> 0) & 0xFFFF) << (32 - 16)) >> (32 - 16);
                }

                public override string ToString()
                {
                    string retVal = "";
                    retVal += $"Value: { value } (offset: 0, width: 16)\r\n";
                    return retVal;
                }
            }
            internal class OutYRegister : Register
            {
                internal OutYRegister(RegisterManager regManager) : base(regManager, 0x85, 2, false) { }

                public int value { get; set; }

                internal override long getValue() { return ((value & 0xFFFF) << 0); }
                internal override void setValue(long _value)
                {
                    value = (int)(((_value >> 0) & 0xFFFF) << (32 - 16)) >> (32 - 16);
                }

                public override string ToString()
                {
                    string retVal = "";
                    retVal += $"Value: { value } (offset: 0, width: 16)\r\n";
                    return retVal;
                }
            }
            internal class OutZRegister : Register
            {
                internal OutZRegister(RegisterManager regManager) : base(regManager, 0x87, 2, false) { }

                public int value { get; set; }

                internal override long getValue() { return ((value & 0xFFFF) << 0); }
                internal override void setValue(long _value)
                {
                    value = (int)(((_value >> 0) & 0xFFFF) << (32 - 16)) >> (32 - 16);
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