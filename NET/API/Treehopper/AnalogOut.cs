using System;

namespace Treehopper
{
    /// <summary>
    /// The AnalogOut functionality produces an analog voltage between 0 and 5V. Unlike Pulse width modulation,
    /// This module produces a true analog voltage, making it suitable for use with comparators and other analog circuits
    /// without needing any filtering.
    /// </summary>
    /// <remarks>
    /// <para>It is important to note that while the DAC output can be enabled on either or both Pin4 and Pin7, 
    /// the output voltage on the pins cannot be controlled independently.</para>
    /// 
    /// <para>Treehopper's 5-bit DAC supports 32 different values, so its resolution is approximately 0.16 volts.</para> 
    /// </remarks>
    public class AnalogOut
    {
        TreehopperUSB Board;
        double dacValue;
        bool enabledOnPin4;
        bool enabledOnPin7;
        byte dacConfig;


        internal AnalogOut(TreehopperUSB board)
        {
            Board = board;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the AnalogOut functionality is activated on Pin4.  
        /// </summary>
        /// <remarks>
        /// It is important to note that while the DAC output can be enabled on either or both Pin4 and Pin7, 
        /// the output value is the same for both pins. 
        /// </remarks>
        public bool IsEnabledOnPin4
        {
            get
            {
                return enabledOnPin4;
            }
            set
            {
                if(enabledOnPin4 != value)
                {
                    enabledOnPin4 = value;
                    Board.RaisePropertyChanged("AnalogOut");
                    if (enabledOnPin4)
                    {
                        dacConfig = (byte)(dacConfig | 0x40);
                        Board.Pin4.State = PinState.AnalogOutput;
                    }
                    else
                    {
                        dacConfig = (byte)(dacConfig & 0xBF); // (0 << 5)
                    }
                    Board.sendCommsConfigPacket(new byte[] { (byte)DeviceCommands.DACConfig, dacConfig });
                } 
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the AnalogOut functionality is activated on Pin7.  
        /// </summary>
        /// <remarks>
        /// It is important to note that while the DAC output can be enabled on either or both Pin4 and Pin7, 
        /// the output value is the same for both pins. 
        /// </remarks>
        public bool IsEnabledOnPin7
        {
            get
            {
                return enabledOnPin7;
            }
            set
            {
                if (enabledOnPin7 != value)
                {
                    enabledOnPin7 = value;
                    Board.RaisePropertyChanged("AnalogOut");
                    if (enabledOnPin7)
                    {
                        dacConfig = (byte)(dacConfig | 0x20);
                        Board.Pin7.State = PinState.AnalogOutput;
                    }
                    else
                    {
                        dacConfig = (byte)(dacConfig & 0xDF); // (0 << 5)
                    }
                    Board.sendCommsConfigPacket(new byte[] { (byte)DeviceCommands.DACConfig, dacConfig });
                }
            }
        }


        /// <summary>
        /// Gets or sets the AnalogOut voltage.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The set voltage is rounded to the nearest value the DAC can produce. This rounded value is available by reading this property. 
        /// Setting a voltage greater than 5 or less than 0 will throw an exception. 
        /// </para>
        /// <para>
        /// This peripheral assumes that the USB bus voltage is equal to 5V. Any bus voltage differences will be reflected in the pin's output,
        /// since the DAC peripheral is ratiometric to the bus voltage. For example, if VBUS = 4.5V, then setting the Voltage property to 5 will
        /// produce an actual output of 4.5V.
        /// </para>
        /// </remarks>
        /// <example>
        /// This example sets the output voltage to 1.5V, and prints the actual voltage set to the debugging console.
        /// <code>
        /// myTreehopperBoard.AnalogOut.Value = 1.5;
        /// Debug.Print(myTreehopperBoard.AnalogOut.Value) // prints 1.451613
        /// </code>
        /// </example>
        public double Voltage
        {
            get { 
                return dacValue; 
            }

            set {
                if (value < 0 || value > 5.0)
                    throw new Exception("AnalogOut voltage must be between 0 and 5V");
                if(dacValue != value)
                {
                    dacValue = value;
                    byte byteValue = (byte)Math.Round(dacValue * 31.0 / 5.0);
                    dacConfig = (byte)(dacConfig & 0xE0);
                    dacConfig = (byte)(dacConfig | (byteValue & 0x1F));
                    dacValue = (double)byteValue * 5.0 / 31.0; // update the actual, rounded value.
                    Board.sendCommsConfigPacket(new byte[] { (byte)DeviceCommands.DACConfig, dacConfig });
                    Board.RaisePropertyChanged("AnalogOut");
                }
                
            }
        }

        /// <summary>
        /// Get the resolution of the DAC peripheral, measured in volts. 
        /// </summary>
        public double Resolution
        {
            get
            {
                return 5.0 / 31.0;
            }
        }


    }
}
