/// This file was auto-generated by RegisterGenerator. Any changes to it will be overwritten!
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper;
using Treehopper.Libraries.Utilities;

namespace Treehopper.Libraries.Sensors.Pressure
{
    public partial class Bmp280
    {
        internal enum Oversamplings
        {
            Skipped = 0,
            Oversampling_x1 = 1,
            Oversampling_x2 = 2,
            Oversampling_x4 = 3,
            Oversampling_x8 = 4,
            Oversampling_x16 = 5
        }

        internal enum Modes
        {
            Sleep = 0,
            Forced = 1,
            Normal = 3
        }

        internal enum OversamplingPressures
        {
            Skipped = 0,
            Oversampling_x1 = 1,
            Oversampling_x2 = 2,
            Oversampling_x4 = 3,
            Oversampling_x8 = 4,
            Oversampling_x16 = 5
        }

        internal enum OversamplingTemperatures
        {
            Skipped = 0,
            Oversampling_x1 = 1,
            Oversampling_x2 = 2,
            Oversampling_x4 = 3,
            Oversampling_x8 = 4,
            Oversampling_x16 = 5
        }

        internal enum Filters
        {
            FilterOff = 0,
            Filter2 = 1,
            Filter4 = 2,
            Filter8 = 3,
            Filter16 = 4
        }

        internal enum TStandbies
        {
            Ms_0_5 = 0,
            Ms_62_5 = 1,
            Ms_125 = 2,
            Ms_250 = 3,
            Ms_500 = 4,
            Ms_1000 = 5,
            Ms_10 = 6,
            Ms_20 = 7
        }

        protected class Bmp280Registers : RegisterManager
        {
            internal Bmp280Registers(IRegisterManagerAdapter adapter) : base(adapter, true)
            {
                t1 = new T1Register(this);
                _registers.Add(t1);
                t2 = new T2Register(this);
                _registers.Add(t2);
                t3 = new T3Register(this);
                _registers.Add(t3);
                p1 = new P1Register(this);
                _registers.Add(p1);
                p2 = new P2Register(this);
                _registers.Add(p2);
                p3 = new P3Register(this);
                _registers.Add(p3);
                p4 = new P4Register(this);
                _registers.Add(p4);
                p5 = new P5Register(this);
                _registers.Add(p5);
                p6 = new P6Register(this);
                _registers.Add(p6);
                p7 = new P7Register(this);
                _registers.Add(p7);
                p8 = new P8Register(this);
                _registers.Add(p8);
                p9 = new P9Register(this);
                _registers.Add(p9);
                h1 = new H1Register(this);
                _registers.Add(h1);
                id = new IdRegister(this);
                _registers.Add(id);
                reset = new ResetRegister(this);
                _registers.Add(reset);
                h2 = new H2Register(this);
                _registers.Add(h2);
                h3 = new H3Register(this);
                _registers.Add(h3);
                h4 = new H4Register(this);
                _registers.Add(h4);
                h4h5 = new H4h5Register(this);
                _registers.Add(h4h5);
                h5 = new H5Register(this);
                _registers.Add(h5);
                h6 = new H6Register(this);
                _registers.Add(h6);
                ctrlHumidity = new CtrlHumidityRegister(this);
                _registers.Add(ctrlHumidity);
                status = new StatusRegister(this);
                _registers.Add(status);
                ctrlMeasure = new CtrlMeasureRegister(this);
                _registers.Add(ctrlMeasure);
                config = new ConfigRegister(this);
                _registers.Add(config);
                pressure = new PressureRegister(this);
                _registers.Add(pressure);
                temperature = new TemperatureRegister(this);
                _registers.Add(temperature);
                humidity = new HumidityRegister(this);
                _registers.Add(humidity);
            }

            internal T1Register t1;
            internal T2Register t2;
            internal T3Register t3;
            internal P1Register p1;
            internal P2Register p2;
            internal P3Register p3;
            internal P4Register p4;
            internal P5Register p5;
            internal P6Register p6;
            internal P7Register p7;
            internal P8Register p8;
            internal P9Register p9;
            internal H1Register h1;
            internal IdRegister id;
            internal ResetRegister reset;
            internal H2Register h2;
            internal H3Register h3;
            internal H4Register h4;
            internal H4h5Register h4h5;
            internal H5Register h5;
            internal H6Register h6;
            internal CtrlHumidityRegister ctrlHumidity;
            internal StatusRegister status;
            internal CtrlMeasureRegister ctrlMeasure;
            internal ConfigRegister config;
            internal PressureRegister pressure;
            internal TemperatureRegister temperature;
            internal HumidityRegister humidity;

            internal class T1Register : Register
            {
                internal T1Register(RegisterManager regManager) : base(regManager, 0x88, 2, false) { }

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
            internal class T2Register : Register
            {
                internal T2Register(RegisterManager regManager) : base(regManager, 0x8a, 2, false) { }

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
            internal class T3Register : Register
            {
                internal T3Register(RegisterManager regManager) : base(regManager, 0x8c, 2, false) { }

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
            internal class P1Register : Register
            {
                internal P1Register(RegisterManager regManager) : base(regManager, 0x8e, 2, false) { }

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
            internal class P2Register : Register
            {
                internal P2Register(RegisterManager regManager) : base(regManager, 0x90, 2, false) { }

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
            internal class P3Register : Register
            {
                internal P3Register(RegisterManager regManager) : base(regManager, 0x92, 2, false) { }

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
            internal class P4Register : Register
            {
                internal P4Register(RegisterManager regManager) : base(regManager, 0x94, 2, false) { }

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
            internal class P5Register : Register
            {
                internal P5Register(RegisterManager regManager) : base(regManager, 0x96, 2, false) { }

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
            internal class P6Register : Register
            {
                internal P6Register(RegisterManager regManager) : base(regManager, 0x98, 2, false) { }

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
            internal class P7Register : Register
            {
                internal P7Register(RegisterManager regManager) : base(regManager, 0x9a, 2, false) { }

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
            internal class P8Register : Register
            {
                internal P8Register(RegisterManager regManager) : base(regManager, 0x9c, 2, false) { }

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
            internal class P9Register : Register
            {
                internal P9Register(RegisterManager regManager) : base(regManager, 0x9e, 2, false) { }

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
            internal class H1Register : Register
            {
                internal H1Register(RegisterManager regManager) : base(regManager, 0xa1, 1, false) { }

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
            internal class IdRegister : Register
            {
                internal IdRegister(RegisterManager regManager) : base(regManager, 0xd0, 1, false) { }

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
            internal class ResetRegister : Register
            {
                internal ResetRegister(RegisterManager regManager) : base(regManager, 0xe0, 1, false) { }

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
            internal class H2Register : Register
            {
                internal H2Register(RegisterManager regManager) : base(regManager, 0xe1, 2, false) { }

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
            internal class H3Register : Register
            {
                internal H3Register(RegisterManager regManager) : base(regManager, 0xe3, 1, false) { }

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
            internal class H4Register : Register
            {
                internal H4Register(RegisterManager regManager) : base(regManager, 0xe4, 1, false) { }

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
            internal class H4h5Register : Register
            {
                internal H4h5Register(RegisterManager regManager) : base(regManager, 0xe5, 1, false) { }

                public int h4Low { get; set; }
                public int h5Low { get; set; }

                internal override long getValue() { return ((h4Low & 0xF) << 0) | ((h5Low & 0xF) << 4); }
                internal override void setValue(long _value)
                {
                    h4Low = (int)((_value >> 0) & 0xF);
                    h5Low = (int)((_value >> 4) & 0xF);
                }

                public override string ToString()
                {
                    string retVal = "";
                    retVal += $"H4Low: { h4Low } (offset: 0, width: 4)\r\n";
                    retVal += $"H5Low: { h5Low } (offset: 4, width: 4)\r\n";
                    return retVal;
                }
            }
            internal class H5Register : Register
            {
                internal H5Register(RegisterManager regManager) : base(regManager, 0xe6, 1, false) { }

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
            internal class H6Register : Register
            {
                internal H6Register(RegisterManager regManager) : base(regManager, 0xe7, 1, false) { }

                public int value { get; set; }

                internal override long getValue() { return ((value & 0xFF) << 0); }
                internal override void setValue(long _value)
                {
                    value = (int)(((_value >> 0) & 0xFF) << (32 - 8)) >> (32 - 8);
                }

                public override string ToString()
                {
                    string retVal = "";
                    retVal += $"Value: { value } (offset: 0, width: 8)\r\n";
                    return retVal;
                }
            }
            internal class CtrlHumidityRegister : Register
            {
                internal CtrlHumidityRegister(RegisterManager regManager) : base(regManager, 0xf2, 1, false) { }

                public int oversampling { get; set; }
                public Oversamplings getOversampling() { return (Oversamplings)oversampling; }
                public void setOversampling(Oversamplings enumVal) { oversampling = (int)enumVal; }

                internal override long getValue() { return ((oversampling & 0x7) << 0); }
                internal override void setValue(long _value)
                {
                    oversampling = (int)((_value >> 0) & 0x7);
                }

                public override string ToString()
                {
                    string retVal = "";
                    retVal += $"Oversampling: { oversampling } (offset: 0, width: 3)\r\n";
                    return retVal;
                }
            }
            internal class StatusRegister : Register
            {
                internal StatusRegister(RegisterManager regManager) : base(regManager, 0xf3, 1, false) { }

                public int imUpdate { get; set; }
                public int measuring { get; set; }

                internal override long getValue() { return ((imUpdate & 0x1) << 0) | ((measuring & 0x1) << 3); }
                internal override void setValue(long _value)
                {
                    imUpdate = (int)((_value >> 0) & 0x1);
                    measuring = (int)((_value >> 3) & 0x1);
                }

                public override string ToString()
                {
                    string retVal = "";
                    retVal += $"ImUpdate: { imUpdate } (offset: 0, width: 1)\r\n";
                    retVal += $"Measuring: { measuring } (offset: 3, width: 1)\r\n";
                    return retVal;
                }
            }
            internal class CtrlMeasureRegister : Register
            {
                internal CtrlMeasureRegister(RegisterManager regManager) : base(regManager, 0xf4, 1, false) { }

                public int mode { get; set; }
                public int oversamplingPressure { get; set; }
                public int oversamplingTemperature { get; set; }
                public Modes getMode() { return (Modes)mode; }
                public void setMode(Modes enumVal) { mode = (int)enumVal; }
                public OversamplingPressures getOversamplingPressure() { return (OversamplingPressures)oversamplingPressure; }
                public void setOversamplingPressure(OversamplingPressures enumVal) { oversamplingPressure = (int)enumVal; }
                public OversamplingTemperatures getOversamplingTemperature() { return (OversamplingTemperatures)oversamplingTemperature; }
                public void setOversamplingTemperature(OversamplingTemperatures enumVal) { oversamplingTemperature = (int)enumVal; }

                internal override long getValue() { return ((mode & 0x3) << 0) | ((oversamplingPressure & 0x7) << 2) | ((oversamplingTemperature & 0x7) << 5); }
                internal override void setValue(long _value)
                {
                    mode = (int)((_value >> 0) & 0x3);
                    oversamplingPressure = (int)((_value >> 2) & 0x7);
                    oversamplingTemperature = (int)((_value >> 5) & 0x7);
                }

                public override string ToString()
                {
                    string retVal = "";
                    retVal += $"Mode: { mode } (offset: 0, width: 2)\r\n";
                    retVal += $"OversamplingPressure: { oversamplingPressure } (offset: 2, width: 3)\r\n";
                    retVal += $"OversamplingTemperature: { oversamplingTemperature } (offset: 5, width: 3)\r\n";
                    return retVal;
                }
            }
            internal class ConfigRegister : Register
            {
                internal ConfigRegister(RegisterManager regManager) : base(regManager, 0xf5, 1, false) { }

                public int enable3Wire { get; set; }
                public int filter { get; set; }
                public int tStandby { get; set; }
                public Filters getFilter() { return (Filters)filter; }
                public void setFilter(Filters enumVal) { filter = (int)enumVal; }
                public TStandbies getTStandby() { return (TStandbies)tStandby; }
                public void setTStandby(TStandbies enumVal) { tStandby = (int)enumVal; }

                internal override long getValue() { return ((enable3Wire & 0x1) << 0) | ((filter & 0x7) << 1) | ((tStandby & 0x7) << 4); }
                internal override void setValue(long _value)
                {
                    enable3Wire = (int)((_value >> 0) & 0x1);
                    filter = (int)((_value >> 1) & 0x7);
                    tStandby = (int)((_value >> 4) & 0x7);
                }

                public override string ToString()
                {
                    string retVal = "";
                    retVal += $"Enable3Wire: { enable3Wire } (offset: 0, width: 1)\r\n";
                    retVal += $"Filter: { filter } (offset: 1, width: 3)\r\n";
                    retVal += $"TStandby: { tStandby } (offset: 4, width: 3)\r\n";
                    return retVal;
                }
            }
            internal class PressureRegister : Register
            {
                internal PressureRegister(RegisterManager regManager) : base(regManager, 0xf7, 3, true) { }

                public int value { get; set; }

                internal override long getValue() { return ((value & 0xFFFFF) << 4); }
                internal override void setValue(long _value)
                {
                    value = (int)((_value >> 4) & 0xFFFFF);
                }

                public override string ToString()
                {
                    string retVal = "";
                    retVal += $"Value: { value } (offset: 4, width: 20)\r\n";
                    return retVal;
                }
            }
            internal class TemperatureRegister : Register
            {
                internal TemperatureRegister(RegisterManager regManager) : base(regManager, 0xfa, 3, true) { }

                public int value { get; set; }

                internal override long getValue() { return ((value & 0xFFFFF) << 4); }
                internal override void setValue(long _value)
                {
                    value = (int)((_value >> 4) & 0xFFFFF);
                }

                public override string ToString()
                {
                    string retVal = "";
                    retVal += $"Value: { value } (offset: 4, width: 20)\r\n";
                    return retVal;
                }
            }
            internal class HumidityRegister : Register
            {
                internal HumidityRegister(RegisterManager regManager) : base(regManager, 0xfd, 2, true) { }

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