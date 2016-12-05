using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Interface.PcfSeries
{
    public class Pcf8574 : PcfInterface
    {
        public Pcf8574(I2c I2c, bool Address0 = false, bool Address1 = false, bool Address2 = false) : base(I2c, 8, Address0, Address1, Address2, 0x20)
        {

        }
    }
}
