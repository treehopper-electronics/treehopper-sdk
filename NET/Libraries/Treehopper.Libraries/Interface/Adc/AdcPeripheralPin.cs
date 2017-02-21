using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper.Utilities;

namespace Treehopper.Libraries.Interface.Adc
{
    /// <summary>
    /// Base class for ADC peripherals
    /// </summary>
    public class AdcPeripheralPin : AdcPin
    {
        private double analogVoltage;
        private double analogValue;
        private int adcValue;
        private IAdcPeripheral parent;
        private int bitDepth;

        /// <summary>
        /// Construct a new ADC pin that's part of the specified peripheral
        /// </summary>
        /// <param name="parent">The peripheral</param>
        /// <param name="bitDepth">The ADC bitdepth</param>
        /// <param name="refVoltage">The reference voltage used to compare</param>
        public AdcPeripheralPin(IAdcPeripheral parent, int bitDepth, double refVoltage)
        {
            this.parent = parent;
            this.bitDepth = bitDepth;
            ReferenceVoltage = refVoltage;
        }

        /// <summary>
        /// The amount the ADC value should change before triggering an event
        /// </summary>
        public int AdcValueChangedThreshold { get; set; }

        /// <summary>
        /// The raw ADC value
        /// </summary>
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

        /// <summary>
        /// The amount that the analog value (0-1) should change before triggering an event
        /// </summary>
        public double AnalogValueChangedThreshold { get; set; }

        /// <summary>
        /// The analog value (from 0-1) of the ADC pin
        /// </summary>
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

        /// <summary>
        /// The change in voltage required to trigger an event
        /// </summary>
        public double AnalogVoltageChangedThreshold { get; set; }

        /// <summary>
        /// The analog voltage of the pin
        /// </summary>
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

        /// <summary>
        /// The reference voltage used for calculating analog voltage
        /// </summary>
        public double ReferenceVoltage { get; protected set; }

        /// <summary>
        /// Fires whenever the ADC value changes by the specified threshold
        /// </summary>
        public event OnAdcValueChanged AdcValueChanged;

        /// <summary>
        /// Fires whenever the analog value changes by the specified threshold
        /// </summary>
        public event OnAnalogValueChanged AnalogValueChanged;

        /// <summary>
        /// Fires whenever the analog voltage changes by the specified threshold
        /// </summary>
        public event OnAnalogVoltageChanged AnalogVoltageChanged;

        /// <summary>
        /// Make the pin an analog input. This is unused in most implementations
        /// </summary>
        public void MakeAnalogIn()
        {

        }
    }
}
