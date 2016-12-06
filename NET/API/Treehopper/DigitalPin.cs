using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper
{
    public interface DigitalPinBase
    {
        bool DigitalValue { get; set; }
    }
    public interface DigitalOutPin : DigitalPinBase
    {
        void ToggleOutput();

        void MakeDigitalPushPullOut();
    }

    public interface DigitalInPin : DigitalPinBase
    {
        Task<bool> AwaitDigitalValueChange();

        event OnDigitalInValueChanged DigitalValueChanged;
        void MakeDigitalIn();
    }

    public interface DigitalIOPin : DigitalInPin, DigitalOutPin
    {

    }
}
