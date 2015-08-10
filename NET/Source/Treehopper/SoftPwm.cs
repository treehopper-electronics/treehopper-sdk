namespace Treehopper
{
    /// <summary>
    /// This class provides software-based pulse-width modulation (PWM) on any pin.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The period of the SoftPwm module defaults to approximately 100 Hz. Changing this period will affect all active SoftPwm pins.
    /// </para>
    /// <para>
    /// Compared to <see cref="Pwm"/> (which is implemented in hardware), SoftPwm has relatively high jitter. However, it has relatively 
    /// good precision, fine-tuned period control, and works well even when many (or all!) Treehopper pins are used for SoftPwm. 
    /// </para>
    /// </remarks>
    public class SoftPwm : IPwm
    {
        Pin Pin;
        TreehopperUsb Board;
        bool isEnabled;
        internal SoftPwm(TreehopperUsb board, Pin pin)
        {
            this.Board = board;
            this.Pin = pin;
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
                if(value != isEnabled)
                {
                    isEnabled = value;
                    if(isEnabled)
                    {
                        Board.SoftPwmMgr.StartPin(Pin);
                        Pin.State = PinState.ReservedPin;
                    }
                    else
                    {
                        Board.SoftPwmMgr.StopPin(Pin);
                        Pin.MakeDigitalInput();
                    }
                }
            }
        }

        /// <summary>
        /// Get or set the duty cycle (0-1) of the pin
        /// </summary>
        public double DutyCycle
        {
            get { return Board.SoftPwmMgr.GetDutyCycle(Pin); }
            set { Board.SoftPwmMgr.SetDutyCycle(Pin, value); }
        }


        /// <summary>
        /// Get or set the pulse width, in milliseconds, of the pin
        /// </summary>
        public double PulseWidth
        {
            get { return Board.SoftPwmMgr.GetPulseWidth(Pin); }
            set { Board.SoftPwmMgr.SetPulseWidth(Pin, value); }
        }

        /// <summary>
        /// Gets or sets the current period for the SoftPwm module. This will affect all SoftPwm pins.
        /// </summary>
        public int Period
        {
            get
            {
                return Board.SoftPwmMgr.Period;
            }
            set{
                Board.SoftPwmMgr.Period = value;
            }

        }
        
    }
}
