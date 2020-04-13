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

        /// <summary>
        /// Helper property so our template knows if this is the last enum value (for trailing commas, etc)
        /// </summary>
        public bool Last { get; set; }
        public EnumVal(int val)
        {
            Value = val;
        }
    }
}
