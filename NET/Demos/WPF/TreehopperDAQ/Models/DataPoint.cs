using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreehopperDAQ.Models
{
    public class DataPoint
    {
        public double TimestampOffset { get; set; }
        public double[] Values { get; set; }
    }
}
