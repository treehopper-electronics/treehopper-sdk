using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Interface.I2cMux
{
    /// <summary>
    /// Use a 4052-type analog mux as a 4-port I2C mux
    /// </summary>
    public class I2cMux4052 : I2cMux
    {
        private DigitalOutPin a;
        private DigitalOutPin b;

        /// <summary>
        /// Construct an <see cref="I2cMux"/> using a standard 4052-style two-bit 4:1 mux. 
        /// </summary>
        /// <param name="MuxedPort">The upstream port to mux</param>
        /// <param name="a">The A (S0) input of the 4052</param>
        /// <param name="b">The B (S1) input of the 4052</param>
        public I2cMux4052(I2c MuxedPort, DigitalOutPin a, DigitalOutPin b) : base(MuxedPort, 4)
        {
            this.a = a;
            this.b = b;
        }

        /// <summary>
        /// set the mux
        /// </summary>
        /// <param name="index">The index to be muxed</param>
        protected override void setMux(int index)
        {
            switch(index)
            {
                case 0:
                    a.DigitalValue = false;
                    b.DigitalValue = false;
                    break;
                case 1:
                    a.DigitalValue = true;
                    b.DigitalValue = false;
                    break;
                case 2:
                    a.DigitalValue = false;
                    b.DigitalValue = true;
                    break;
                case 3:
                    a.DigitalValue = true;
                    b.DigitalValue = true;
                    break;
            }
        }
    }
}
