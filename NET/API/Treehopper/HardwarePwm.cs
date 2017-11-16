using Treehopper.Utilities;

namespace Treehopper
{
    /// <summary>
    ///     The Pwm class manages the hardware PWM module on the Treehopper board.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Both Pwm and <see cref="SoftPwm" /> implement <see cref="Pwm" />, which provides a useful interface to generic
    ///         PWM functionality.
    ///     </para>
    /// </remarks>
    public class HardwarePwm : Pwm
    {
        private readonly TreehopperUsb _board;
        private readonly Pin _pin;
        private double _dutyCycle;
        private bool _isEnabled;
        private double _pulseWidth;

        internal HardwarePwm(Pin pin)
        {
            _pin = pin;
            _board = pin.Board;
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
                        _board.HardwarePwmManager.StartPin(_pin);
                        _pin.Mode = PinMode.Reserved;
                    }
                    else
                    {
                        _board.HardwarePwmManager.StopPin(_pin);
                        _pin.Mode = PinMode.Unassigned;
                    }
                }
            }
        }

        /// <summary>
        ///     Get or set the duty cycle (0-1) of the pin
        /// </summary>
        public double DutyCycle
        {
            get { return _dutyCycle; }

            set
            {
                if (_dutyCycle.CloseTo(value)) return;
                if (value > 1.0 || value < 0.0)
                {
                    Utility.Error("DutyCycle must be between 0.0 and 1.0");
                    _dutyCycle = value.Constrain();
                }

                // update the pulseWidth just in case the user wants to read from the value
                _pulseWidth = _dutyCycle * _board.HardwarePwmManager.PeriodMicroseconds;
                _board.HardwarePwmManager.SetDutyCycle(_pin, _dutyCycle);
            }
        }

        /// <summary>
        ///     Get or set the pulse width, in microseconds, of the pin
        /// </summary>
        public double PulseWidth
        {
            get { return _pulseWidth; }

            set
            {
                if (_pulseWidth.CloseTo(value)) return;
                if (value > _board.HardwarePwmManager.PeriodMicroseconds || value < 0.0)
                {
                    Utility.Error($"PulseWidth must be between 0.0 and {_board.HardwarePwmManager.PeriodMicroseconds}");
                    _pulseWidth = value.Constrain(0, _board.HardwarePwmManager.PeriodMicroseconds);
                }

                DutyCycle = _pulseWidth / _board.HardwarePwmManager.PeriodMicroseconds;
            }
        }

        /// <summary>
        ///     Returns this PWM channel's data in an easy-to-read format
        /// </summary>
        /// <returns>A string representation of the channel</returns>
        public override string ToString()
        {
            if (Enabled)
                return $"{DutyCycle * 100:0.00}% duty cycle ({PulseWidth:0.00} us pulse width)";
            return "Not enabled";
        }
    }
}