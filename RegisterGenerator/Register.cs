using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegisterGenerator
{
    public class Register
    {
        public string Name { get; set; }
        public string LibraryName { get; set; }
        public string CapitalizedName => Name.ToPascalCase();
        public string Address { get; set; }
        public int AddressNumber => int.Parse(Address.Replace("0x", ""), System.Globalization.NumberStyles.HexNumber);
        public string Access { get; set; }
        public int Width { get; set; }
        public string IsBigEndian { get; set; }
        public Dictionary<string, Value> Values { get; set; }
        public bool IsReadOnly => Access == "read";
        public bool IsWriteOnly => Access == "write";
        public bool IsSigned { get; set; }
        public int Offset { get; set; } = 0;
        public int NumBytes => (Width - 1) / 8 + 1;

        public void Preprocess()
        {
            if (IsBigEndian != "true")
            {
                IsBigEndian = "false";
            }

            if (Values == null)
            {
                Values = new Dictionary<string, Value>();
                // single-value register
                Values.Add("value",
                    new Value() {Name = "value", Width = this.Width, Offset = this.Offset, IsSigned = this.IsSigned});
            }
            else
            {
                // offsets might not be explicitly given, so calculate them:
                int offset = 0;
                var numBits = 0;

                foreach (var reg in Values)
                {
                    reg.Value.Name = reg.Key; // set the name from the key

                    if (reg.Value.Offset.HasValue)
                    {
                        offset = reg.Value.Offset.Value + reg.Value.Width;
                    }
                    else
                    {
                        reg.Value.Offset = offset;
                        offset += reg.Value.Width;
                    }

                    var width = reg.Value.Offset + reg.Value.Width;
                    if (width > numBits)
                        numBits = width.Value;

                    reg.Value.Preprocess(); // do any setup the register needs
                }


                if (Width == 0) // no width explicitly set; infer from the values
                {
                    Width = numBits;
                }
            }

            Values.Values.Last().Last = true;
        }
    }
}
