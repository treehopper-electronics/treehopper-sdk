using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper
{
    public interface IDigitalPin
    {
        bool DigitalValue { get; set; }
        void ToggleOutput();
        Task<bool> AwaitDigitalValueChange();
        void MakeDigitalPushPullOut();
        void MakeDigitalIn();
    }
}
