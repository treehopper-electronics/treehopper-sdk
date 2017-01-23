using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Interface.Adc
{
    public class AdsPin : AdcPeripheralPin
    {
        private GainControlSetting gainControl = GainControlSetting.mV_6144;
        public AdsPin(IAdcPeripheral parent) : base(parent, 15, 6.144)
        {
        }

        public enum GainControlSetting
        {
            mV_6144,
            mV_4096,
            mV_2048,
            mV_1024,
            mV_512,
            mV_256
        }

        public GainControlSetting GainControl
        {
            get
            {
                return gainControl;
            }

            set
            {
                gainControl = value;
                switch(gainControl)
                {
                    case GainControlSetting.mV_256:
                        ReferenceVoltage = 0.256;
                        break;

                    case GainControlSetting.mV_512:
                        ReferenceVoltage = 0.512;
                        break;

                    case GainControlSetting.mV_1024:
                        ReferenceVoltage = 1.024;
                        break;

                    case GainControlSetting.mV_2048:
                        ReferenceVoltage = 2.048;
                        break;

                    case GainControlSetting.mV_4096:
                        ReferenceVoltage = 4.096;
                        break;

                    case GainControlSetting.mV_6144:
                        ReferenceVoltage = 6.144;
                        break;
                }
                
            }
        }
    }
}
