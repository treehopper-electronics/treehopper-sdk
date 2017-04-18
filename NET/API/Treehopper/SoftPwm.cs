﻿namespace Treehopper
{
    using System;

    /// <summary>
    /// This class provides software-based pulse-width modulation (PWM) on any pin.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The period of the SoftPwm module defaults to approximately 100 Hz. Changing this period will affect all active SoftPwm pins.
    /// </para>
    /// <para>
    /// Compared to <see cref="HardwarePwm"/> (which is implemented in hardware), SoftPwm has relatively high jitter. However, it has relatively 
    /// good precision, fine-tuned period control, and works well even when many (or all!) Treehopper pins are used for SoftPwm. 
    /// </para>
    /// </remarks>
    public class SoftPwm : Pwm
    {
        private Pin pin;
        private TreehopperUsb board;
        private bool isEnabled;

        internal SoftPwm(TreehopperUsb board, Pin pin)
        {
            this.board = board;
            this.pin = pin;
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
                        board.SoftPwmMgr.StartPin(pin);
                        pin.Mode = PinMode.PushPullOutput;
                    }
                    else
                    {
                        board.SoftPwmMgr.StopPin(pin);
                        pin.Mode = PinMode.DigitalInput;
                    }
                }
            }
        }

        /// <summary>
        /// Get or set the duty cycle of the pin. This property has a range between 0.0 and 1.0, inclusive.
        /// </summary>
        /// <remarks>
        /// Due to software implementation constraints, the minimum duty cycle
        /// that will be generated is approximately 0.0003, and the maximum
        /// duty cycle will be 0.9993.
        /// </remarks>
        public double DutyCycle
        {
            get
            {
                return board.SoftPwmMgr.GetDutyCycle(pin);
            }

            set
            {
                if (value > 1.0 || value < 0.0)
                    throw new ArgumentOutOfRangeException("DutyCycle", "DutyCycle must be between 0.0 and 1.0");
                board.SoftPwmMgr.SetDutyCycle(pin, value);
            }
        }

        /// <summary>
        /// Get or set the pulse width, in microseconds, of the pin. This property has a range between 0 and 16409. 
        /// </summary>
        /// <remarks>
        /// Due to software implementation constraints, the minimum pulse width that will be generated is approximately
        /// 5.4 microseconds, and the maximum pulse width will be approximately 16,400. 
        /// </remarks>
        public double PulseWidth
        {
            get
            {
                return board.SoftPwmMgr.GetPulseWidth(pin);
            }

            set
            {
                if (value > 16409 || value < 0.0)
                    throw new ArgumentOutOfRangeException("PulseWidth", "PulseWidth must be between 0 and 16409");
                board.SoftPwmMgr.SetPulseWidth(pin, value);
            }
        }

        /// <summary>
        /// Gets a string representation of the soft-PWM pin's state
        /// </summary>
        /// <returns>The soft-PWM pin's state</returns>
        public override string ToString()
        {
            if (Enabled)
                return string.Format("{0:0.00}% duty cycle ({1:0.00} us pulse width)", DutyCycle * 100, PulseWidth);
            else
                return "Not enabled";
        }
    }
}
