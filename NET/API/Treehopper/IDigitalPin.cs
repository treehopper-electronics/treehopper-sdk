using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper
{
    public interface IDigitalPinBase
    {
        bool DigitalValue { get; set; }
    }
    public interface IDigitalOutPin : IDigitalPinBase
    {
        void ToggleOutput();

        void MakeDigitalPushPullOut();
    }

    public interface IDigitalInPin : IDigitalPinBase
    {
        Task<bool> AwaitDigitalValueChange();

        event OnDigitalInValueChanged DigitalValueChanged;
        void MakeDigitalIn();
    }

    public interface IDigitalIOPin : IDigitalInPin, IDigitalOutPin
    {

    }
}
