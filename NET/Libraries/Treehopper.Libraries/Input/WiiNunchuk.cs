/** Built from documentation found at http://wiibrew.org/wiki/Wiimote/Extension_Controllers/Nunchuck
 */

using System;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Treehopper.Libraries.IO.PortExpander;
using Treehopper.Libraries.Sensors;
using Treehopper.Libraries.Sensors.Inertial;

namespace Treehopper.Libraries.Input
{
    /// <summary>
    ///     Nintendo Wii Nunchuk
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This library implements an <see cref="IAccelerometer" />-, <see cref="DigitalIn" />-,  and
    ///         <see cref="IPollable" />-compatible input device that exposes the joystick, accelerometer, C button, and Z
    ///         button on a Nintendo Wii (or compatible) Nunchuk controller.
    ///     </para>
    ///     <para>
    ///         Just like all <see cref="IPollable" /> devices, there are different options for getting the data out of the
    ///         peripheral. By default, <see cref="AutoUpdateWhenPropertyRead" /> will be true, so the library will
    ///         automatically fetch updates from the device on-the-fly whenever you read one of the object's properties ---
    ///         this is a great method when you only need to grab the state of the device infrequently, as it doesn't tie up
    ///         the <see cref="I2C" /> bus constantly. If your application is going to constantly read the input (say, in a
    ///         game), you should change <see cref="AutoUpdateWhenPropertyRead" /> to false, and poll the sensor manually (with
    ///         <see cref="UpdateAsync()" />) whenever you want an update. Or, if you're hooking into events for the device, you can
    ///         wrap this class inside a <see cref="Poller{TPollable}" /> to automatically run the polling loop for you.
    ///     </para>
    /// </remarks>
    [Supports("Nintendo", "Wii Nunchuk")]
    public class WiiNunchuk : IAccelerometer
    {
        public delegate void JoystickChangedDelegate(object sender, JoystickEventArgs e);

        private readonly SMBusDevice dev;

        private readonly byte[] oldJoystickData =
                new byte[2] {127, 128}
            ; // load with initial joystick "resting position" data so we don't fire changed events when we launch

        private Vector3 accelerometer;
        private Vector2 joystick;

        public WiiNunchuk(I2C i2c)
        {
            dev = new SMBusDevice(0x52, i2c);
            dev.WriteData(new byte[] {0xF0, 0x55}).Wait();
            dev.WriteData(new byte[] {0xFB, 0x00}).Wait();

            C = new Button(new DigitalInPeripheralPin(this), false);
            Z = new Button(new DigitalInPeripheralPin(this), false);
        }

        /// <summary>
        ///     Gets a two-dimensional normalized vector representing the joystick state
        /// </summary>
        public Vector2 Joystick
        {
            get
            {
                if (AutoUpdateWhenPropertyRead) UpdateAsync().Wait();
                return joystick;
            }
        }

        /// <summary>
        ///     Gets the "C" button
        /// </summary>
        public Button C { get; }

        /// <summary>
        ///     Gets the "Z" button
        /// </summary>
        public Button Z { get; }

        /// <summary>
        ///     Gets a three-dimensional normalized vector representing the accelerometer state
        /// </summary>
        public Vector3 Accelerometer
        {
            get
            {
                if (AutoUpdateWhenPropertyRead) UpdateAsync().Wait();
                return accelerometer;
            }
        }

        /// <summary>
        ///     Whether reading from one of the data properties should trigger a call to <see cref="UpdateAsync()" /> to fetch the
        ///     latest state from the device. If false, the user should periodically call <see cref="UpdateAsync()" /> to poll the
        ///     device.
        /// </summary>
        public bool AutoUpdateWhenPropertyRead { get; set; } = true;

        /// <summary>
        ///     The interval, in milliseconds, that "AwaitValueChanged"-type functions should use when polling this Nunchuk for
        ///     updates.
        /// </summary>
        public int AwaitPollingInterval { get; set; } = 20;

        /// <summary>
        ///     Fetch an update from the Nunchuk and push it to all the properties.
        /// </summary>
        /// <returns>An awaitable task that completes upon success.</returns>
        public async Task UpdateAsync()
        {
            // the nunchuk does not respond to standard SMBus-compliant register reads, so we have to write a register, then do a new transaction to read out the data
            await dev.WriteByte(0x00).ConfigureAwait(false);
            var response = await dev.ReadData(6).ConfigureAwait(false);

            joystick.X = response[0] / 127.5f - 1f; // normalize joystick data to [-1, 1]
            joystick.Y = response[1] / 127.5f - 1f;

            if (!oldJoystickData.SequenceEqual(response.Take(2)))
                JoystickChanged?.Invoke(this, new JoystickEventArgs {NewValue = joystick});

            Array.Copy(response, oldJoystickData, 2);

            accelerometer.X = 2 * (short) ((response[2] << 8) | (((response[5] >> 2) & 0x3) << 6)) /
                              (float) ushort.MaxValue; // normalize accelerometer data to [-1, 1]
            accelerometer.Y = 2 * (short) ((response[3] << 8) | (((response[5] >> 4) & 0x3) << 6)) /
                              (float) ushort.MaxValue;
            accelerometer.Z = 2 * (short) ((response[4] << 8) | (((response[5] >> 6) & 0x3) << 6)) /
                              (float) ushort.MaxValue;

            ((DigitalInPeripheralPin) Z.Input).DigitalValue = (response[5] & 0x01) == 0;
            ((DigitalInPeripheralPin) C.Input).DigitalValue = (response[5] & 0x02) == 0;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Joystick)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Accelerometer)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(C)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Z)));

        }

        public event JoystickChangedDelegate JoystickChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        public class JoystickEventArgs : EventArgs
        {
            public Vector2 NewValue { get; set; }
        }
    }
}