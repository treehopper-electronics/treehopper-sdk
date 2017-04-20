using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Treehopper.Libraries.Utilities;

namespace Treehopper.Libraries
{
    public class Register : IEnumerable<RegisterValue>
    {
        public delegate void RegisterUpdatedHandler(Register register);

        private RegisterAccess access;

        public Register(string name, int address, RegisterAccess access, params RegisterValue[] registerValues)
        {
            Name = name;
            Address = address;
            this.access = access;
            foreach (var registerValue in registerValues)
            {
                this.registerValues.Add(registerValue.Name, registerValue);
                registerValue.ValueUpdated += RegisterValue_ValueUpdated;
            }
        }

        public Endianness Endianness { get; set; } = Endianness.LittleEndian;

        private void RegisterValue_ValueUpdated(RegisterValue register)
        {
            RegisterUpdated?.Invoke(this);
        }

        public byte[] Bytes
        {
            get
            {
                long currentValue = 0;

                var retVal = new byte[TotalBytes];

                foreach (var reg in registerValues.Values)
                {
                    var mask = (1 << reg.Length) - 1;
                    currentValue |= (reg.Value & mask) << reg.Offset;
                }

                for (var i = 0; i < TotalBytes; i++)
                    retVal[i] = (byte) ((currentValue >> (8 * i)) & 0xFF);

                if (BitConverter.IsLittleEndian ^ (Endianness == Endianness.LittleEndian))
                    retVal = retVal.Reverse().ToArray();

                return retVal;
            }
            set
            {
                if (BitConverter.IsLittleEndian ^ (Endianness == Endianness.LittleEndian))
                    value = value.Reverse().ToArray();

                long regVal = 0;

                for (var i = 0; i < value.Length; i++)
                    regVal |= value[i] << (i * 8);

                foreach (var reg in registerValues.Values)
                {
                    var mask = (1 << reg.Length) - 1;
                    var val = (int) ((regVal >> reg.Offset) & mask);

                    switch (reg.Depth)
                    {
                        case RegisterDepth.SignedByte:
                            reg.updateValue((sbyte) val);
                            break;

                        case RegisterDepth.SignedShort:
                            reg.updateValue((short) val);
                            break;

                        case RegisterDepth.SignedInt:
                        case RegisterDepth.Unsigned:
                            reg.updateValue(val);
                            break;
                    }
                }
            }
        }

        public int TotalBytes
        {
            get
            {
                var numBits = 0;
                foreach (var reg in registerValues.Values)
                {
                    var width = reg.Offset + reg.Length;
                    if (width > numBits)
                        numBits = width;
                }

                return (numBits - 1) / 8 + 1;
            }
        }

        public Register(string name, int address)
        {
            Name = name;
            Address = address;
        }

        private readonly Dictionary<string, RegisterValue> registerValues = new Dictionary<string, RegisterValue>();

        public int Address { get; }

        public string Name { get; }

        public int this[string val]
        {
            get { return registerValues[val].Value; }
            set { registerValues[val].Value = value; }
        }

        public IEnumerator<RegisterValue> GetEnumerator()
        {
            return registerValues.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public event RegisterUpdatedHandler RegisterUpdated;
    }
}