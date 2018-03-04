using System;
using System.Threading.Tasks;
using Treehopper.Utilities;

namespace Treehopper
{
    /// <summary>
    ///     Hardware PWM manager
    /// </summary>
    public class HardwarePwmManager
    {
        private readonly TreehopperUsb _board;

        private byte[] _dutyCyclePin7 = new byte[2];
        private byte[] _dutyCyclePin8 = new byte[2];
        private byte[] _dutyCyclePin9 = new byte[2];
        private HardwarePwmFrequency _frequency = HardwarePwmFrequency.Freq732Hz;
        private PwmPinEnableMode _mode;

        internal HardwarePwmManager(TreehopperUsb treehopperUsb)
        {
            _board = treehopperUsb;
        }

        /// <summary>
        ///     Gets or sets the PWM frequency of the pin, selected from <see cref="HardwarePwmFrequency" />
        /// </summary>
        public HardwarePwmFrequency Frequency
        {
            get { return _frequency; }

            set
            {
                if (_frequency != value)
                {
                    _frequency = value;

                    if (TreehopperUsb.Settings.PropertyWritesReturnImmediately)
                        SendConfigAsync().Forget();
                    else
                        Task.Run(SendConfigAsync).Wait();
                }
            }
        }

        /// <summary>
        ///     Get the number of microseconds per tick
        /// </summary>
        public double MicrosecondsPerTick => 1000000d / (FrequencyHz * 65536);

        /// <summary>
        ///     Get the number of microseconds per period
        /// </summary>
        public double PeriodMicroseconds => 1000000d / FrequencyHz;

        /// <summary>
        ///     Get an integer representing the current PWM frequency
        /// </summary>
        public int FrequencyHz
        {
            get
            {
                switch (_frequency)
                {
                    case HardwarePwmFrequency.Freq183Hz:
                        return 183;
                    case HardwarePwmFrequency.Freq61Hz:
                        return 61;
                    case HardwarePwmFrequency.Freq732Hz:
                        return 732;
                    default:
                        return 0;
                }
            }
        }

        /// <summary>
        ///     Gets a string representing the state of the Hardware PWM manager
        /// </summary>
        /// <returns>the state of the hardware PWM manager</returns>
        public override string ToString()
        {
            switch (_mode)
            {
                case PwmPinEnableMode.Pin7:
                    return "channel 1 enabled";
                case PwmPinEnableMode.Pin7Pin8:
                    return "channels 1 and 2 enabled";
                case PwmPinEnableMode.Pin7Pin8Pin9:
                    return "channels 1, 2, and 3 enabled";
                default:
                    return "No channels enabled";
            }
        }

        internal Task StartPinAsync(Pin pin)
        {
            // first check to make sure the previous PWM pins have been enabled first.
            if (pin.PinNumber == 8 && _mode != PwmPinEnableMode.Pin7)
                throw new Exception(
                    "You must enable PWM functionality on Pin 7 (PWM1) before you enable PWM functionality on Pin 8 (PWM2).");
            if (pin.PinNumber == 9 && _mode != PwmPinEnableMode.Pin7Pin8)
                throw new Exception(
                    "You must enable PWM functionality on Pin 7 and 8 (PWM1 and PWM2) before you enable PWM functionality on Pin 9 (PWM3).");

            switch (pin.PinNumber)
            {
                case 7:
                    _mode = PwmPinEnableMode.Pin7;
                    break;
                case 8:
                    _mode = PwmPinEnableMode.Pin7Pin8;
                    break;
                case 9:
                    _mode = PwmPinEnableMode.Pin7Pin8Pin9;
                    break;
            }

            return SendConfigAsync();
        }

        internal Task StopPinAsync(Pin pin)
        {
            // first check to make sure the higher PWM pins have been disabled first
            if (pin.PinNumber == 8 && _mode != PwmPinEnableMode.Pin7Pin8)
                throw new Exception(
                    "You must disable PWM functionality on Pin 9 (PWM3) before disabling Pin 8's PWM functionality.");
            if (pin.PinNumber == 7 && _mode != PwmPinEnableMode.Pin7)
                throw new Exception(
                    "You must disable PWM functionality on Pin 8 and 9 (PWM2 and PWM3) before disabling Pin 7's PWM functionality.");

            switch (pin.PinNumber)
            {
                case 7:
                    _mode = PwmPinEnableMode.None;
                    break;
                case 8:
                    _mode = PwmPinEnableMode.Pin7;
                    break;
                case 9:
                    _mode = PwmPinEnableMode.Pin7Pin8;
                    break;
            }

            return SendConfigAsync();
        }

        internal Task SetDutyCycleAsync(Pin pin, double value)
        {
            var registerValue = (ushort) Math.Round(value * ushort.MaxValue);
            var newValue = BitConverter.GetBytes(registerValue); // TODO: is this endian-safe?
            switch (pin.PinNumber)
            {
                case 7:
                    _dutyCyclePin7 = newValue;
                    break;
                case 8:
                    _dutyCyclePin8 = newValue;
                    break;
                case 9:
                    _dutyCyclePin9 = newValue;
                    break;
            }

            return SendConfigAsync();
        }

        internal Task SendConfigAsync()
        {
            var configuration = new byte[9];

            configuration[0] = (byte) DeviceCommands.PwmConfig;

            configuration[1] = (byte) _mode;
            configuration[2] = (byte) _frequency;

            configuration[3] = _dutyCyclePin7[0];
            configuration[4] = _dutyCyclePin7[1];

            configuration[5] = _dutyCyclePin8[0];
            configuration[6] = _dutyCyclePin8[1];

            configuration[7] = _dutyCyclePin9[0];
            configuration[8] = _dutyCyclePin9[1];

            return _board.SendPeripheralConfigPacketAsync(configuration);
        }

        private enum PwmPinEnableMode
        {
            None,
            Pin7,
            Pin7Pin8,
            Pin7Pin8Pin9
        }
    }
}