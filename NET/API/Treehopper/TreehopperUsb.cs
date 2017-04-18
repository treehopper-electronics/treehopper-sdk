namespace Treehopper
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Text;
    using System.Threading.Tasks;
    using ThirdParty;
    using Utilities;

    /// <summary>
    /// TreehopperBoard is the main class for interacting with Treehopper. Once constructed, it contains instances of all pins and peripherals. 
    /// <seealso cref="IConnectionService"/>
    /// </summary>
    /// <remarks>
    /// The lifecycle of a TreehopperBoard instance begins when a board is connected. Usually, TreehopperManager is used to 
    /// automatically instantiate a TreehopperBoard, and return the reference to the user's application. 
    /// 
    /// All pins and modules are automatically constructed by this class, so you can use all functions without worrying about calling constructors.
    /// 
    /// </remarks>
    public class TreehopperUsb : INotifyPropertyChanged, IDisposable, IComparable, IEquatable<TreehopperUsb>, IEqualityComparer<TreehopperUsb>
    {
        internal readonly AsyncLock ComsLock = new AsyncLock();
        private const int MinimumSupportedFirmwareVersion = 110;
        private IConnection _connection;
        private bool _led = false;

        /// <summary>
        /// Construct a new TreehopperUsb board from a connection
        /// </summary>
        /// <param name="treehopperUsbConnection">the connection to construct the board with</param>
        public TreehopperUsb(IConnection treehopperUsbConnection)
        {
            this._connection = treehopperUsbConnection;

            // initialize pins
            for (var i = 0; i < NumberOfPins; i++)
            {
                Pins.Add(new Pin(this, (byte)i));
            }

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
            I2c = new HardwareI2c(this);
            Spi = new HardwareSpi(this);
            Uart = new HardwareUart(this);
            Pwm1 = new HardwarePwm(Pins[7]);
            Pwm2 = new HardwarePwm(Pins[8]);
            Pwm3 = new HardwarePwm(Pins[9]);
            ParallelInterface = new ParallelInterface(this);
        }

        /// <summary>
        /// Destruct a Treehopper board
        /// </summary>
        ~TreehopperUsb()
        {
            Dispose();
        }

        /// <summary>
        /// Occurs whenever a property changes.
        /// </summary>
        /// <remarks>
        /// </remarks>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Event fires whenever a new pin data report comes in (i.e., when all input pins are updated)
        /// </summary>
        /// <remarks>
        /// <para>
        /// While each pin can be configured to fire events when it changes, if you're dealing with a large number of input pins, you may get better performance by subscribing to this event alone.
        /// </para>
        /// </remarks>
        public event PinValuesUpdatedHandler OnPinValuesUpdated;

        /// <summary>
        /// Controls the global settings used by all Treehopper libraries for this session
        /// </summary>
        public static Settings Settings { get; set; } = new Settings();

        /// <summary>
        /// I2C module
        /// </summary>
        public I2c I2c { get; private set; }

        /// <summary>
        /// SPI module
        /// </summary>
        public Spi Spi { get; private set; }

        /// <summary>
        /// Hardware UART supporting RS-232 and OneWire-style communication.
        /// </summary>
        public HardwareUart Uart { get; private set; }

        /// <summary>
        /// Hardware PWM #1
        /// </summary>
        public Pwm Pwm1 { get; private set; }

        /// <summary>
        /// Hardware PWM #2
        /// </summary>
        public Pwm Pwm2 { get; private set; }

        /// <summary>
        /// Hardware PWM #3
        /// </summary>
        public Pwm Pwm3 { get; private set; }

        /// <summary>
        /// 8080-style parallel interface
        /// </summary>
        public ParallelInterface ParallelInterface { get; private set; }

        /// <summary>
        /// Hardware PWM manager
        /// </summary>
        public HardwarePwmManager HardwarePwmManager { get; private set; }

        /// <summary>
        /// Gets or sets the LED state
        /// </summary>
        public bool Led
        {
            get
            {
                return _led;
            }

            set
            {
                if (value == _led)
                    return;
                _led = value;

                var data = new byte[2];
                data[0] = (byte)DeviceCommands.LedConfig;
                data[1] = (byte)(_led ? 0x01 : 0x00); // Unicode 16-bit strings are 2 bytes per character
                if(TreehopperUsb.Settings.PropertyWritesReturnImmediately)
                    SendPeripheralConfigPacket(data).Forget();
                else
                    SendPeripheralConfigPacket(data).Wait();
            }
        }

        /// <summary>
        /// Get whether or not the board is connected
        /// </summary>
        public bool IsConnected { get; internal set; }

        /// <summary>
        /// Get the number of pins of this board
        /// </summary>
        public int NumberOfPins { get; private set; } = 20;

        /// <summary>
        /// Get the Connection used by this board
        /// </summary>
        public IConnection Connection
        {
            get { return _connection; }
        }

        /// <summary>
        /// The unique identification string for the board.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This string is part of the USB descriptors, so it is available even if the device is not opened. Since it is nontrivial to detect
        /// which device your program is attempting to connect to, this property may be useful for screening your software from connecting to
        /// unwanted hardware clones.
        /// </para>
        /// <para>
        /// Despite the name of this property, most device serial numbers will consist of both numbers and alphabetical characters. Do not
        /// attempt to convert this to a numeric type.
        /// </para>
        /// <para>
        /// This property is used internally by <see cref="IConnectionService"/> to match unique devices. Attaching two devices with the same
        /// serial number will produce undesirable application behavior.
        /// </para>
        /// <para>
        /// This number is set during factory programming. Re-flashing device firmware using the USB bootloader will not alter this number.
        /// However, if you re-program the device using ICSP hardware, care must be taken not to erase the serial number from the device. 
        /// If you accidentally erase this number, you must reprogram it using the Factory Programmer software.
        /// </para>
        /// </remarks>
        public string SerialNumber
        {
            get
            {
                return _connection.Serial;
            }
        }

        /// <summary>
        /// The name of the board
        /// </summary>
        /// <remarks>
        /// <para>
        /// This property is set by a USB string descriptor read when the device is attached to the computer. It is available even if the device is not open.
        /// </para>
        /// <para>
        /// You can set this string to whichever value you want by using the Treehopper Firmware Manager software. Once set, this property will persist, even
        /// if the device is removed from power or connected to a different computer.
        /// </para>
        /// <para>
        /// This property is useful for identifying a particular Treehopper board when you have multiple boards connected to your computer, or you don't
        /// want a program communicating with the incorrect board.
        /// </para>
        /// </remarks>
        public string Name
        {
            get
            {
                return _connection.Name;
            }
        }

        /// <summary>
        /// Get a string representation of the firmware version number
        /// </summary>
        public string VersionString
        {
            get
            {
                return $"{_connection.Version / 100d:0.00}";
            }
        }

        /// <summary>
        /// Get the firmware version number
        /// </summary>
        public int Version
        {
            get
            {
                return _connection.Version;
            }
        }

        /// <summary>
        /// This list contains all the pins that belong to the board. 
        /// </summary>
        public ObservableCollection<Pin> Pins { get; set; } = new ObservableCollection<Pin>();

        /// <summary>
        /// Instance of SoftPwmMgr
        /// </summary>
        internal SoftPwmManager SoftPwmMgr { get; private set; }

        /// <summary>
        /// Quick pin accessor property
        /// </summary>
        /// <param name="index">The pin index to access</param>
        /// <returns></returns>
        public Pin this[int index]
        {
            get { return Pins[index]; }
        }

        /// <summary>
        /// Reboots the board.
        /// </summary>
        /// <remarks>
        /// Calling this method will automatically call the Disconnect() method, and no further communication will be possible until the board is reopened.
        /// </remarks>
        public void Reboot()
        {
            SendPeripheralConfigPacket(new byte[] { (byte)DeviceCommands.Reboot });
            Disconnect(); // This is called by the manager when the board is removed, but call it here just in case the manager isn't running.
        }

        /// <summary>
        /// Reboot the board into bootloader mode
        /// </summary>
        public void RebootIntoBootloader()
        {
            SendPeripheralConfigPacket(new byte[] { (byte)DeviceCommands.EnterBootloader });
            Disconnect(); // This is called by the manager when the board is removed, but call it here just in case the manager isn't running.
        }

        /// <summary>
        /// Update the serial number on the device.
        /// </summary>
        /// <param name="serialNumber">A 60-character (or fewer) string containing the new serial number</param>
        /// <remarks>
        /// While the name is immediately written to the device and the Name property is updated immediately, the changes 
        /// will not take effect to other applications until the device is reset. This can be done by calling <see cref="Reboot()"/>
        /// </remarks>
        public Task UpdateSerialNumber(string serialNumber)
        {
            if (serialNumber.Length > 60)
                throw new Exception("String must be 15 characters or less");

            var bytes = Encoding.UTF8.GetBytes(serialNumber);
            var dataToSend = new byte[bytes.Length + 2];
            dataToSend[0] = (byte)DeviceCommands.FirmwareUpdateSerial;
            dataToSend[1] = (byte)serialNumber.Length;
            bytes.CopyTo(dataToSend, 2);
            SendPeripheralConfigPacket(dataToSend);
            return Task.Delay(100); // wait a bit for the flash operation to finish (global interrupts are disabled during programming)
        }

        /// <summary>
        /// Update the name of the device.
        /// </summary>
        /// <param name="deviceName">A 60-character (or fewer) string containing the new name of the device.</param>
        /// <remarks>
        /// While the name is immediately written to the device and the Name property is updated immediately, the changes 
        /// will not take effect to other applications until the device is reset. This can be done by calling <see cref="Reboot()"/>
        /// </remarks>
        public Task UpdateDeviceName(string deviceName)
        {
            if (deviceName.Length > 60)
                throw new Exception("Device name must be 60 characters or less");
            var dataToSend = new byte[deviceName.Length + 2];
            dataToSend[0] = (byte)DeviceCommands.FirmwareUpdateName;
            dataToSend[1] = (byte)deviceName.Length;
            var stringData = Encoding.UTF8.GetBytes(deviceName);
            stringData.CopyTo(dataToSend, 2);
            SendPeripheralConfigPacket(dataToSend);
            return Task.Delay(100); // wait a bit for the flash operation to finish (global interrupts are disabled during programming)
        }

        /// <summary>
        /// Open the TreehopperBoard. The board must be opened before any other methods are called.
        /// </summary>
        public async Task<bool> ConnectAsync()
        {
            if (Version < MinimumSupportedFirmwareVersion)
                Debug.WriteLine("NOTICE: The specified board has an old firmware version. Please use the Firmware Updater to load a firmware image with a minimum version of " + MinimumSupportedFirmwareVersion);
            var res = await _connection.OpenAsync().ConfigureAwait(false);
            if (!res)
            {
                IsConnected = false;
                return false;
            }

            if (IsConnected)
                return true;

            _connection.PinEventDataReceived += Connection_PinEventDataReceived;
            this.IsConnected = true;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Version"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("VersionString"));

            Reinitialize();
            return true;
        }

        /// <summary>
        /// Reinitialize the board, setting all pins as digital inputs
        /// </summary>
        public void Reinitialize()
        {
            var data = new byte[2];
            data[0] = (byte)DeviceCommands.ConfigureDevice;
            SendPeripheralConfigPacket(data);
        }

        /// <summary>
        /// Close device
        /// </summary>
        /// <remarks>
        /// This will reset all pins to inputs and then disconnect from the device. It will not free memory associated with this device.
        /// </remarks>
        public void Disconnect()
        {
            try
            {
                Reinitialize();
            } catch(Exception ex)
            {

            }

            if (_connection != null)
                _connection.Close();
            IsConnected = false;
        }

        /// <summary>
        /// Close device connection and free memory. 
        /// </summary>
        public void Dispose()
        {
            Disconnect();
            _connection.Dispose();
        }

        /// <summary>
        /// Compares another Treehopper to this one.
        /// </summary>
        /// <param name="obj">The other Treehopper</param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            var board = obj as TreehopperUsb;
            if (board == null)
                return 1;

            return string.Compare(ToString(), board.ToString());
        }

        /// <summary>
        /// A human-readable string describing this instance. This is formed from the name and serial number of the device.
        /// </summary>
        /// <returns>The string</returns>
        public override string ToString()
        {
            return Name + " (" + SerialNumber + ")";
        }

        /// <summary>
        /// Determines if another Treehopper board is the same as this one
        /// </summary>
        /// <param name="other">The other Treehopper board to compare</param>
        /// <returns>True if the two boards are the same</returns>
        public bool Equals(TreehopperUsb other)
        {
            return other.ToString() == this.ToString();
        }

        /// <summary>
        /// Determines if two Treehopper boards are the same
        /// </summary>
        /// <param name="x">The first board</param>
        /// <param name="y">The second board</param>
        /// <returns></returns>
        public bool Equals(TreehopperUsb x, TreehopperUsb y)
        {
            if (x == null || y == null)
                return false;
            return x.ToString() == y.ToString();
        }

        /// <summary>
        /// Get the hash code of this board
        /// </summary>
        /// <returns>The hash code</returns>
        public override int GetHashCode()
        {
            return GetHashCode(this);
        }

        /// <summary>
        /// Hash, obtained from the serial number of the device.
        /// </summary>
        /// <param name="board">the Treehopper board to calculate the hash for</param>
        /// <returns>The hash code for the board</returns>
        /// <see cref="Object.GetHashCode"/>
        public int GetHashCode(TreehopperUsb board)
        {
            if (board != null)
                if (board.Name != null)
                    return board.SerialNumber.GetHashCode();
                else
                    return 0;
            else
                return 0;
        }

        /// <summary>
        /// Determines if another object is the same as this Treehopper board
        /// </summary>
        /// <param name="obj">The other Treehopper board to compare</param>
        /// <returns>True if the two boards are the same</returns>
        public override bool Equals(object obj)
        {
            var y = obj as TreehopperUsb;
            if (y == null)
                return false;
            else
                return this.SerialNumber == y.SerialNumber;
        }

        internal Task SendPinConfigPacket(byte[] data)
        {
            if (IsConnected)
                return _connection.SendDataPinConfigChannel(data);
            else
                return Task.FromResult<object>(null);
        }

        internal Task SendPeripheralConfigPacket(byte[] data)
        {
            if (IsConnected)
                return _connection.SendDataPeripheralChannel(data);
            else
                return Task.FromResult<object>(null);
        }

        internal Task<byte[]> ReceiveCommsResponsePacket(uint bytesToRead)
        {
            return _connection.ReadPeripheralResponsePacket(bytesToRead);
        }

        private void RaisePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        private void Connection_PinEventDataReceived(byte[] pinStateBuffer)
        {
            if (pinStateBuffer[0] != 0x00)
            {
                var i = 1;
                foreach (var pin in Pins)
                {
                    pin.UpdateValue(pinStateBuffer[i++], pinStateBuffer[i++]);
                }

                OnPinValuesUpdated?.Invoke(this, new EventArgs());
            }
        }
    }
}
