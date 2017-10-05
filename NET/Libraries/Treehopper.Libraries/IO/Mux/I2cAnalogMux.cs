using System.Collections;

namespace Treehopper.Libraries.IO.Mux
{
    /// <summary>
    ///     Use a standard analog mux as a I2c mux
    /// </summary>
    public class I2cAnalogMux : I2cMux
    {
        private readonly DigitalOut[] pins;

        /// <summary>
        ///     Construct an <see cref="I2cMux" /> using a standard 4052-style two-bit 4:1 mux.
        /// </summary>
        /// <param name="MuxedPort">The upstream port to mux</param>
        /// <param name="pins">The pin(s) to use to control the mux, starting with the least-significant bit</param>
        public I2cAnalogMux(I2C MuxedPort, params DigitalOut[] pins) : base(MuxedPort, 1 << pins.Length)
        {
            this.pins = pins;
            foreach (var pin in pins)
                pin.MakeDigitalPushPullOut();
        }

        /// <summary>
        ///     Set the mux
        /// </summary>
        /// <param name="index">The index to be muxed</param>
        protected override void setMux(int index)
        {
            var array = new BitArray(new[] {(byte) index});
            for (var i = 0; i < pins.Length; i++)
                pins[i].DigitalValue = array[i];
        }
    }
}