using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper.Utilities;

namespace Treehopper.Libraries.Interface.Dac
{
    /// <summary>
    /// MCP4725 12-bit DAC library
    /// </summary>
    public class Mcp4725 : IDac
    {
        private SMBusDevice dev;
        private double voltage;
        private double normalizedValue;
        private int dacValue;

        /// <summary>
        /// Construct an MCP4725
        /// </summary>
        /// <param name="i2c">The I2c bus this DAC is attached to</param>
        /// <param name="addr">The address pin state</param>
        /// <param name="refVoltage">The voltage of the VDD pin</param>
        /// <param name="speed">The speed to use</param>
        public Mcp4725(I2c i2c, bool addr = false, double refVoltage = 3.3, int speed = 400) : this(i2c, (byte)(0x62 | (addr ? 1 : 0)), refVoltage, speed)
        {

        }

        /// <summary>
        /// Construct an MCP4725
        /// </summary>
        /// <param name="i2c">The I2c bus this DAC is attached to</param>
        /// <param name="address">The address to use</param>
        /// <param name="refVoltage">The voltage of the VDD pin</param>
        /// <param name="speed">The speed to use</param>
        public Mcp4725(I2c i2c, byte address, double refVoltage = 3.3, int speed = 400)
        {
            dev = new SMBusDevice(address, i2c, speed);
            ReferenceVoltage = refVoltage;
        }

        /// <summary>
        /// Gets the reference voltage of this DAC
        /// </summary>
        public double ReferenceVoltage { get; protected set; }

        /// <summary>
        /// Get or set the output voltage of the DAC
        /// </summary>
        public double Voltage
        {
            get
            {
                return voltage;
            }

            set
            {
                if (voltage.CloseTo(value)) return;
                voltage = value;

                Value = (voltage / ReferenceVoltage);
            }
        }



        /// <summary>
        /// Get or set the output value of the DAC, normalized from 0-1
        /// </summary>
        public double Value
        {
            get
            {
                return normalizedValue;
            }

            set
            {
                if (normalizedValue.CloseTo(value)) return; // nothing to see here, folks
                normalizedValue = value;
                if(value > 1.0 || value < 0.0)
                {
                    normalizedValue = normalizedValue.Constrain(0, 1);
                }

                DacValue = (int)Math.Round(normalizedValue * 4095);
            }
        }

        /// <summary>
        /// Get or set the raw, 12-bit value of the DAC
        /// </summary>
        public int DacValue
        {
            get
            {
                return dacValue;
            }

            set
            {
                if (dacValue == value) return;
                dacValue = value;
                if (dacValue > 4095 || dacValue < 0)
                {
                    Utility.Error("The maximum DAC value supported is 4095. Clipping will occur");
                    dacValue = Numbers.Constrain(dacValue, 0, 4095);
                }

                // update the other properties
                normalizedValue = dacValue / 4095.0;
                voltage = normalizedValue * ReferenceVoltage;

                dev.WriteData(new byte[] { (byte)(dacValue >> 8), (byte)(dacValue & 0xff) }).Wait();
            }
        }
    }
}
