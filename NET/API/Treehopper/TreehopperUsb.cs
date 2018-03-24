using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Treehopper.ThirdParty;
using Treehopper.Utilities;

namespace Treehopper
{
/** The core class for communicating with %Treehopper USB boards.

![Treehopper pinout](images/treehopper-hardware.svg)
# Core Hardware {#core-hardware}
%Treehopper is a USB 2.0 Full Speed device with 20 \link Pin Pins\endlink — each of which can be used as an analog input, digital input, digital output, or soft-PWM output. Many of these pins also have dedicated peripheral functions for \link HardwareSpi SPI\endlink, \link HardwareI2C I2C\endlink, \link HardwareUart UART\endlink, and \link HardwarePwm PWM\endlink.

You'll access all the pins, peripherals, and board functions through this class, which will automatically create all peripheral instances for you.

## Getting a board reference
To obtain a reference to the board, use the \link Treehopper.ConnectionService.GetFirstDeviceAsync() ConnectionService.Instance.GetFirstDeviceAsync()\endlink method:

```
// Get a reference to the connected board, or 
// hang out on this line until a board is connected.
var board = await ConnectionService.Instance.GetFirstDeviceAsync();
// now do something with the newly-attached board
```

Or the \link Treehopper.ConnectionService.Boards ConnectionService.Instance.Boards\endlink ObservableCollection, which fires events useful for hot-plug reactions:

```
ConnectionService.Instance.Boards.CollectionChanged += async (o, e) =>
{
        var board = (TreehopperUsb)e.NewItems[0];
        // do something with the new board
}
```

\warning While you're free to create TreehopperUsb variables that reference boards, never call %TreehopperUsb's constructor yourself.

## Connect to the board
Before you use the board, you must explicitly connect to it by calling the ConnectAsync() method

```
var board = await ConnectionService.Instance.GetFirstDeviceAsync();
await board.ConnectAsync();
```

\note Once a board is connected, other applications won't be able to use it. When your app is running in Windows or macOS, you can investigate the #Name or #SerialNumber properties before calling ConnectAsync(). This will let you determine which board to connect to if you have multiple boards used by multiple applications.

## Next steps
To learn about accessing different %Treehopper peripherals, visit the doc links to the relevant classes:
 - Pin
 - HardwareSpi
 - HardwareI2C
 - HardwareUart
 - HardwarePwm
*/
    public class TreehopperUsb : INotifyPropertyChanged, IDisposable, IComparable, IEquatable<TreehopperUsb>,
        IEqualityComparer<TreehopperUsb>
    {
        private readonly static Settings settings = new Settings();

        private const int MinimumSupportedFirmwareVersion = 111;

        internal readonly AsyncLock ComsLock = new AsyncLock();

        private bool _led;
        private TaskCompletionSource<bool> _pinUpdateReportReceived = new TaskCompletionSource<bool>();

        /*! \cond PRIVATE */
        public TreehopperUsb(IConnection treehopperUsbConnection)
        {
            Pins = new ObservableCollection<Pin>();
            Connection = treehopperUsbConnection;

            // initialize pins
            for (var i = 0; i < NumberOfPins; i++)
                Pins.Add(new Pin(this, (byte) i));

            // set special names
            Pins[0].Name = "Pin 0 (SCK)";
            Pins[1].Name = "Pin 1 (MISO)";
            Pins[2].Name = "Pin 2 (MOSI)";
            Pins[3].Name = "Pin 3 (SDA)";
            Pins[4].Name = "Pin 4 (SCL)";
            Pins[5].Name = "Pin 5 (TX)";
            Pins[6].Name = "Pin 6 (RX)";
            Pins[7].Name = "Pin 7 (PWM1)";
            Pins[8].Name = "Pin 8 (PWM2)";
            Pins[9].Name = "Pin 9 (PWM3)";

            SoftPwmMgr = new SoftPwmManager(this);
            HardwarePwmManager = new HardwarePwmManager(this);

            // Initialize modules
            I2c = new HardwareI2C(this);
            Spi = new HardwareSpi(this);
            Uart = new HardwareUart(this);
            Pwm1 = new HardwarePwm(Pins[7]);
            Pwm2 = new HardwarePwm(Pins[8]);
            Pwm3 = new HardwarePwm(Pins[9]);
            ParallelInterface = new ParallelInterface(this);
        }
        /*! \endcond */

        /** @name Main components
         *  
         *  @{
         */
        /// <summary>
        ///     Open the TreehopperBoard. The board must be opened before any other methods are called.
        /// </summary>
        public async Task<bool> ConnectAsync()
        {
            if (Version < MinimumSupportedFirmwareVersion)
                Debug.WriteLine(
                    "NOTICE: The specified board has an old firmware version. Please use the Firmware Updater to load a firmware image with a minimum version of " +
                    MinimumSupportedFirmwareVersion);
            var res = await Connection.OpenAsync().ConfigureAwait(false);
            if (!res)
            {
                IsConnected = false;
                return false;
            }

            if (IsConnected)
                return true;

            Connection.PinEventDataReceived += Connection_PinEventDataReceived;
            IsConnected = true;

            RaisePropertyChanged(nameof(Version));
            RaisePropertyChanged(nameof(VersionString));

            Reinitialize();
            return true;
        }

        /// <summary>
        ///     This list contains all the pins that belong to the board.
        /// </summary>
        public ObservableCollection<Pin> Pins { get; }

        /// <summary>
        ///     SPI module
        /// </summary>
        public HardwareSpi Spi { get; }

        /// <summary>
        ///     I2C module
        /// </summary>
        public HardwareI2C I2c { get; }

        /// <summary>
        ///     Hardware UART supporting RS-232 and OneWire-style communication.
        /// </summary>
        public HardwareUart Uart { get; }

        /// <summary>
        ///     Hardware PWM #1
        /// </summary>
        public HardwarePwm Pwm1 { get; }

        /// <summary>
        ///     Hardware PWM #2
        /// </summary>
        public HardwarePwm Pwm2 { get; }

        /// <summary>
        ///     Hardware PWM #3
        /// </summary>
        public HardwarePwm Pwm3 { get; }

        /// <summary>
        ///     Gets or sets the LED state
        /// </summary>
        public bool Led
        {
            get { return _led; }

            set
            {
                if (value == _led)
                    return;
                _led = value;

                var data = new byte[2];
                data[0] = (byte)DeviceCommands.LedConfig;
                data[1] = (byte)(_led ? 0x01 : 0x00); // Unicode 16-bit strings are 2 bytes per character
                SendPeripheralConfigPacketAsync(data).Forget();
            }
        }

        /// <summary>
        ///     Get whether or not the board is connected
        /// </summary>
        public bool IsConnected { get; internal set; }

        /// <summary>
        ///     Close device
        /// </summary>
        /// <remarks>
        ///     This will reset all pins to inputs and then disconnect from the device. It will not free memory associated with
        ///     this device.
        /// </remarks>
        public void Disconnect()
        {
            try
            {
                Reinitialize();
            }
            catch (Exception)
            {
                // ignored
            }

            Connection?.Close();
            IsConnected = false;
        }

        /// <summary>
        ///     Shorthand pin accessor property
        /// </summary>
        /// <param name="index">The pin index to access</param>
        /// <returns>the Pin with the provided index</returns>
        /** This property gives you shorthand access to pins. Instead of typing
         * ```
         * newValue = board.Pins[5].DigitalValue;
         * ```
         * using this accessor, you can type:
         * ```
         * newValue = board[5].DigitalValue;
         * ```
         * We provide both accessors, because while this one is briefer, it is less
         * intuitive.
         */ 
        public Pin this[int index]
        {
            get
            {
                return Pins[index];
            }
        }

        /// <summary>
        ///     Occurs whenever a property changes.
        /// </summary>
        /// <remarks>
        /// </remarks>
        public event PropertyChangedEventHandler PropertyChanged;

        /** @} */

        /** @name Board identity & firmware management
         *  @{
         */
        
        /// <summary>
        ///     The name of the board
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         This property is set by a USB string descriptor read when the device is attached to the computer. It is
        ///         available even if the device is not open.
        ///     </para>
        ///     <para>
        ///         You can set this string to whichever value you want by using the Treehopper Firmware Manager software. Once
        ///         set, this property will persist, even
        ///         if the device is removed from power or connected to a different computer.
        ///     </para>
        ///     <para>
        ///         This property is useful for identifying a particular Treehopper board when you have multiple boards connected
        ///         to your computer, or you don't
        ///         want a program communicating with the incorrect board.
        ///     </para>
        /// </remarks>
        public string Name
        {
            get
            {
                return Connection.Name;
            }
        }

        
        /// <summary>
        /// The unique serial identification string for the board. 
        /// </summary>
        /**

        This property is available even if the device is not opened (though on Linux, the device will be implicitly opened and then closed to read this property). 
        
        Consequently, you can use this property, or the TreehopperUsb.Name property, to screen the devices your application attempts to connect to, without interfereing with other applications.

        You can write a new serial number to the board by calling UpdateSerialNumber().

        \warning
        Despite the name of this property, most device serial numbers will consist of both numbers and alphabetical characters. Do not attempt to convert this to a numeric type. 
        */
        public string SerialNumber
        {
            get
            {
                return Connection.Serial;
            }
        }

        /// <summary>
        ///     Update the name of the device.
        /// </summary>
        /// <param name="deviceName">A 60-character (or fewer) string containing the new name of the device.</param>
        /// <remarks>
        ///     While the name is immediately written to the device and the Name property is updated immediately, the changes
        ///     will not take effect to other applications until the device is reset. This can be done by calling
        ///     <see cref="Reboot()" />
        /// </remarks>
        public Task UpdateDeviceNameAsync(string deviceName)
        {
            if (deviceName.Length > 60)
                throw new Exception("Device name must be 60 characters or less");
            var dataToSend = new byte[deviceName.Length + 2];
            dataToSend[0] = (byte)DeviceCommands.FirmwareUpdateName;
            dataToSend[1] = (byte)deviceName.Length;
            var stringData = Encoding.UTF8.GetBytes(deviceName);
            stringData.CopyTo(dataToSend, 2);
            SendPeripheralConfigPacketAsync(dataToSend);
            return
                Task
                    .Delay(
                        100); // wait a bit for the flash operation to finish (global interrupts are disabled during programming)
        }

        /// <summary>
        ///     Update the serial number on the device.
        /// </summary>
        /// <param name="serialNumber">A 60-character (or fewer) string containing the new serial number</param>
        /// <remarks>
        ///     While the new serial number is immediately available from the SerialNumber property, the changes
        ///     will not take effect in other applications until the device is reset. This can be done by calling
        ///     <see cref="Reboot()" />
        /// </remarks>
        public Task UpdateSerialNumberAsync(string serialNumber)
        {
            if (serialNumber.Length > 60)
                throw new Exception("String must be 60 characters or less");

            var bytes = Encoding.UTF8.GetBytes(serialNumber);
            var dataToSend = new byte[bytes.Length + 2];
            dataToSend[0] = (byte)DeviceCommands.FirmwareUpdateSerial;
            dataToSend[1] = (byte)serialNumber.Length;
            bytes.CopyTo(dataToSend, 2);
            SendPeripheralConfigPacketAsync(dataToSend);
            return Task.Delay(100); // wait a bit for the flash operation to finish (global interrupts are disabled during programming)
        }


        /// <summary>
        ///     Get a string representation of the firmware version number
        /// </summary>
        public string VersionString
        {
            get
            {
                return $"{Connection.Version / 100d:0.00}";
            }
        }       

        /// <summary>
        ///     Get the firmware version number
        /// </summary>
        public int Version
        {
            get {
                return Connection.Version;
            }
        }

        /// <summary>
        ///     Reboots the board.
        /// </summary>
        /// <remarks>
        ///     Calling this method will automatically call the Disconnect() method, and no further communication will be possible
        ///     until the board is reopened.
        /// </remarks>
        public void Reboot()
        {
            SendPeripheralConfigPacketAsync(new[] { (byte)DeviceCommands.Reboot });
            Disconnect(); // This is called by the manager when the board is removed, but call it here just in case the manager isn't running.
        }

        /// <summary>
        ///     Reboot the board into bootloader mode
        /// </summary>
        public void RebootIntoBootloader()
        {
            SendPeripheralConfigPacketAsync(new[] { (byte)DeviceCommands.EnterBootloader });
            Disconnect(); // This is called by the manager when the board is removed, but call it here just in case the manager isn't running.
        }

        ///@}

        /** @name Other components
        *  @{
        */
        /// <summary>
        /// Controls the global settings used by all Treehopper libraries for this session
        /// </summary>
        /// <remarks>
        /// This is a static property. Access it through the TreehopperUsb class. As an example:
        /// ```
        /// TreehopperUsb.Settings.PropertyWritesReturnImmediately = true;
        /// ```
        /// </remarks>
        public static Settings Settings
        {
            get
            {
                return settings;
            }
        }

        /// <summary>
        ///     Event fires whenever a new pin data report comes in (i.e., when all input pins are updated)
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         While each pin can be configured to fire events when it changes, if you're dealing with a large number of input
        ///         pins, you may get better performance by subscribing to this event alone.
        ///     </para>
        /// </remarks>
        public event PinValuesUpdatedHandler OnPinValuesUpdated;

        /// <summary>
        ///     8080-style parallel interface
        /// </summary>
        public ParallelInterface ParallelInterface { get; }

        /// <summary>
        /// Awaits a pin update report from the board
        /// </summary>
        /// <returns>An awaitable Task that completes when a new pin update report has been received.</returns>
        public Task AwaitPinUpdateAsync()
        {
            _pinUpdateReportReceived = new TaskCompletionSource<bool>();
            return _pinUpdateReportReceived.Task;
        }

        /// <summary>
        ///     Reinitialize the board, setting all pins as digital inputs
        /// </summary>
        public void Reinitialize()
        {
            var data = new byte[2];
            data[0] = (byte)DeviceCommands.ConfigureDevice;
            SendPeripheralConfigPacketAsync(data);
        }

        /// <summary>
        ///     Hardware PWM manager
        /// </summary>
        public HardwarePwmManager HardwarePwmManager { get; }

        /// <summary>
        ///     Get the number of pins of this board
        /// </summary>
        public int NumberOfPins { get; } = 20;

        /// <summary>
        ///     Get the Connection used by this board
        /// </summary>
        public IConnection Connection { get; }

        /// <summary>
        ///     Instance of SoftPwmMgr
        /// </summary>
        internal SoftPwmManager SoftPwmMgr { get; }

        /// <summary>
        ///     Close device connection and free memory.
        /// </summary>
        public void Dispose()
        {
            Disconnect();
            Connection.Dispose();
        }

        /// <summary>
        ///     Compares another Treehopper to this one.
        /// </summary>
        /// <param name="obj">The other Treehopper</param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            var board = obj as TreehopperUsb;
            if (board == null)
                return 1;

            return string.CompareOrdinal(Connection.DevicePath, board.Connection.DevicePath);
        }

        /// <summary>
        ///     Determines if two Treehopper boards are the same
        /// </summary>
        /// <param name="x">The first board</param>
        /// <param name="y">The second board</param>
        /// <returns></returns>
        public bool Equals(TreehopperUsb x, TreehopperUsb y)
        {
            if (x == null || y == null)
                return false;
            return x.Connection.DevicePath == y.Connection.DevicePath;
        }

        /// <summary>
        ///     Hash, obtained from the device path.
        /// </summary>
        /// <param name="board">the Treehopper board to calculate the hash for</param>
        /// <returns>The hash code for the board</returns>
        /// <see cref="Object.GetHashCode" />
        public int GetHashCode(TreehopperUsb board)
        {
            if (board != null)
                if (board.Name != null)
                    return board.Connection.DevicePath.GetHashCode();
                else
                    return 0;
            return 0;
        }

        /// <summary>
        ///     Determines if another Treehopper board is the same as this one
        /// </summary>
        /// <param name="other">The other Treehopper board to compare</param>
        /// <returns>True if the two boards are the same</returns>
        public bool Equals(TreehopperUsb other)
        {
            return other.Connection.DevicePath == Connection.DevicePath;
        }

        /// <summary>
        ///     A human-readable string describing this instance. This is formed from the name and serial number of the device.
        /// </summary>
        /// <returns>The string</returns>
        public override string ToString()
        {
            return $"{Name} ({SerialNumber})";
        }

        /// <summary>
        ///     Get the hash code of this board
        /// </summary>
        /// <returns>The hash code</returns>
        public override int GetHashCode()
        {
            return GetHashCode(this);
        }

        /// <summary>
        ///     Determines if another object is the same as this Treehopper board
        /// </summary>
        /// <param name="obj">The other Treehopper board to compare</param>
        /// <returns>True if the two boards are the same</returns>
        public override bool Equals(object obj)
        {
            var y = obj as TreehopperUsb;
            if (y == null)
                return false;
            return SerialNumber == y.SerialNumber;
        }

        ///@}


        /// <summary>
        ///     Destruct a Treehopper board
        /// </summary>
        ~TreehopperUsb()
        {
            Dispose();
        }

        internal Task SendPinConfigPacketAsync(byte[] data)
        {
            if (IsConnected)
                return Connection.SendDataPinConfigChannelAsync(data);
            return Task.FromResult<object>(null);
        }

        internal Task SendPeripheralConfigPacketAsync(byte[] data)
        {
            if (IsConnected)
                return Connection.SendDataPeripheralChannelAsync(data);
            return Task.FromResult<object>(null);
        }

        internal Task<byte[]> ReceiveCommsResponsePacketAsync(uint bytesToRead)
        {
            return Connection.ReadPeripheralResponsePacketAsync(bytesToRead);
        }

        private void RaisePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        private void Connection_PinEventDataReceived(byte[] pinStateBuffer)
        {
            _pinUpdateReportReceived.TrySetResult(false);
            if (pinStateBuffer[0] != 0x00)
            {
                var i = 1;
                foreach (var pin in Pins)
                    pin.UpdateValue(pinStateBuffer[i++], pinStateBuffer[i++]);

                OnPinValuesUpdated?.Invoke(this, new EventArgs());
            }
        }
    }
}
