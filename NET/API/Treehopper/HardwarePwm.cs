using System.Threading.Tasks;
using Treehopper.Utilities;

namespace Treehopper
{
    /** Built-in hardware PWM channels

\note
Treehopper has two types of PWM support --- Hardware and Software PWM. For information on software PWM functionality, visit Treehopper.Pin.

Treehopper has three 16-bit hardware PWM channels, \link TreehopperUsb.Pwm1 Pwm1\endlink, \link TreehopperUsb.Pwm2 Pwm2\endlink, and \link TreehopperUsb.Pwm3 Pwm3\endlink. These correspond to pins 7, 8, and 9.

You can change the frequency used by the PWM channels by modifying the \link HardwarePwmManager.Frequency TreehopperUsb.HardwarePwmManager.Frequency\endlink (this is a global setting that affects all three channels).

     */
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
                        if (TreehopperUsb.Settings.PropertyWritesReturnImmediately)
                            _board.HardwarePwmManager.StartPinAsync(_pin).Forget();
                        else
                            Task.Run(() => _board.HardwarePwmManager.StartPinAsync(_pin)).Wait();

                        _pin.Mode = PinMode.Reserved;
                    }
                    else
                    {
                        if (TreehopperUsb.Settings.PropertyWritesReturnImmediately)
                            _board.HardwarePwmManager.StopPinAsync(_pin).Forget();
                        else
                            Task.Run(() => _board.HardwarePwmManager.StopPinAsync(_pin)).Wait();

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
                }
                _dutyCycle = value.Constrain();

                // update the pulseWidth just in case the user wants to read from the value
                _pulseWidth = _dutyCycle * _board.HardwarePwmManager.PeriodMicroseconds;

                if (TreehopperUsb.Settings.PropertyWritesReturnImmediately)
                    _board.HardwarePwmManager.SetDutyCycleAsync(_pin, _dutyCycle).Forget();
                else
                    Task.Run(() => _board.HardwarePwmManager.SetDutyCycleAsync(_pin, _dutyCycle)).Wait();
            }
        }

        /// <summary>
        ///     Gets or sets the pulse width, in microseconds, of the pin. The maximum pulse width depends on the hardware PWM frequency --- attempting to write a value greater than the maximum pulse width will trigger an error (and optionally an exception).
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

        public Task DisablePwmAsync()
        {
            _isEnabled = false;
            return _board.HardwarePwmManager.StopPinAsync(_pin);
        }

        public Task EnablePwmAsync()
        {
            _isEnabled = true;
            return _board.HardwarePwmManager.StartPinAsync(_pin);
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