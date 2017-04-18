namespace Treehopper
{
    using System;

    /// <summary>
    /// Hardware PWM manager
    /// </summary>
    public class HardwarePwmManager
    {
        private TreehopperUsb board;
        private PwmPinEnableMode mode;
        private HardwarePwmFrequency frequency = HardwarePwmFrequency.Freq_732Hz;

        private byte[] dutyCyclePin7 = new byte[2];
        private byte[] dutyCyclePin8 = new byte[2];
        private byte[] dutyCyclePin9 = new byte[2];

        internal HardwarePwmManager(TreehopperUsb treehopperUSB)
        {
            board = treehopperUSB;
        }

        private enum PwmPinEnableMode
        {
            None,
            Pin7,
            Pin7_Pin8,
            Pin7_Pin8_Pin9
        }

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
                switch (frequency)
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

        /// <summary>
        /// Gets a string representing the state of the Hardware PWM manager
        /// </summary>
        /// <returns>the state of the hardware PWM manager</returns>
        public override string ToString()
        {
            switch (mode)
            {
                case PwmPinEnableMode.Pin7:
                    return "channel 1 enabled";
                case PwmPinEnableMode.Pin7_Pin8:
                    return "channels 1 and 2 enabled";
                case PwmPinEnableMode.Pin7_Pin8_Pin9:
                    return "channels 1, 2, and 3 enabled";
                default:
                case PwmPinEnableMode.None:
                    return "No channels enabled";
            }
        }

        internal void StartPin(Pin pin)
        {
            // first check to make sure the previous PWM pins have been enabled first.
            if (pin.PinNumber == 8 && mode != PwmPinEnableMode.Pin7)
                throw new Exception("You must enable PWM functionality on Pin 8 (PWM1) before you enable PWM functionality on Pin 9 (PWM2). See http://treehopper.io/pwm");
            if (pin.PinNumber == 9 && mode != PwmPinEnableMode.Pin7_Pin8)
                throw new Exception("You must enable PWM functionality on Pin 8 and 9 (PWM1 and PWM2) before you enable PWM functionality on Pin 10 (PWM3). See http://treehopper.io/pwm");

            switch (pin.PinNumber)
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

        internal void StopPin(Pin pin)
        {
            // first check to make sure the higher PWM pins have been disabled first
            if (pin.PinNumber == 8 && mode != PwmPinEnableMode.Pin7_Pin8)
                throw new Exception("You must disable PWM functionality on Pin 10 (PWM3) before disabling Pin 9's PWM functionality. See http://treehopper.io/pwm");
            if (pin.PinNumber == 7 && mode != PwmPinEnableMode.Pin7)
                throw new Exception("You must disable PWM functionality on Pin 9 and 10 (PWM2 and PWM3) before disabling Pin 8's PWM functionality. See http://treehopper.io/pwm");

            switch (pin.PinNumber)
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

        internal void SetDutyCycle(Pin pin, double value)
        {
            var registerValue = (ushort)Math.Round(value * ushort.MaxValue);
            var newValue = BitConverter.GetBytes(registerValue); // TODO: is this endian-safe?
            switch (pin.PinNumber)
            {
                case 7:
                    dutyCyclePin7 = newValue;
                    break;
                case 8:
                    dutyCyclePin8 = newValue;
                    break;
                case 9:
                    dutyCyclePin9 = newValue;
                    break;
            }

            SendConfig();
        }

        internal void SendConfig()
        {
            var configuration = new byte[9];

            configuration[0] = (byte)DeviceCommands.PwmConfig;

            configuration[1] = (byte)mode;
            configuration[2] = (byte)frequency;

            configuration[3] = dutyCyclePin7[0];
            configuration[4] = dutyCyclePin7[1];

            configuration[5] = dutyCyclePin8[0];
            configuration[6] = dutyCyclePin8[1];

            configuration[7] = dutyCyclePin9[0];
            configuration[8] = dutyCyclePin9[1];

            board.SendPeripheralConfigPacket(configuration);
        }
    }
}
