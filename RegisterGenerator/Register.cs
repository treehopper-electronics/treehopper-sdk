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
        public string CapitalizedName => Name.ToPascalCase();
        public string Address { get; set; }
        public int AddressNumber => int.Parse(Address.Replace("0x", ""), System.Globalization.NumberStyles.HexNumber);
        public string Access { get; set; }
        public int Width { get; set; }
        public string LittleEndian { get; set; } = "true";
        public List<Value> Values { get; set; }
        public bool IsReadOnly => Access == "read";
        public bool IsWriteOnly => Access == "write";
        public bool IsSigned { get; set; }
        public int Offset { get; set; }
        public int NumBytes => (Width - 1) / 8 + 1;

        public void Preprocess()
        {
            if (Width == 0) // no width explicitly set; infer from the values
            {
                var numBits = 0;
                foreach (var reg in Values)
                {
                    var width = reg.Offset + reg.Width;
                    if (width > numBits)
                        numBits = width;
                }

                Width = numBits;
            }

            if (Values == null)
            {
                // single-value register
                Values = new List<Value>();
                Values.Add(new Value() { Name="Value", Width = this.Width, Offset = this.Offset, IsSigned =  this.IsSigned });
            }

            Values.Last().Last = true;
        }
    }
}
