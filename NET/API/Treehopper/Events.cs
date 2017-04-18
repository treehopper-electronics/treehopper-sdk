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
    /// <param name="e">The new value of the pin</param>
    public delegate void OnDigitalInValueChanged(object sender, DigitalInValueChangedEventArgs e);

    /// <summary>
    /// Used to send VoltageChanged events from the AnalogIn pin.
    /// </summary>
    /// <param name="sender">The AnalogIn pin that sent that message</param>
    /// <param name="e">The new voltage of the AnalogIn pin</param>
    public delegate void OnAnalogVoltageChanged(object sender, AnalogVoltageChangedEventArgs e);

    /// <summary>
    /// Used to send ValueChanged events from the AnalogIn pin.
    /// </summary>
    /// <param name="sender">The AnalogIn pin that sent that message</param>
    /// <param name="e">The new voltage of the AnalogIn pin</param>
    public delegate void OnAdcValueChanged(object sender, AdcValueChangedEventArgs e);

    /// <summary>
    /// Used to send ValueChanged events from the pin.
    /// </summary>
    /// <param name="sender">The pin that sent that message</param>
    /// <param name="e">The new normalized value of the pin</param>
    public delegate void OnAnalogValueChanged(object sender, AnalogValueChangedEventArgs e);

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

    /// <summary>
    /// Analog value changed EventArgs
    /// </summary>
    public class AnalogValueChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Construct a new Analog value changed EventArgs
        /// </summary>
        /// <param name="newValue">the new value</param>
        public AnalogValueChangedEventArgs(double newValue)
        {
            NewValue = newValue;
        }

        /// <summary>
        /// The new analog value
        /// </summary>
        public double NewValue { get; set; }
    }

    /// <summary>
    /// Analog voltage changed EventArgs
    /// </summary>
    public class AnalogVoltageChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Construct a new Analog voltage changed EventArgs
        /// </summary>
        /// <param name="newValue">the new value</param>
        public AnalogVoltageChangedEventArgs(double newValue)
        {
            NewValue = newValue;
        }

        /// <summary>
        /// The new analog voltage
        /// </summary>
        public double NewValue { get; set; }
    }

    /// <summary>
    /// ADC value changed EventArgs
    /// </summary>
    public class AdcValueChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Construct a new ADC value changed EventArgs
        /// </summary>
        /// <param name="newValue">the new value</param>
        public AdcValueChangedEventArgs(int newValue)
        {
            NewValue = newValue;
        }

        /// <summary>
        /// The new ADC value
        /// </summary>
        public int NewValue { get; set; }
    }
}
