using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Displays
{
    public abstract class GraphicDisplay
    {
        public int Height { get; protected set; }
        public int Width { get; protected set; }

        public byte[] RawBuffer { get; protected set; }

        public GraphicDisplay(int Width, int Height)
        {
            this.Width = Width;
            this.Height = Height;
        }

        public void Flush()
        {
            flush();
        }

        protected abstract void flush();

    }
}
