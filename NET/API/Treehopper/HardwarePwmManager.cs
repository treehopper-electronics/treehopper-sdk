using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper
{
    
    /// <summary>
    /// Defines the PWM period options
    /// </summary>
    public enum HardwarePwmFrequency { 
        /// <summary>
        /// 732 Hz PWM frequency
        /// </summary>
        Freq_732Hz,

        /// <summary>
        /// 183 Hz PWM frequency
        /// </summary>
        Freq_183Hz,

        /// <summary>
        /// 61 Hz PWM frequency
        /// </summary>
        Freq_61Hz
    };

    /// <summary>
    /// Hardware PWM manager
    /// </summary>
    public class HardwarePwmManager
    {
        enum PwmPinEnableMode
        {
            None,
            Pin7,
            Pin7_Pin8,
            Pin7_Pin8_Pin9
        };

        private TreehopperUsb board;

        private PwmPinEnableMode mode;

        private byte[] DutyCyclePin7 = new byte[2];
        private byte[] DutyCyclePin8 = new byte[2];
        private byte[] DutyCyclePin9 = new byte[2];

        internal HardwarePwmManager(TreehopperUsb treehopperUSB)
        {
            this.board = treehopperUSB;
        }

        internal void StartPin(Pin Pin)
        {
            // first check to make sure the previous PWM pins have been enabled first.
            if(Pin.PinNumber == 8 & mode != PwmPinEnableMode.Pin7)
                throw new Exception("You must enable PWM functionality on Pin 8 (PWM1) before you enable PWM functionality on Pin 9 (PWM2). See http://treehopper.io/pwm");
            if (Pin.PinNumber == 9 & mode != PwmPinEnableMode.Pin7_Pin8)
                throw new Exception("You must enable PWM functionality on Pin 8 and 9 (PWM1 and PWM2) before you enable PWM functionality on Pin 10 (PWM3). See http://treehopper.io/pwm");
            
            switch(Pin.PinNumber)
            {
                case 7:
                    mode = PwmPinEnableMode.Pin7;
                    break;
                case 8:
                    mode = PwmPinEnableMode.Pin7_Pin8;
                    break;
                case 9:
                    mode = PwmPinEnableMode.Pin7_Pin8_Pin9;
                    break;
            }

            SendConfig();

        }

        internal void StopPin(Pin Pin)
        {
            // first check to make sure the higher PWM pins have been disabled first
            if (Pin.PinNumber == 8 & mode != PwmPinEnableMode.Pin7_Pin8)
                throw new Exception("You must disable PWM functionality on Pin 10 (PWM3) before disabling Pin 9's PWM functionality. See http://treehopper.io/pwm");
            if (Pin.PinNumber == 7 & mode != PwmPinEnableMode.Pin7)
                throw new Exception("You must disable PWM functionality on Pin 9 and 10 (PWM2 and PWM3) before disabling Pin 8's PWM functionality. See http://treehopper.io/pwm");

            switch (Pin.PinNumber)
            {
                case 7:
                    mode = PwmPinEnableMode.None;
                    break;
                case 8:
                    mode = PwmPinEnableMode.Pin7;
                    break;
                case 9:
                    mode = PwmPinEnableMode.Pin7_Pin8;
                    break;
            }

            SendConfig();
        }

        internal void SetDutyCycle(Pin Pin, double value)
        {
            UInt16 PwmRegisterValue = (UInt16)Math.Round((value) * UInt16.MaxValue);
            byte[] newValue = BitConverter.GetBytes(PwmRegisterValue);
            switch(Pin.PinNumber)
            {
                case 7:
                    DutyCyclePin7 = newValue;
                    break;
                case 8:
                    DutyCyclePin8 = newValue;
                    break;
                case 9:
                    DutyCyclePin9 = newValue;
                    break;
            }
            SendConfig();
        }

        private HardwarePwmFrequency frequency = HardwarePwmFrequency.Freq_732Hz;
        /// <summary>
        /// Gets or sets the PWM frequency of the pin, selected from <see cref="HardwarePwmFrequency"/>
        /// </summary>
        public HardwarePwmFrequency Frequency
        {
            get
            {
                return frequency;
            }
            set
            {
                if (frequency != value)
                {
                    frequency = value;
                    SendConfig();
                }
            }
        }

        /// <summary>
        /// Get the number of microseconds per tick
        /// </summary>
        public double MicrosecondsPerTick
        {
            get
            {
                return 1000000 / (FrequencyHz * 65536);
            }
        }

        /// <summary>
        /// Get the number of microseconds per period
        /// </summary>
        public double PeriodMicroseconds
        {
            get
            {
                return 1000000 / FrequencyHz;
            }
        }

        /// <summary>
        /// Get an integer representing the current PWM frequency
        /// </summary>
        public int FrequencyHz
        {
            get
            {
                switch(frequency)
                {
                    case HardwarePwmFrequency.Freq_183Hz:
                        return 183;
                    case HardwarePwmFrequency.Freq_61Hz:
                        return 61;
                    case HardwarePwmFrequency.Freq_732Hz:
                        return 732;
                    default:
                        return 0;
                }
            }
        }


        internal void SendConfig()
        {
            byte[] configuration = new byte[64];

            configuration[0] = (byte)DeviceCommands.PwmConfig;

            configuration[1] = (byte)mode;
            configuration[2] = (byte)frequency;

            configuration[3] = DutyCyclePin7[0];
            configuration[4] = DutyCyclePin7[1];

            configuration[5] = DutyCyclePin8[0];
            configuration[6] = DutyCyclePin8[1];

            configuration[7] = DutyCyclePin9[0];
            configuration[8] = DutyCyclePin9[1];

            board.sendPeripheralConfigPacket(configuration);
        }
    }
}
