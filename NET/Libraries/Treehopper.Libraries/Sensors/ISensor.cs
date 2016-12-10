using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Sensors
{
    public interface ISensor
    {
        bool AutoUpdateWhenPropertyRead { get; set; }
        void Update();
    }
}
