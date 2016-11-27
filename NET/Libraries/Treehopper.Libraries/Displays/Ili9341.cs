using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Displays
{
    public class Ili9341 : GraphicDisplay
    {
        public Ili9341(int Width, int Height) : base(Width, Height)
        {
        }

        protected override void flush()
        {
            throw new NotImplementedException();
        }
    }
}
