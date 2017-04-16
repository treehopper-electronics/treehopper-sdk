﻿namespace Treehopper.Libraries.Input
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Pushbutton attached to DigitalIn
    /// </summary>
    public class Button
    {
        public class ButtonPressedEventArgs : EventArgs
        {
            public bool ButtonPressed { get; set; }
        }

        public class ButtonReleasedEventArgs : EventArgs
        {
            public bool ButtonReleased { get; set; }
        }

        public delegate void ButtonPressedEventHandler(object sender, ButtonPressedEventArgs e);
        public delegate void ButtonReleasedEventHandler(object sender, ButtonReleasedEventArgs e);

        private bool activeLow;

        /// <summary>
        /// Gets the underlying DigitalIn input.
        /// </summary>
        public DigitalIn Input { get; private set; }

        /// <summary>
        /// Construct a new Button attached to a DigitalIn input.
        /// </summary>
        /// <param name="input">The DigitalIn the button is attached to</param>
        /// <param name="activeLow">Whether the button is active-low (true, default). If the button is active-high, set to false.</param>
        public Button(DigitalIn input, bool activeLow = true)
        {
            this.Input = input;
            this.activeLow = activeLow;
            input.DigitalValueChanged += Input_DigitalValueChanged;
        }

        /// <summary>
        /// Determines whether the button is pressed (by reading Input.DigitalValue)
        /// </summary>
        public bool Pressed { get { return Input.DigitalValue ^ activeLow; } }

        /// <summary>
        /// Fired whenever the button is pressed
        /// </summary>
        public event ButtonPressedEventHandler OnPressed;

        /// <summary>
        /// Fired whenever the button is released
        /// </summary>
        public event ButtonReleasedEventHandler OnReleased;

        private void Input_DigitalValueChanged(object sender, DigitalInValueChangedEventArgs e)
        {
            if(e.NewValue ^ activeLow)
            {
                OnPressed?.Invoke(this, new ButtonPressedEventArgs() { ButtonPressed = true });
            } else
            {
                OnReleased?.Invoke(this, new ButtonReleasedEventArgs() { ButtonReleased = true });
            }
        }

        /// <summary>
        /// Wait for the button to change state
        /// </summary>
        /// <returns>An awaitable Task that returns a bool indicating if the button is pressed</returns>
        public async Task<bool> AwaitButtonChanged()
        {
            var newVal = await Input.AwaitDigitalValueChange().ConfigureAwait(false);
            return newVal ^ activeLow;
        }

        /// <summary>
        /// Gets a string representing of whether the button is pressed or not
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Pressed ? "Pressed" : "Not pressed";
        }
    }
}