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
        // These properties are filled in by the JSON file
        /// <summary>
        /// The name of the register.  Required.
        /// 
        /// This should match the name in the datasheet if it is reasonably coherent; cryptically short names should be expanded to a human-readable name to minimize datasheet consultation. For example, a register named "EnUTh" in a datasheet should be named "EnUpperThresh" or "EnableUpperThreshold"
        /// </summary>
        public string Name { get; set; }
        public string LibraryName { get; set; }
        public string Address { get; set; }
        public string Access { get; set; }
        public int Width { get; set; }
        public string IsBigEndian { get; set; }
        public bool IsSigned { get; set; }
        public int Offset { get; set; } = 0;


        // These are "helper" properties we use to preprocess the class that are derived from the above properties.
        public string CapitalizedName => Name.ToPascalCase();
        public int AddressNumber => int.Parse(Address.Replace("0x", ""), System.Globalization.NumberStyles.HexNumber);
        public string IsBigEndianCapitalized => IsBigEndian.ToPascalCase();
        public Dictionary<string, Value> Values { get; set; }
        public bool IsReadOnly => Access == "read";
        public bool IsWriteOnly => Access == "write";

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

                int width = this.Width > 0 ? this.Width : 8; // default width
                int offset = this.Offset;
                Values.Add("value",
                    new Value() {Name = "value", Width = width, Offset = offset, IsSigned = this.IsSigned});
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
