using System;

namespace Treehopper
{
    /// <summary>
    /// Defines the PWM period options
    /// </summary>
    public enum PwmFrequency { 
        
        /// <summary>
        /// 48 kHz (20.833 microseconds)
        /// </summary>
        Freq_48KHz, 
        
        /// <summary>
        /// 12 kHz (83.333 microseconds)
        /// </summary>
        Freq_12KHz, 
        
        /// <summary>
        /// 3 kHz (333.333 microseconds)
        /// </summary>
        Freq_3KHz,  
        
        /// <summary>
        /// 750 Hz (1.333 milliseconds)
        /// </summary>
        Freq_750HZ 
    };

    /// <summary>
    /// The Pwm class manages the hardware PWM module on the Treehopper board.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Both Pwm and <see cref="SoftPwm"/> implement <see cref="IPwm"/>, which provides a useful interface to generic PWM functionality.
    /// </para>
    /// </remarks>
    public class Pwm : IPwm
    {
        Pin Pin;
        double dutyCycle;
        double pulseWidth;
        double period;
        private bool isEnabled = false;
        private PwmFrequency frequency = PwmFrequency.Freq_750HZ;
        internal Pwm(Pin pin)
        {
            Pin = pin;
        }

        /// <summary>
        /// Gets or sets the PWM frequency of the pin, selected from <see cref="PwmFrequency"/>
        /// </summary>
        public PwmFrequency Frequency
        {
            get
            {
                return frequency;
            }
            set
            {
                if(frequency != value)
                {
                    frequency = value;
                    if(IsEnabled)
                    {
                        Pin.SendCommand(new byte[] { (byte)PinConfigCommands.MakePWMPin, (byte)frequency });
                    }
                }
            }
        }


        /// <summary>
        /// Gets or sets the value determining whether the PWM functionality of the pin is enabled.
        /// </summary>
        public bool IsEnabled
        {
            get 
            {
                return isEnabled;
            }
            set {
                isEnabled = value;
                if(isEnabled)
                {
                    Pin.SendCommand(new byte[] { (byte)PinConfigCommands.MakePWMPin, (byte)Frequency });
                    Pin.State = PinState.PWM;
                }
                else
                {
                    Pin.SendCommand(new byte[] { (byte)PinConfigCommands.MakePWMPin, 0xFF }); // magic byte to disable PWM module
                    Pin.MakeDigitalInput();
                }
            }
        }

        
        /// <summary>
        /// Get or set the duty cycle (0-1) of the pin
        /// </summary>
        public double DutyCycle
        {
            get
            {
                return dutyCycle;
            }
            set
            {
                dutyCycle = value;
                UInt16 register = (UInt16)Math.Round(dutyCycle * 1024.0);
                byte high = (byte)(register >> 2);
                byte low = (byte)(register & 0x3);

                Pin.SendCommand(new byte[] { (byte)PinConfigCommands.SetPWMValue, high, low });
            }
        }

        /// <summary>
        /// Get or set the pulse width, in milliseconds, of the pin
        /// </summary>
        public double PulseWidth
        {
            get
            {
                return pulseWidth;
            }
            set
            {
                pulseWidth = value;

            }
        }
    }
}
