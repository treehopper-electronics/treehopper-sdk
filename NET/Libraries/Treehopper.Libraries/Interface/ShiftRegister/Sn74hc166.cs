using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Treehopper;

namespace Treehopper.Libraries.Interface.ShiftRegister
{
    public class Sn74hc166
    {
        public Collection<IDigitalInPin> Pins { get; set; } = new Collection<IDigitalInPin>();

        public Sn74hc166(Spi spiModule, IDigitalOutPin loadPin)
        {
            loadPin.MakeDigitalPushPullOut();
            spiModule.Enabled = true;
        }
    }
}
