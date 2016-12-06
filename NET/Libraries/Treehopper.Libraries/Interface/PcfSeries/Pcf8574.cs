using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Interface.PcfSeries
{
    /// <summary>
    /// An 8-bit I2c I/O expander
    /// </summary>
    public class Pcf8574 : PcfInterface
    {
        /// <summary>
        /// Construct a PCF8574
        /// </summary>
        /// <param name="I2c">The I2c interface to use</param>
        /// <param name="Address0">The state of the Address0 pin</param>
        /// <param name="Address1">The state of the Address1 pin</param>
        /// <param name="Address2">The state of the Address2 pin</param>
        public Pcf8574(I2c I2c, bool Address0 = false, bool Address1 = false, bool Address2 = false) : base(I2c, 8, Address0, Address1, Address2, 0x20)
        {

        }
    }
}
