using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegisterGenerator
{
    public class EnumVal
    {
        public int Value { get; set; }
        public bool Last { get; set; }
        public EnumVal(int val)
        {
            Value = val;
        }
    }
}
