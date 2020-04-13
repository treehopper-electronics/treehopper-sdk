namespace Treehopper
{
    internal class SoftPwmPinConfig
    {
        public Pin Pin { get; set; }

        /// <summary>
        ///     Duty Cycle, from 0 to 1.
        /// </summary>
        public double DutyCycle { get; set; }

        /// <summary>
        ///     Pulse Width, in Microseconds
        /// </summary>
        public double PulseWidthUs { get; set; }

        public bool UsePulseWidth { get; set; }

        public ushort Ticks { get; set; }
    }
}