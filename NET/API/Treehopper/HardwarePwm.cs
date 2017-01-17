using System;

namespace Treehopper
{

    /// <summary>
    /// The Pwm class manages the hardware PWM module on the Treehopper board.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Both Pwm and <see cref="SoftPwm"/> implement <see cref="Pwm"/>, which provides a useful interface to generic PWM functionality.
    /// </para>
    /// </remarks>
    public class HardwarePwm : Pwm
    {
        Pin Pin;
        double dutyCycle;
        double pulseWidth;
        private bool isEnabled = false;
        TreehopperUsb Board;
        internal HardwarePwm(Pin pin)
        {
            Pin = pin;
            Board = pin.Board;
        }

        /// <summary>
        /// Gets or sets the value determining whether the PWM functionality of the pin is enabled.
        /// </summary>
        public bool Enabled
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
                        Board.HardwarePwmManager.StartPin(Pin);
                        Pin.Mode = PinMode.Reserved;
                    }
                    else
                    {
                        Board.HardwarePwmManager.StopPin(Pin);
                        Pin.Mode = PinMode.Unassigned;
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
                if (dutyCycle.CloseTo(value)) return;
                if (value > 1.0 || value < 0.0)
                    throw new ArgumentOutOfRangeException("DutyCycle", "DutyCycle must be between 0.0 and 1.0");
                dutyCycle = value;

                // update the pulseWidth just in case the user wants to read from the value
                pulseWidth = (int)Math.Round(dutyCycle * Board.HardwarePwmManager.PeriodMicroseconds);
                Board.HardwarePwmManager.SetDutyCycle(Pin, value);
            }
        }

        /// <summary>
        /// Get or set the pulse width, in microseconds, of the pin
        /// </summary>
        public double PulseWidth
        {
            get
            {
                return pulseWidth;
            }
            set
            {
                if (pulseWidth.CloseTo(value)) return;
                if (value > Board.HardwarePwmManager.PeriodMicroseconds || value < 0.0)
                    throw new ArgumentOutOfRangeException("PulseWidth", "PulseWidth must be between 0.0 and " + Board.HardwarePwmManager.PeriodMicroseconds);
                pulseWidth = value;

                DutyCycle = pulseWidth / Board.HardwarePwmManager.PeriodMicroseconds;
            }
        }

        public override string ToString()
        {
            if (Enabled)
                return string.Format("{0:0.00}% duty cycle ({1:0.00} us pulse width)", DutyCycle*100, PulseWidth);
            else
                return "Not enabled";
        }
    }
}
