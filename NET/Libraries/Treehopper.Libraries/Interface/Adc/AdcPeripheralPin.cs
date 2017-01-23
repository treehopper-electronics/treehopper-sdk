using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper.Utilities;

namespace Treehopper.Libraries.Interface.Adc
{
    public class AdcPeripheralPin : AdcPin
    {
        private double analogVoltage;
        private double analogValue;
        private int adcValue;
        private IAdcPeripheral parent;
        private int bitDepth;

        public AdcPeripheralPin(IAdcPeripheral parent, int bitDepth, double refVoltage)
        {
            this.parent = parent;
            this.bitDepth = bitDepth;
            ReferenceVoltage = refVoltage;
        }

        public int AdcValueChangedThreshold { get; set; }
        public int AdcValue
        {
            internal set
            {
                if (adcValue.CloseTo(value, AdcValueChangedThreshold))
                {
                    adcValue = value;
                    AdcValueChanged?.Invoke(this, new AdcValueChangedEventArgs(adcValue));
                }
                else
                {
                    adcValue = value;
                }

                analogValue = ((double)adcValue / ((2 << (bitDepth - 1)) - 1));
                analogVoltage = ReferenceVoltage * analogValue;
            }

            get
            {
                if (parent.AutoUpdateWhenPropertyRead) parent.Update().Wait();
                return adcValue;
            }
        }

        public double AnalogValueChangedThreshold { get; set; }

        public double AnalogValue
        {
            internal set
            {
                if (analogValue.CloseTo(value, AnalogValueChangedThreshold))
                {
                    analogValue = value;
                    AnalogValueChanged?.Invoke(this, new AnalogValueChangedEventArgs(analogValue));
                }
                else
                {
                    analogValue = value;
                }
            }

            get
            {
                if (parent.AutoUpdateWhenPropertyRead) parent.Update().Wait();
                return analogValue;
            }
        }

        public double AnalogVoltageChangedThreshold { get; set; }

        public double AnalogVoltage
        {
            internal set
            {
                if (analogVoltage.CloseTo(value, AnalogVoltageChangedThreshold))
                {
                    analogVoltage = value;
                    AnalogVoltageChanged?.Invoke(this, new AnalogVoltageChangedEventArgs(analogVoltage));
                }
                else
                {
                    analogValue = value;
                }
            }

            get
            {
                if (parent.AutoUpdateWhenPropertyRead) parent.Update().Wait();
                return analogVoltage;
            }
        }

        public double ReferenceVoltage { get; protected set; }

        public event OnAdcValueChanged AdcValueChanged;
        public event OnAnalogValueChanged AnalogValueChanged;
        public event OnAnalogVoltageChanged AnalogVoltageChanged;

        public void MakeAnalogIn()
        {

        }
    }
}
