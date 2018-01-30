using Treehopper.Utilities;

namespace Treehopper
{
    /// <summary>
    ///     This class provides software-based pulse-width modulation (PWM) on any pin.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The period of the SoftPwm module defaults to approximately 60 Hz. Changing this period will affect all active
    ///         SoftPwm pins.
    ///     </para>
    ///     <para>
    ///         Compared to <see cref="HardwarePwm" /> (which is implemented in hardware), SoftPwm has relatively high jitter.
    ///         However, it has relatively
    ///         good precision, fine-tuned period control, and works well even when many (or all!) Treehopper pins are used for
    ///         SoftPwm.
    ///     </para>
    /// </remarks>
    public class SoftPwm : Pwm
    {
        private readonly TreehopperUsb _board;
        private readonly Pin _pin;
        private bool _isEnabled;

        internal SoftPwm(Pin pin)
        {
            _board = pin.Board;
            _pin = pin;
        }

        /// <summary>
        ///     Gets or sets the value determining whether the PWM functionality of the pin is enabled.
        /// </summary>
        public bool Enabled
        {
            get { return _isEnabled; }

            set
            {
                if (value != _isEnabled)
                {
                    _isEnabled = value;
                    if (_isEnabled)
                    {
                        _board.SoftPwmMgr.StartPin(_pin);
                        _pin.Mode = PinMode.PushPullOutput;
                    }
                    else
                    {
                        _board.SoftPwmMgr.StopPin(_pin);
                        _pin.Mode = PinMode.DigitalInput;
                    }
                }
            }
        }

        /// <summary>
        ///     Get or set the duty cycle of the pin. This property has a range between 0.0 and 1.0, inclusive.
        /// </summary>
        /// <remarks>
        ///     Due to software implementation constraints, the minimum duty cycle
        ///     that will be generated is approximately 0.0003, and the maximum
        ///     duty cycle will be 0.9993.
        /// </remarks>
        public double DutyCycle
        {
            get { return _board.SoftPwmMgr.GetDutyCycle(_pin); }

            set
            {
                if (value > 1.0 || value < 0.0)
                    Utility.Error("DutyCycle must be between 0.0 and 1.0");

                _board.SoftPwmMgr.SetDutyCycle(_pin, value.Constrain());
            }
        }

        /// <summary>
        ///     Get or set the pulse width, in microseconds, of the pin. This property has a range between 0 and 16409.
        /// </summary>
        /// <remarks>
        ///     Due to software implementation constraints, the minimum pulse width that will be generated is approximately
        ///     5.4 microseconds, and the maximum pulse width will be approximately 16,400.
        /// </remarks>
        public double PulseWidth
        {
            get { return _board.SoftPwmMgr.GetPulseWidth(_pin); }

            set
            {
                if (value > 16409 || value < 0.0)
                    Utility.Error("PulseWidth must be between 0 and 16409");

                _board.SoftPwmMgr.SetPulseWidth(_pin, value.Constrain(0, 16409));
            }
        }

        /// <summary>
        ///     Gets a string representation of the soft-PWM pin's state
        /// </summary>
        /// <returns>The soft-PWM pin's state</returns>
        public override string ToString()
        {
            return Enabled ? $"{DutyCycle * 100:0.00}% duty cycle ({PulseWidth:0.00} us pulse width)" : "Not enabled";
        }
    }
}