namespace Treehopper
{
    using System.Threading.Tasks;

    /// <summary>
    /// Base interface that describes a pin with a DigitalValue property
    /// </summary>
    public interface DigitalPinBase
    {
        /// <summary>
        /// Get or set the DigitalValue of the pin
        /// </summary>
        bool DigitalValue { get; set; }
    }

    /// <summary>
    /// Interface for digital output pins
    /// </summary>
    public interface DigitalOutPin : DigitalPinBase
    {
        /// <summary>
        /// Toggle the output's value
        /// </summary>
        Task ToggleOutputAsync();

        /// <summary>
        /// Make the pin a push-pull output
        /// </summary>
        Task MakeDigitalPushPullOut();
    }

    /// <summary>
    /// Interface for digital pins supporting input functionality
    /// </summary>
    public interface DigitalInPin : DigitalPinBase
    {
        /// <summary>
        /// Fires whenever the digital pin's input changes value
        /// </summary>
        event OnDigitalInValueChanged DigitalValueChanged;

        /// <summary>
        /// Awaits until the digital value changes
        /// </summary>
        /// <returns>An awaitable bool of the new value</returns>
        Task<bool> AwaitDigitalValueChange();

        /// <summary>
        /// Make the pin a digital input
        /// </summary>
        Task MakeDigitalIn();
    }

    /// <summary>
    /// Helper interface to describe pins that are both inputs and outputs
    /// </summary>
    public interface DigitalIOPin : DigitalInPin, DigitalOutPin
    {
    }
}
