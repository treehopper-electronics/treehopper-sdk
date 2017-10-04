using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class Supports : System.Attribute
    {
        public Supports(string Vendor, string Product)
        {
            this.Vendor = Vendor;
            this.Product = Product;
        }

        public string Vendor { get; private set; }
        public string Product { get; private set; }
    }
}
