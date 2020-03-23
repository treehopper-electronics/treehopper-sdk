using System;
using System.Linq;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Displays
{
    /// <summary>
    ///     ISSI IS31FL3236 I2c 36-channel 8-bit PWM constant-current LED driver
    /// </summary>
    [Supports("ISSI", "IS31FL3236")]
    public class Is31fl3236 : LedDriver
    {
        /// <summary>
        /// The possible values to assign to each Currents element
        /// </summary>
        public enum Current
        {
            /// <summary>
            /// The maximum current
            /// </summary>
            IMax,

            /// <summary>
            /// Half of Imax
            /// </summary>
            HalfImax,

            /// <summary>
            /// One third of Imax
            /// </summary>
            ThirdImax,

            /// <summary>
            /// One fourth of Imax
            /// </summary>
            QuarterImax
        }

        private bool[] states = new bool[36];
        private byte[] values = new byte[36];
        private readonly SMBusDevice dev;
        private bool shutdown;

        /// <summary>
        /// Gets or sets the current for each channel (defaults to Imax)
        /// </summary>
        public Current[] Currents = new Current[36];

        /// <summary>
        ///     Construct a new IS31FL3236
        /// </summary>
        /// <param name="i2c">The I2C peripheral this chip is attached to</param>
        /// <param name="rateKhz">The frequency, in kHz, that should be used to communicate with the chip</param>
        /// <param name="sdb">The (optional) hardware shutdown pin, SDB</param>
        public Is31fl3236(I2C i2c, int rateKhz = 100, bool ad0 = false, DigitalOut sdb = null) : base(36, false, true)
        {
            if (sdb != null)
            {
                sdb.DigitalValue = true;
            }
            dev = new SMBusDevice(0x3C, i2c, rateKhz);
            dev.WriteByteDataAsync((byte) Registers.Shutdown, 0x01).Wait();

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
                    dev.WriteByteDataAsync((byte) Registers.Shutdown, 0x00).Wait();
                else
                    dev.WriteByteDataAsync((byte) Registers.Shutdown, 0x01).Wait();
            }
        }

        /// <summary>
        ///     Flush the data out to the LED driver
        /// </summary>
        /// <param name="force">Whether the data should be sent even if the data does not appear to have changed</param>
        /// <returns>An awaitable task</returns>
        public override Task FlushAsync(bool force = false)
        {
            var controls = new byte[36];
            for (var i = 0; i < states.Length; i++)
            {
                controls[i] = (byte)((((int)Currents[i]) << 1) | (states[i] ? 1 : 0));
            }

            // send the 36 PWM registers, followed by the PWM update register (0x25), followed by the 36 LED control registers, followed by the global control register
            var dataToWrite = values.Concat(new byte[] { 0x00 }).Concat(controls).Concat(new byte[1] {0x00}).ToArray();
            return dev.WriteBufferDataAsync((byte) Registers.PwmBase, dataToWrite);
        }

        internal override void LedBrightnessChanged(Led led)
        {
            values[led.Channel] = (byte) Math.Round(led.Brightness * 255);
            if (AutoFlush) FlushAsync().Wait();
        }

        internal override void LedStateChanged(Led led)
        {
            states[led.Channel] = led.State;
            if (AutoFlush) FlushAsync().Wait();
        }

        internal override void SetGlobalBrightness(double brightness)
        {
        }

        private enum Registers
        {
            Shutdown = 0x00,
            PwmBase = 0x01
        }
    }
}