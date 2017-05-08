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
    public partial class Adxl345
    {
        internal class Adxl345Registers : RegisterManager
        {
            internal Adxl345Registers(SMBusDevice dev = null) : base(dev, true)
            {
                PowerCtl = new PowerCtlRegister(this);
                _registers.Add(PowerCtl);
                DataFormat = new DataFormatRegister(this);
                _registers.Add(DataFormat);
                DataX = new DataXRegister(this);
                _registers.Add(DataX);
                DataY = new DataYRegister(this);
                _registers.Add(DataY);
                DataZ = new DataZRegister(this);
                _registers.Add(DataZ);
            }

            internal PowerCtlRegister PowerCtl;
            internal DataFormatRegister DataFormat;
            internal DataXRegister DataX;
            internal DataYRegister DataY;
            internal DataZRegister DataZ;

            internal class PowerCtlRegister : Register
            {
                internal PowerCtlRegister(RegisterManager regManager) : base(regManager, 0x2D, 1, false) { }

                public int Sleep { get; set; }
                public int Measure { get; set; }

                public async Task<PowerCtlRegister> Read()
                {
                    await manager.Read(this).ConfigureAwait(false);
                    return this;
                }
                internal override long GetValue() { return ((Sleep & 0x1) << 2) | ((Measure & 0x1) << 3); }
                internal override void SetValue(long value)
                {
                    Sleep = (int)((value >> 2) & 0x1);
                    Measure = (int)((value >> 3) & 0x1);
                }

                public override string ToString()
                {
                    string retVal = "";
                    retVal += $"Sleep: { Sleep } (offset: 2, width: 1)\r\n";
                    retVal += $"Measure: { Measure } (offset: 3, width: 1)\r\n";
                    return retVal;
                }
            }
            internal class DataFormatRegister : Register
            {
                internal DataFormatRegister(RegisterManager regManager) : base(regManager, 0x31, 1, false) { }

                public int Range { get; set; }

                public async Task<DataFormatRegister> Read()
                {
                    await manager.Read(this).ConfigureAwait(false);
                    return this;
                }
                internal override long GetValue() { return ((Range & 0x3) << 0); }
                internal override void SetValue(long value)
                {
                    Range = (int)((value >> 0) & 0x3);
                }

                public override string ToString()
                {
                    string retVal = "";
                    retVal += $"Range: { Range } (offset: 0, width: 2)\r\n";
                    return retVal;
                }
            }
            internal class DataXRegister : Register
            {
                internal DataXRegister(RegisterManager regManager) : base(regManager, 0x32, 2, false) { }

                public int Value { get; set; }

                public async Task<DataXRegister> Read()
                {
                    await manager.Read(this).ConfigureAwait(false);
                    return this;
                }
                internal override long GetValue() { return ((Value & 0xFFFF) << 0); }
                internal override void SetValue(long value)
                {
                    Value = (int)(((value >> 0) & 0xFFFF) << (32 - 16)) >> (32 - 16);
                }

                public override string ToString()
                {
                    string retVal = "";
                    retVal += $"Value: { Value } (offset: 0, width: 16)\r\n";
                    return retVal;
                }
            }
            internal class DataYRegister : Register
            {
                internal DataYRegister(RegisterManager regManager) : base(regManager, 0x34, 2, false) { }

                public int Value { get; set; }

                public async Task<DataYRegister> Read()
                {
                    await manager.Read(this).ConfigureAwait(false);
                    return this;
                }
                internal override long GetValue() { return ((Value & 0xFFFF) << 0); }
                internal override void SetValue(long value)
                {
                    Value = (int)(((value >> 0) & 0xFFFF) << (32 - 16)) >> (32 - 16);
                }

                public override string ToString()
                {
                    string retVal = "";
                    retVal += $"Value: { Value } (offset: 0, width: 16)\r\n";
                    return retVal;
                }
            }
            internal class DataZRegister : Register
            {
                internal DataZRegister(RegisterManager regManager) : base(regManager, 0x36, 2, false) { }

                public int Value { get; set; }

                public async Task<DataZRegister> Read()
                {
                    await manager.Read(this).ConfigureAwait(false);
                    return this;
                }
                internal override long GetValue() { return ((Value & 0xFFFF) << 0); }
                internal override void SetValue(long value)
                {
                    Value = (int)(((value >> 0) & 0xFFFF) << (32 - 16)) >> (32 - 16);
                }

                public override string ToString()
                {
                    string retVal = "";
                    retVal += $"Value: { Value } (offset: 0, width: 16)\r\n";
                    return retVal;
                }
            }
        }
    }
}