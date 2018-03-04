using System.Threading.Tasks;

namespace Treehopper
{
    /// <summary>
    ///     Base interface that describes a pin with a DigitalValue property
    /// </summary>
    public interface DigitalBase
    {
        /// <summary>
        ///     Get or set the DigitalValue of the pin
        /// </summary>
        bool DigitalValue { get; }
    }

    /// <summary>
    ///     Interface for digital output pins
    /// </summary>
    public interface DigitalOut : DigitalBase
    {
        /// <summary>
        ///     Get or set the digital value of the pin
        /// </summary>
        new bool DigitalValue { get; set; }

        /// <summary>
        ///     Toggle the output's value
        /// </summary>
        Task ToggleOutputAsync();

        /// <summary>
        ///     Make the pin a push-pull output
        /// </summary>
        Task MakeDigitalPushPullOutAsync();
    }

    /// <summary>
    ///     Interface for digital pins supporting input functionality
    /// </summary>
    public interface DigitalIn : DigitalBase
    {
        /// <summary>
        ///     Fires whenever the digital pin's input changes value
        /// </summary>
        event OnDigitalInValueChanged DigitalValueChanged;

        /// <summary>
        ///     Awaits until the digital value changes
        /// </summary>
        /// <returns>An awaitable bool of the new value</returns>
        Task<bool> AwaitDigitalValueChangeAsync();

        /// <summary>
        ///     Make the pin a digital input
        /// </summary>
        Task MakeDigitalInAsync();
    }

    /// <summary>
    ///     Helper interface to describe pins that are both inputs and outputs
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public interface DigitalIO : DigitalIn, DigitalOut
    {
    }
}