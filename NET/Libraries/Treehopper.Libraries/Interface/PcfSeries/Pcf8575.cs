using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Interface.PcfSeries
{
    public class Pcf8575 : PcfInterface
    {
        public Pcf8575(I2c i2c, bool Address0 = false, bool Address1 = false, bool Address2 = false) : base(i2c, 16, Address0, Address1, Address2, 0x20)
        {
        }
    }
}
