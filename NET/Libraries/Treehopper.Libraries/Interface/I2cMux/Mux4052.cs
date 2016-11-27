using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Interface.I2cMux
{
    public class Mux4052 : I2cMux
    {
        private IDigitalOutPin a;
        private IDigitalOutPin b;

        public Mux4052(II2c MuxedPort, IDigitalOutPin a, IDigitalOutPin b) : base(MuxedPort)
        {
            this.a = a;
            this.b = b;
        }

        protected override void setMux(int index)
        {
            
        }
    }
}
