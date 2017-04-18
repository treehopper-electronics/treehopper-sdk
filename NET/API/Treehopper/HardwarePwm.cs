namespace Treehopper
{
    using System;
    using Utilities;

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
        private Pin pin;
        private double dutyCycle;
        private double pulseWidth;
        private bool isEnabled = false;
        private TreehopperUsb board;

        internal HardwarePwm(Pin pin)
        {
            this.pin = pin;
            board = pin.Board;
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
                        board.HardwarePwmManager.StartPin(pin);
                        pin.Mode = PinMode.Reserved;
                    }
                    else
                    {
                        board.HardwarePwmManager.StopPin(pin);
                        pin.Mode = PinMode.Unassigned;
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
                pulseWidth = dutyCycle * board.HardwarePwmManager.PeriodMicroseconds;
                board.HardwarePwmManager.SetDutyCycle(pin, value);
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
                if (value > board.HardwarePwmManager.PeriodMicroseconds || value < 0.0)
                    throw new ArgumentOutOfRangeException("PulseWidth", "PulseWidth must be between 0.0 and " + board.HardwarePwmManager.PeriodMicroseconds);
                pulseWidth = value;

                DutyCycle = pulseWidth / board.HardwarePwmManager.PeriodMicroseconds;
            }
        }

        /// <summary>
        /// Returns this PWM channel's data in an easy-to-read format
        /// </summary>
        /// <returns>A string representation of the channel</returns>
        public override string ToString()
        {
            if (Enabled)
                return $"{DutyCycle * 100:0.00}% duty cycle ({PulseWidth:0.00} us pulse width)";
            else
                return "Not enabled";
        }
    }
}
