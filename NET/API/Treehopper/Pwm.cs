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
        int pulseWidth;
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
                        Board.PwmManager.StartPin(Pin);
                        Pin.Mode = PinMode.Reserved;
                    }
                    else
                    {
                        Board.PwmManager.StopPin(Pin);
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
                if (value > 1.0 || value < 0.0)
                    throw new ArgumentOutOfRangeException("DutyCycle", "DutyCycle must be between 0.0 and 1.0");
                dutyCycle = value;

                // update the pulseWidth just in case the user wants to read from the value
                pulseWidth = (int)Math.Round(dutyCycle * Board.PwmManager.PeriodMicroseconds);
                Board.PwmManager.SetDutyCycle(Pin, value);
            }
        }

        /// <summary>
        /// Get or set the pulse width, in microseconds, of the pin
        /// </summary>
        public int PulseWidth
        {
            get
            {
                return pulseWidth;
            }
            set
            {
                if (value > Board.PwmManager.PeriodMicroseconds || value < 0.0)
                    throw new ArgumentOutOfRangeException("PulseWidth", "PulseWidth must be between 0.0 and " + Board.PwmManager.PeriodMicroseconds);
                pulseWidth = value;

                DutyCycle = pulseWidth / Board.PwmManager.PeriodMicroseconds;
            }
        }
    }
}
