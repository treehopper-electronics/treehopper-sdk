using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper
{
    public interface IDigitalInPin
    {
        bool DigitalValue { get; }
        Task<bool> AwaitDigitalValueChange();
        void MakeDigitalIn();
    }
}
