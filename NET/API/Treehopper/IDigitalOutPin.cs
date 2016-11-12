using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper
{
    public interface IDigitalOutPin
    {
        bool DigitalValue { get; set; }
        void ToggleOutput();

        void MakeDigitalPushPullOut();
    }
}
