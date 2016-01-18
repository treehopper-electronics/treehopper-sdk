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
    public enum PwmFrequency { 
        Freq_732Hz,
        Freq_183Hz,
        Freq_61Hz
    };

    public class PwmManager
    {

        enum PwmPinEnableMode
        {
            None,
            Pin8,
            Pin8_Pin9,
            Pin8_Pin9_Pin10
        };

        private TreehopperUsb board;

        private PwmPinEnableMode mode;

        private byte[] DutyCyclePin8 = new byte[2];
        private byte[] DutyCyclePin9 = new byte[2];
        private byte[] DutyCyclePin10 = new byte[2];

        public PwmManager(TreehopperUsb treehopperUSB)
        {
            this.board = treehopperUSB;
        }

        internal void StartPin(Pin Pin)
        {
            // first check to make sure the previous PWM pins have been enabled first.
            if(Pin.PinNumber == 9 & mode != PwmPinEnableMode.Pin8)
                throw new Exception("You must enable PWM functionality on Pin 8 (PWM1) before you enable PWM functionality on Pin 9 (PWM2). See http://treehopper.io/pwm");
            if (Pin.PinNumber == 10 & mode != PwmPinEnableMode.Pin8_Pin9)
                throw new Exception("You must enable PWM functionality on Pin 8 and 9 (PWM1 and PWM2) before you enable PWM functionality on Pin 10 (PWM3). See http://treehopper.io/pwm");
            
            switch(Pin.PinNumber)
            {
                case 8:
                    mode = PwmPinEnableMode.Pin8;
                    break;
                case 9:
                    mode = PwmPinEnableMode.Pin8_Pin9;
                    break;
                case 10:
                    mode = PwmPinEnableMode.Pin8_Pin9_Pin10;
                    break;
            }

            SendConfig();

        }

        internal void StopPin(Pin Pin)
        {
            // first check to make sure the higher PWM pins have been disabled first
            if (Pin.PinNumber == 9 & mode != PwmPinEnableMode.Pin8_Pin9)
                throw new Exception("You must disable PWM functionality on Pin 10 (PWM3) before disabling Pin 9's PWM functionality. See http://treehopper.io/pwm");
            if (Pin.PinNumber == 8 & mode != PwmPinEnableMode.Pin8)
                throw new Exception("You must disable PWM functionality on Pin 9 and 10 (PWM2 and PWM3) before disabling Pin 8's PWM functionality. See http://treehopper.io/pwm");
            
        }

        internal void SetDutyCycle(Pin Pin, double value)
        {
            UInt16 PwmRegisterValue = (UInt16)Math.Round((value) * UInt16.MaxValue);
            byte[] newValue = BitConverter.GetBytes(PwmRegisterValue);
            switch(Pin.PinNumber)
            {
                case 8:
                    DutyCyclePin8 = newValue;
                    break;
                case 9:
                    DutyCyclePin9 = newValue;
                    break;
                case 10:
                    DutyCyclePin10 = newValue;
                    break;
            }
            SendConfig();
        }

        private PwmFrequency frequency = PwmFrequency.Freq_732Hz;
        /// <summary>
        /// Gets or sets the PWM frequency of the pin, selected from <see cref="PwmFrequency"/>
        /// </summary>
        public PwmFrequency Frequency
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

        public double MicrosecondsPerTick
        {
            get
            {
                return 1000000 / (FrequencyHertz * 65536);
            }
        }

        public double PeriodMicroseconds
        {
            get
            {
                return 1000000 / FrequencyHertz;
            }
        }

        public int FrequencyHertz
        {
            get
            {
                switch(frequency)
                {
                    case PwmFrequency.Freq_183Hz:
                        return 183;
                    case PwmFrequency.Freq_61Hz:
                        return 61;
                    case PwmFrequency.Freq_732Hz:
                        return 732;
                    default:
                        return 0;
                }
            }
        }

        public void SendConfig()
        {
            byte[] configuration = new byte[64];

            configuration[0] = (byte)DeviceCommands.PwmConfig;

            configuration[1] = (byte)mode;
            configuration[2] = (byte)frequency;

            configuration[3] = DutyCyclePin8[0];
            configuration[4] = DutyCyclePin8[1];

            configuration[5] = DutyCyclePin9[0];
            configuration[6] = DutyCyclePin9[1];

            configuration[7] = DutyCyclePin10[0];
            configuration[8] = DutyCyclePin10[1];

            board.sendPeripheralConfigPacket(configuration);
        }
    }
}
