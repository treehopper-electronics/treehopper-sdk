using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Interface.Adc
{
    public abstract class AdcPeripheral : IAdcPeripheral
    {
        public AdcPeripheral(int numPins, int bitDepth, double refVoltage)
        {

        }
        public IList<AdcPeripheralPin> Pins { get; set; } = new List<AdcPeripheralPin>();
        public bool AutoUpdateWhenPropertyRead { get; set; }

        public abstract Task Update();
    }
}
