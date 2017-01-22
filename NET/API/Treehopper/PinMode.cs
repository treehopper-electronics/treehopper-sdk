namespace Treehopper
{
    /// <summary>
    /// An enumeration representing the different supported modes of this pin.
    /// </summary>
    public enum PinMode
    {
        /// <summary>
        /// Pin is reserved for other use
        /// </summary>
        Reserved,

        /// <summary>
        /// Pin is a digital input
        /// </summary>
        DigitalInput,

        /// <summary>
        /// Pin is a push-pull output
        /// </summary>
        PushPullOutput,

        /// <summary>
        /// Pin is an open-drain output
        /// </summary>
        OpenDrainOutput,

        /// <summary>
        /// Pin is an analog input
        /// </summary>
        AnalogInput,

        /// <summary>
        /// Pin is unassigned
        /// </summary>
        Unassigned
    }
}
