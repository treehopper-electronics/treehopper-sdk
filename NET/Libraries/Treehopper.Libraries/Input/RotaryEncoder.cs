using System;
using Treehopper.Utilities;

namespace Treehopper.Libraries.Input
{
    /// <summary>
    ///     Quadrature rotary encoder
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This class is designed to interface with standard, two-channel quadrature-style incremental rotary encoders.
    ///         These are often used as infinite-turn knobs for human interfaces, motor feedback, measuring wheels, and other
    ///         rotary/linear motion measurement applications.
    ///     </para>
    ///     <para>
    ///         If you are using an encoder with an detent, you can set the stepsPerTick parameter to match your encoder so
    ///         that <see cref="Position" /> will increment/decrement for each "click" (for many encoders, a value of 4 is
    ///         correct). For maximum resolution, leave at 1.
    ///     </para>
    ///     <para>
    ///         This class makes use of <see cref="DigitalIn" />'s <see cref="DigitalIn.DigitalValueChanged" /> event. If
    ///         you're attaching the encoder to an I2c or SPI I/O expander or shift register, make sure your driver is sampling
    ///         these pins often.
    ///     </para>
    ///     <para>
    ///         Because the inputs are sampled over USB by the host API, only low to moderate-speed applications are
    ///         supported; while Treehopper has been tested at input speeds in the 5-10 kHz range, the nondeterministic nature
    ///         of Treehopper means it is unlikely that this class will work reliably above 500-1000 Hz, and will occasionally
    ///         miss clicks.
    ///     </para>
    ///     <para>
    ///         While earlier versions of this library used a 32-signed <see cref="Position" /> value, this would overflow
    ///         after less than 25 days with a 1 kHz input signal, which could be problematic for some use cases. As such, this
    ///         is now stored as a long (64-bit) signed value. With a 1 kHz signal, this will overflow after approximately 292
    ///         million years, which should be sufficient for most users.
    ///     </para>
    /// </remarks>
    public class RotaryEncoder
    {
        /// <summary>
        ///     Rotary encoder position changed delegate
        /// </summary>
        /// <param name="sender">The RotaryEncoder where this message originated from</param>
        /// <param name="e">The EventArgs this RotaryEncoder generated</param>
        public delegate void PositionChangedDelegate(object sender, PositionChangedEventArgs e);

        private readonly DigitalIn a;
        private readonly DigitalIn b;
        private readonly int stepsPerTick;
        private long oldPosition;
        private long position;

        /// <summary>
        ///     Construct a new RotaryEncoder from two digital inputs
        /// </summary>
        /// <param name="a">A channel input</param>
        /// <param name="b">B channel input</param>
        /// <param name="stepsPerTick">The number of steps to count as one "tick"</param>
        public RotaryEncoder(DigitalIn a, DigitalIn b, int stepsPerTick = 1)
        {
            this.stepsPerTick = stepsPerTick;
            this.a = a;
            this.b = b;

            a.MakeDigitalIn();
            b.MakeDigitalIn();

            Position = 0;

            a.DigitalValueChanged += A_DigitalValueChanged;
            b.DigitalValueChanged += B_DigitalValueChanged;
        }

        /// <summary>
        ///     Gets or sets the Position counter.
        /// </summary>
        public long Position
        {
            set { position = value * stepsPerTick; }
            get { return position / stepsPerTick; }
        }

        /// <summary>
        ///     The maximum value this encoder should report
        /// </summary>
        public long MaxValue { get; set; } = long.MaxValue;

        /// <summary>
        ///     The minimum value this encoder should report
        /// </summary>
        public long MinValue { get; set; } = long.MinValue;

        private void B_DigitalValueChanged(object sender, DigitalInValueChangedEventArgs e)
        {
            if (b.DigitalValue && a.DigitalValue)
                position--;
            else if (b.DigitalValue && !a.DigitalValue)
                position++;
            else if (!b.DigitalValue && a.DigitalValue)
                position++;
            else if (!b.DigitalValue && !a.DigitalValue)
                position--;
            updatePosition();
        }

        private void A_DigitalValueChanged(object sender, DigitalInValueChangedEventArgs e)
        {
            // A changed
            if (b.DigitalValue && a.DigitalValue)
                position++;
            else if (b.DigitalValue && !a.DigitalValue)
                position--;
            else if (!b.DigitalValue && a.DigitalValue)
                position--;
            else if (!b.DigitalValue && !a.DigitalValue)
                position++;
            updatePosition();
        }

        /// <summary>
        ///     Fires whenever the position has changed
        /// </summary>
        public event PositionChangedDelegate PositionChanged;

        private void updatePosition()
        {
            if (position % stepsPerTick == 0)
            {
                Position = Numbers.Constrain(Position, MinValue, MaxValue);

                if (Position == oldPosition) return;

                PositionChanged?.Invoke(this, new PositionChangedEventArgs {NewPosition = Position});
                oldPosition = Position;
            }
        }

        /// <summary>
        ///     Rotary encoder position changed event arguments
        /// </summary>
        public class PositionChangedEventArgs : EventArgs
        {
            /// <summary>
            ///     The new position of the rotary encoder
            /// </summary>
            public long NewPosition { get; set; }
        }
    }
}