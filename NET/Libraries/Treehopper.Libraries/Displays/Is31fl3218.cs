using System;
using System.Linq;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Displays
{
    /// <summary>
    ///     ISSI IS31FL3218 I2c 18-channel 16-bit PWM constant-current LED driver
    /// </summary>
    [Supports("ISSI", "IS31FL3218")]
    public class Is31fl3218 : LedDriver
    {
        private readonly bool[] currentStates = new bool[18];

        private readonly byte[] currentValues = new byte[18];
        private readonly SMBusDevice dev;

        private bool shutdown;

        /// <summary>
        ///     Construct a new IS31FL3218
        /// </summary>
        /// <param name="i2c">The I2C peripheral this chip is attached to</param>
        /// <param name="rateKhz">The frequency, in kHz, that should be used to communicate with the chip</param>
        public Is31fl3218(I2C i2c, int rateKhz = 100) : base(18, false, true)
        {
            dev = new SMBusDevice(0x54, i2c, rateKhz);
            dev.WriteByteData((byte) Registers.Shutdown, 0x01).Wait();
        }

        /// <summary>
        ///     Gets or sets whether the chip should be in shutdown mode.
        /// </summary>
        public bool Shutdown
        {
            get { return shutdown; }
            set
            {
                if (shutdown == value) return;
                shutdown = value;

                if (shutdown)
                    dev.WriteByteData((byte) Registers.Shutdown, 0x00).Wait();
                else
                    dev.WriteByteData((byte) Registers.Shutdown, 0x01).Wait();
            }
        }

        /// <summary>
        ///     Flush the data out to the LED driver
        /// </summary>
        /// <param name="force">Whether the data should be sent even if the data does not appear to have changed</param>
        /// <returns>An awaitable task</returns>
        public override Task Flush(bool force = false)
        {
            var states = new byte[3];
            for (var i = 0; i < currentStates.Length; i++)
            {
                var bit = i % 6;
                var theByte = i / 6;
                if (currentStates[i])
                    states[theByte] |= (byte) (1 << bit);
                else
                    states[theByte] &= (byte) ~(1 << bit);
            }

            var dataToWrite = currentValues.Concat(states).Concat(new byte[1] {0x00}).ToArray();
            return dev.WriteBufferData((byte) Registers.PwmBase, dataToWrite);
        }

        internal override void LedBrightnessChanged(Led led)
        {
            currentValues[led.Channel] = (byte) Math.Round(led.Brightness * 255);
            if (AutoFlush) Flush().Wait();
        }

        internal override void LedStateChanged(Led led)
        {
            currentStates[led.Channel] = led.State;
            if (AutoFlush) Flush().Wait();
        }

        internal override void SetGlobalBrightness(double brightness)
        {
        }

        private enum Registers
        {
            Shutdown = 0x00,
            PwmBase = 0x01,
            LedControl1 = 0x13,
            LedControl2 = 0x14,
            LedControl3 = 0x15,
            Update = 0x16,
            Reset = 0x17
        }
    }
}