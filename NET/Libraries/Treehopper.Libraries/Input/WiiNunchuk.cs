/** Built from documentation found at http://wiibrew.org/wiki/Wiimote/Extension_Controllers/Nunchuck
 */
namespace Treehopper.Libraries.Input
{
    using System.Numerics;
    using System.Threading.Tasks;
    using Sensors;
    using Sensors.Inertial;
    using System;
    using System.Linq;

    /// <summary>
    /// Supports Nintendo Wii Nunchuk controllers (and 3rd-party Wii-compatible Nunchuk controllers) attached via <see cref="I2c"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This library implements an <see cref="IAccelerometer"/>-, <see cref="DigitalInPin"/>-,  and <see cref="IPollable"/>-compatible input device that exposes the joystick, accelerometer, C button, and Z button on a Nintendo Wii (or compatible) Nunchuk controller. 
    /// </para>
    /// <para>
    /// Just like all <see cref="IPollable"/> devices, there are different options for getting the data out of the peripheral. By default, <see cref="AutoUpdateWhenPropertyRead"/> will be true, so the library will automatically fetch updates from the device on-the-fly whenever you read one of the object's properties --- this is a great method when you only need to grab the state of the device infrequently, as it doesn't tie up the <see cref="I2c"/> bus constantly. If your application is going to constantly read the input (say, in a game), you should change <see cref="AutoUpdateWhenPropertyRead"/> to false, and poll the sensor manually (with <see cref="Update()"/>) whenever you want an update. Or, if you're hooking into events for the device, you can wrap this class inside a <see cref="Poller{TPollable}"/> to automatically run the polling loop for you.
    /// </para>
    /// </remarks>
    public class WiiNunchuk : IAccelerometer
    {
        public class JoystickEventArgs : EventArgs
        {
            public Vector2 NewValue { get; set; }
        }

        public delegate void JoystickChangedDelegate(object sender, JoystickEventArgs e);

        Vector3 accelerometer;
        Vector2 joystick;
        WiiButton c;
        WiiButton z;
        SMBusDevice dev;
        private byte[] oldJoystickData = new byte[2] { 127, 128 }; // load with initial joystick "resting position" data so we don't fire changed events when we launch
        public WiiNunchuk(I2c i2c)
        {
            dev = new SMBusDevice(0x52, i2c);
            dev.WriteData(new byte[] { 0xF0, 0x55 }).Wait();
            dev.WriteData(new byte[] { 0xFB, 0x00 }).Wait();

            c = new WiiButton(this);
            z = new WiiButton(this);
        }

        public event JoystickChangedDelegate JoystickChanged;

        /// <summary>
        /// Gets a three-dimensional normalized vector representing the accelerometer state
        /// </summary>
        public Vector3 Accelerometer
        {
            get
            {
                if (AutoUpdateWhenPropertyRead) Update().Wait();
                return accelerometer;
            }
        }

        /// <summary>
        /// Gets a two-dimensional normalized vector representing the joystick state
        /// </summary>
        public Vector2 Joystick
        {
            get
            {
                if (AutoUpdateWhenPropertyRead) Update().Wait();
                return joystick;
            }
        }

        /// <summary>
        /// Gets the "C" button
        /// </summary>
        public DigitalInPin C { get { return c; } }

        /// <summary>
        /// Gets the "Z" button
        /// </summary>
        public DigitalInPin Z { get { return z; } }

        /// <summary>
        /// Whether reading from one of the data properties should trigger a call to <see cref="Update()"/> to fetch the latest state from the device. If false, the user should periodically call <see cref="Update()"/> to poll the device.
        /// </summary>
        public bool AutoUpdateWhenPropertyRead { get; set; } = true;

        /// <summary>
        /// The interval, in milliseconds, that "AwaitValueChanged"-type functions should use when polling this Nunchuk for updates.
        /// </summary>
        public int AwaitPollingInterval { get; set; } = 20;
        
        /// <summary>
        /// Fetch an update from the Nunchuk and push it to all the properties.
        /// </summary>
        /// <returns>An awaitable task that completes upon success.</returns>
        public async Task Update()
        {
            // the nunchuk does not respond to standard SMBus-compliant register reads, so we have to write a register, then do a new transaction to read out the data
            await dev.WriteByte(0x00);
            var response = await dev.ReadData(6);

            joystick.X = response[0] / 127.5f - 1f; // normalize joystick data to [-1, 1]
            joystick.Y = response[1] / 127.5f - 1f;

            if(!oldJoystickData.SequenceEqual(response.Take(2)))
            {
                JoystickChanged?.Invoke(this, new JoystickEventArgs() { NewValue = joystick });
            }

            Array.Copy(response, oldJoystickData, 2);

            accelerometer.X = 2*(short)(response[2] << 8 | ((response[5] >> 2) & 0x3) << 6) / (float)ushort.MaxValue; // normalize accelerometer data to [-1, 1]
            accelerometer.Y = 2*(short)(response[3] << 8 | ((response[5] >> 4) & 0x3) << 6) / (float)ushort.MaxValue;
            accelerometer.Z = 2*(short)(response[4] << 8 | ((response[5] >> 6) & 0x3) << 6) / (float)ushort.MaxValue;

            z.DigitalValue = (response[5] & 0x01) == 0;
            c.DigitalValue = (response[5] & 0x02) == 0;
        }

        /// <summary>
        /// A DigitalInPin-only implementation for the C and Z buttons
        /// </summary>
        private class WiiButton : DigitalInPin
        {
            private WiiNunchuk parent;
            TaskCompletionSource<bool> digitalSignal = new TaskCompletionSource<bool>();
            private bool digitalValue;
            internal WiiButton(WiiNunchuk parent)
            {
                this.parent = parent;
            }

            /// <summary>
            /// Gets the digital state of the button. "0" for "unpressed" and "1" for "pressed"
            /// </summary>
            public bool DigitalValue
            {
                get
                {
                    if (parent.AutoUpdateWhenPropertyRead) parent.Update().Wait();
                    return digitalValue;
                }

                internal set
                {
                    if (digitalValue == value) return;
                    digitalValue = value;
                    DigitalValueChanged?.Invoke((DigitalInPin)this, new DigitalInValueChangedEventArgs(digitalValue));
                    digitalSignal.TrySetResult(digitalValue);
                }
            }

            /// <summary>
            /// Fires when the button state changes
            /// </summary>
            public event OnDigitalInValueChanged DigitalValueChanged;

            /// <summary>
            /// Wait for the button to change state
            /// </summary>
            /// <remarks>
            /// <para>
            /// If the parent Nunchuk's <see cref="WiiNunchuk.AutoUpdateWhenPropertyRead"/> is "true", executing this function will start a Task that will poll the Nunchuk at an update rate specified by <see cref="WiiNunchuk.AwaitPollingInterval"/> until the button's state changes. Otherwise, this will return an awaitable (empty) task that will complete when the button state changes (the user is required to call <see cref="WiiNunchuk.Update()"/> periodically).
            /// </para>
            /// </remarks>
            /// <returns>The new state of the button</returns>
            public Task<bool> AwaitDigitalValueChange()
            {
                if (parent.AutoUpdateWhenPropertyRead)
                {
                    return Task.Run(async () =>
                    {
                        bool oldValue = digitalValue;
                        // poll the device
                        while (digitalValue == oldValue)
                        {
                            await parent.Update().ConfigureAwait(false);
                            await Task.Delay(parent.AwaitPollingInterval);
                        }

                        return digitalValue;
                    });
                }
                else
                {
                    digitalSignal = new TaskCompletionSource<bool>();
                    return digitalSignal.Task;
                }
            }

            /// <summary>
            /// Unused. Stub for DigitalInPin compliance.
            /// </summary>
            public void MakeDigitalIn()
            {
            }

            /// <summary>
            /// Returns "pressed" or "not pressed" depending on the button state. Note that this function will execute a read from the Nunchuk if AutoUpdatePropertyWhenRead is true.
            /// </summary>
            /// <returns>A string representing the button state</returns>
            public override string ToString()
            {
                if (DigitalValue)
                    return "Pressed";
                else
                    return "Not pressed";
            }
        }
    }
}
