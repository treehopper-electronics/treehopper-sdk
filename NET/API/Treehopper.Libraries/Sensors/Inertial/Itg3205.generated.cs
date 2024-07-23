/// This file was auto-generated by RegisterGenerator. Any changes to it will be overwritten!
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper;
using Treehopper.Libraries.Utilities;

namespace Treehopper.Libraries.Sensors.Inertial
{
    public partial class Itg3205
    {
        protected class Itg3205Registers : RegisterManager
        {
            internal Itg3205Registers(IRegisterManagerAdapter adapter) : base(adapter, true)
            {
                whoAmI = new WhoAmIRegister(this);
                _registers.Add(whoAmI);
                smplrtDiv = new SmplrtDivRegister(this);
                _registers.Add(smplrtDiv);
                dlpfFs = new DlpfFsRegister(this);
                _registers.Add(dlpfFs);
                intCfg = new IntCfgRegister(this);
                _registers.Add(intCfg);
                intStatus = new IntStatusRegister(this);
                _registers.Add(intStatus);
                temp = new TempRegister(this);
                _registers.Add(temp);
                gyroX = new GyroXRegister(this);
                _registers.Add(gyroX);
                gyroY = new GyroYRegister(this);
                _registers.Add(gyroY);
                gyroZ = new GyroZRegister(this);
                _registers.Add(gyroZ);
                pwrMgm = new PwrMgmRegister(this);
                _registers.Add(pwrMgm);
            }

            internal WhoAmIRegister whoAmI;
            internal SmplrtDivRegister smplrtDiv;
            internal DlpfFsRegister dlpfFs;
            internal IntCfgRegister intCfg;
            internal IntStatusRegister intStatus;
            internal TempRegister temp;
            internal GyroXRegister gyroX;
            internal GyroYRegister gyroY;
            internal GyroZRegister gyroZ;
            internal PwrMgmRegister pwrMgm;

            internal class WhoAmIRegister : Register
            {
                internal WhoAmIRegister(RegisterManager regManager) : base(regManager, 0x00, 1, false) { }

                public int value { get; set; }

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
            internal class SmplrtDivRegister : Register
            {
                internal SmplrtDivRegister(RegisterManager regManager) : base(regManager, 0x15, 1, false) { }

                public int value { get; set; }

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
            internal class DlpfFsRegister : Register
            {
                internal DlpfFsRegister(RegisterManager regManager) : base(regManager, 0x16, 1, false) { }

                public int dlpfCfg { get; set; }
                public int fsSel { get; set; }

                internal override long getValue() { return ((dlpfCfg & 0x7) << 0) | ((fsSel & 0x3) << 3); }
                internal override void setValue(long _value)
                {
                    dlpfCfg = (int)((_value >> 0) & 0x7);
                    fsSel = (int)((_value >> 3) & 0x3);
                }

                public override string ToString()
                {
                    string retVal = "";
                    retVal += $"DlpfCfg: { dlpfCfg } (offset: 0, width: 3)\r\n";
                    retVal += $"FsSel: { fsSel } (offset: 3, width: 2)\r\n";
                    return retVal;
                }
            }
            internal class IntCfgRegister : Register
            {
                internal IntCfgRegister(RegisterManager regManager) : base(regManager, 0x17, 1, false) { }

                public int rawRdyEn { get; set; }
                public int itgRdyEn { get; set; }
                public int intAnyrd2Clear { get; set; }
                public int latchIntEn { get; set; }
                public int open { get; set; }
                public int actl { get; set; }

                internal override long getValue() { return ((rawRdyEn & 0x1) << 0) | ((itgRdyEn & 0x1) << 2) | ((intAnyrd2Clear & 0x1) << 4) | ((latchIntEn & 0x1) << 5) | ((open & 0x1) << 6) | ((actl & 0x1) << 7); }
                internal override void setValue(long _value)
                {
                    rawRdyEn = (int)((_value >> 0) & 0x1);
                    itgRdyEn = (int)((_value >> 2) & 0x1);
                    intAnyrd2Clear = (int)((_value >> 4) & 0x1);
                    latchIntEn = (int)((_value >> 5) & 0x1);
                    open = (int)((_value >> 6) & 0x1);
                    actl = (int)((_value >> 7) & 0x1);
                }

                public override string ToString()
                {
                    string retVal = "";
                    retVal += $"RawRdyEn: { rawRdyEn } (offset: 0, width: 1)\r\n";
                    retVal += $"ItgRdyEn: { itgRdyEn } (offset: 2, width: 1)\r\n";
                    retVal += $"IntAnyrd2Clear: { intAnyrd2Clear } (offset: 4, width: 1)\r\n";
                    retVal += $"LatchIntEn: { latchIntEn } (offset: 5, width: 1)\r\n";
                    retVal += $"Open: { open } (offset: 6, width: 1)\r\n";
                    retVal += $"Actl: { actl } (offset: 7, width: 1)\r\n";
                    return retVal;
                }
            }
            internal class IntStatusRegister : Register
            {
                internal IntStatusRegister(RegisterManager regManager) : base(regManager, 0x1a, 1, false) { }

                public int rawDataReady { get; set; }
                public int itgReady { get; set; }

                internal override long getValue() { return ((rawDataReady & 0x1) << 0) | ((itgReady & 0x1) << 2); }
                internal override void setValue(long _value)
                {
                    rawDataReady = (int)((_value >> 0) & 0x1);
                    itgReady = (int)((_value >> 2) & 0x1);
                }

                public override string ToString()
                {
                    string retVal = "";
                    retVal += $"RawDataReady: { rawDataReady } (offset: 0, width: 1)\r\n";
                    retVal += $"ItgReady: { itgReady } (offset: 2, width: 1)\r\n";
                    return retVal;
                }
            }
            internal class TempRegister : Register
            {
                internal TempRegister(RegisterManager regManager) : base(regManager, 0x1b, 2, true) { }

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
            internal class GyroXRegister : Register
            {
                internal GyroXRegister(RegisterManager regManager) : base(regManager, 0x1d, 2, true) { }

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
            internal class GyroYRegister : Register
            {
                internal GyroYRegister(RegisterManager regManager) : base(regManager, 0x1f, 2, true) { }

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
            internal class GyroZRegister : Register
            {
                internal GyroZRegister(RegisterManager regManager) : base(regManager, 0x21, 2, true) { }

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
            internal class PwrMgmRegister : Register
            {
                internal PwrMgmRegister(RegisterManager regManager) : base(regManager, 0x3e, 1, false) { }

                public int clkSel { get; set; }
                public int stbyZg { get; set; }
                public int stbyYg { get; set; }
                public int stbyXg { get; set; }
                public int sleep { get; set; }
                public int hReset { get; set; }

                internal override long getValue() { return ((clkSel & 0x7) << 0) | ((stbyZg & 0x1) << 3) | ((stbyYg & 0x1) << 4) | ((stbyXg & 0x1) << 5) | ((sleep & 0x1) << 6) | ((hReset & 0x1) << 7); }
                internal override void setValue(long _value)
                {
                    clkSel = (int)((_value >> 0) & 0x7);
                    stbyZg = (int)((_value >> 3) & 0x1);
                    stbyYg = (int)((_value >> 4) & 0x1);
                    stbyXg = (int)((_value >> 5) & 0x1);
                    sleep = (int)((_value >> 6) & 0x1);
                    hReset = (int)((_value >> 7) & 0x1);
                }

                public override string ToString()
                {
                    string retVal = "";
                    retVal += $"ClkSel: { clkSel } (offset: 0, width: 3)\r\n";
                    retVal += $"StbyZg: { stbyZg } (offset: 3, width: 1)\r\n";
                    retVal += $"StbyYg: { stbyYg } (offset: 4, width: 1)\r\n";
                    retVal += $"StbyXg: { stbyXg } (offset: 5, width: 1)\r\n";
                    retVal += $"Sleep: { sleep } (offset: 6, width: 1)\r\n";
                    retVal += $"HReset: { hReset } (offset: 7, width: 1)\r\n";
                    return retVal;
                }
            }
        }
    }
}