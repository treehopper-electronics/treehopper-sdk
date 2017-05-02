using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegisterGenerator
{
    public static class Utility
    {
        public static string ToPascalCase(this string str)
        {
            string retVal = str[0].ToString().ToUpper();
            retVal += str.Substring(1);
            return retVal;
        }
    }
}
