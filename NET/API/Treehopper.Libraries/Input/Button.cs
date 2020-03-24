using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Input
{
    /// <summary>
    ///     Pushbutton attached to DigitalIn
    /// </summary>
    public class Button : INotifyPropertyChanged
    {
        public delegate void ButtonPressedEventHandler(object sender, ButtonPressedEventArgs e);

        public delegate void ButtonReleasedEventHandler(object sender, ButtonReleasedEventArgs e);

        private readonly bool activeLow;

        /// <summary>
        ///     Construct a new Button attached to a DigitalIn input.
        /// </summary>
        /// <param name="input">The DigitalIn the button is attached to</param>
        /// <param name="activeLow">Whether the button is active-low (true, default). If the button is active-high, set to false.</param>
        public Button(DigitalIn input, bool activeLow = true)
        {
            Input = input;
            this.activeLow = activeLow;
            input.DigitalValueChanged += Input_DigitalValueChanged;
            input.MakeDigitalInAsync();
        }

        /// <summary>
        ///     Gets the underlying DigitalIn input.
        /// </summary>
        public DigitalIn Input { get; }

        /// <summary>
        ///     Determines whether the button is pressed (by reading Input.DigitalValue)
        /// </summary>
        public bool Pressed => Input.DigitalValue ^ activeLow;

        /// <summary>
        ///     Fired whenever the button is pressed
        /// </summary>
        public event ButtonPressedEventHandler OnPressed;

        /// <summary>
        ///     Fired whenever the button is released
        /// </summary>
        public event ButtonReleasedEventHandler OnReleased;
        public event PropertyChangedEventHandler PropertyChanged;

        private void Input_DigitalValueChanged(object sender, DigitalInValueChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Pressed"));
            if (e.NewValue ^ activeLow)
                OnPressed?.Invoke(this, new ButtonPressedEventArgs {ButtonPressed = true});
            else
                OnReleased?.Invoke(this, new ButtonReleasedEventArgs {ButtonReleased = true});
        }

        /// <summary>
        ///     Wait for the button to change state
        /// </summary>
        /// <returns>An awaitable Task that returns a bool indicating if the button is pressed</returns>
        public async Task<bool> AwaitButtonChangedAsync()
        {
            var newVal = await Input.AwaitDigitalValueChangeAsync().ConfigureAwait(false);
            return newVal ^ activeLow;
        }

        /// <summary>
        ///     Gets a string representing of whether the button is pressed or not
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Pressed ? "Pressed" : "Not pressed";
        }

        public class ButtonPressedEventArgs : EventArgs
        {
            public bool ButtonPressed { get; set; }
        }

        public class ButtonReleasedEventArgs : EventArgs
        {
            public bool ButtonReleased { get; set; }
        }
    }
}