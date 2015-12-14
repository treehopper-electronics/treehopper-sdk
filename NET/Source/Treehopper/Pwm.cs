using System;

namespace Treehopper
{

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
        TreehopperUsb Board;
        internal Pwm(Pin pin)
        {
            Pin = pin;
            Board = pin.Board;
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
            set
            {
                if (value != isEnabled)
                {
                    isEnabled = value;
                    if (isEnabled)
                    {
                        Board.PwmMgr.StartPin(Pin);
                        Pin.Mode = PinMode.Reserved;
                    }
                    else
                    {
                        Board.PwmMgr.StopPin(Pin);
                        Pin.Mode = PinMode.DigitalInput;
                    }
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
                Board.PwmMgr.SetDutyCycle(Pin, value);

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
