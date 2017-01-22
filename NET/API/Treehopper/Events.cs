namespace Treehopper
{
    using System;

    /// <summary>
    /// An event handler for when a board is added
    /// </summary>
    /// <param name="BoardAdded">The new board</param>
    public delegate void TreehopperUsbAddedHandler(TreehopperUsb BoardAdded);

    /// <summary>
    /// An event handler for when a board is removed
    /// </summary>
    /// <param name="BoardRemoved">the removed board</param>
    public delegate void TreehopperUsbRemovedHandler(TreehopperUsb BoardRemoved);

    /// <summary>
    /// An event handler for when a new pin update report comes in
    /// </summary>
    /// <param name="sender">The board from where it originated</param>
    /// <param name="e">A ference to the new pin update report</param>
    public delegate void PinValuesUpdatedHandler(object sender, EventArgs e);

    /// <summary>
    /// This is the delegate prototype used for event-driven reading of digital pins.
    /// </summary>
    /// <param name="sender">The Pin that changed</param>
    /// <param name="value">The new value of the pin</param>
    public delegate void OnDigitalInValueChanged(object sender, DigitalInValueChangedEventArgs value);

    /// <summary>
    /// Used to send VoltageChanged events from the AnalogIn pin.
    /// </summary>
    /// <param name="sender">The AnalogIn pin that sent that message</param>
    /// <param name="voltage">The new voltage of the AnalogIn pin</param>
    public delegate void OnAnalogVoltageChanged(Pin sender, double voltage);

    /// <summary>
    /// Used to send ValueChanged events from the AnalogIn pin.
    /// </summary>
    /// <param name="sender">The AnalogIn pin that sent that message</param>
    /// <param name="value">The new voltage of the AnalogIn pin</param>
    public delegate void OnAdcValueChanged(Pin sender, int value);

    /// <summary>
    /// Used to send ValueChanged events from the pin.
    /// </summary>
    /// <param name="sender">The pin that sent that message</param>
    /// <param name="value">The new normalized value of the pin</param>
    public delegate void OnAnalogValueChanged(Pin sender, double value);

    /// <summary>
    /// An EventArgs that represents a digital input changing
    /// </summary>
    public class DigitalInValueChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Construct the event argument with the new digital value
        /// </summary>
        /// <param name="newValue">The new digital value</param>
        public DigitalInValueChangedEventArgs(bool newValue)
        {
            NewValue = newValue;
        }

        /// <summary>
        /// A property representing the new value
        /// </summary>
        public bool NewValue { get; set; }
    }
}
