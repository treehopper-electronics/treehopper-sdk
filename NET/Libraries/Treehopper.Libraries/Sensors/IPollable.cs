using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Sensors
{
    public interface IPollable
    {
        bool AutoUpdateWhenPropertyRead { get; set; }
        Task Update();
    }
}
