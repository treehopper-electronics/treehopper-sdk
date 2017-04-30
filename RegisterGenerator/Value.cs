using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegisterGenerator
{
    public class Value
    {
        public string Name { get; set; }
        public string CapitalizedName => Name.ToPascalCase();
        public int Offset { get; set; } = 0;
        public int Width { get; set; }
        public string Bitmask => $"0x{((1 << Width) - 1):X}";
        public bool IsSigned { get; set; }
        public bool Last { get; set; }
    }
}
