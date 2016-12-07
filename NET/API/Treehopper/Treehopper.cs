using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.ComponentModel;
using System.Text.RegularExpressions;
using Treehopper;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Treehopper.ThirdParty;

namespace Treehopper
{
	/// <summary>
	/// This is the main Treehopper namespace that contains core Treehopper classes.
	/// </summary>
	internal static class NamespaceDoc
	{

	}

	internal enum DeviceCommands : byte
	{
		Reserved = 0,	// Not implemented
		ConfigureDevice,	// Sent upon device connect/disconnect
		PwmConfig,	// Configures the hardware DAC
		UartConfig,	// Not implemented
		I2cConfig,	// Configures i2C master
		SpiConfig,	// Configures SPI master
		I2cTransaction,	// (Endpoint 2) Performs an i2C transaction 
		SpiTransaction,	// (Endpoint 2) Performs an SPI transaction
        UartTransaction,
		SoftPwmConfig,	// 
		FirmwareUpdateSerial,	//
		FirmwareUpdateName,	//
		Reboot,	//
		EnterBootloader,	//
        LedConfig,
        ParallelConfig,
        ParallelTransaction
    }

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
        /// <summary>
        /// Controls the global settings used by all Treehopper libraries for this session
        /// </summary>
        public static Settings Settings { get; set; } = new Settings();


        const int MinimumSupportedFirmwareVersion = 110;
        byte[] pinStateBuffer;
        private IConnection connection;
        private bool led = false;

        /// <summary>
        /// Construct a new TreehopperUsb board from a connection
        /// </summary>
        /// <param name="treehopperUsbConnection">the connection to construct the board with</param>
        public TreehopperUsb(IConnection treehopperUsbConnection)
        {
            this.connection = treehopperUsbConnection;

            // initialize pins
            for(int i=0;i<NumberOfPins;i++)
            {
                Pins.Add(new Pin(this, (byte)i));
            }

            SoftPwmMgr = new SoftPwmManager(this);
            PwmManager = new HardwarePwmManager(this);

            // Initialize modules
            I2c = new HardwareI2c(this);
            Spi = new Spi(this);
            Uart = new Uart(this);
            Pwm1 = new HardwarePwm(Pins[7]);
            Pwm2 = new HardwarePwm(Pins[8]);
            Pwm3 = new HardwarePwm(Pins[9]);
            ParallelInterface = new ParallelInterface(this);
        }

        #region Modules

        internal readonly AsyncLock ComsLock = new AsyncLock();

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
        public Uart Uart { get; private set; }

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
        /// Instance of SoftPwmMgr
        /// </summary>
        internal SoftPwmManager SoftPwmMgr { get; private set; }

        /// <summary>
        /// Hardware PWM manager
        /// </summary>
        public HardwarePwmManager PwmManager { get; private set; }

        /// <summary>
        /// Gets or sets the LED state
        /// </summary>
        public bool Led
        {
            get
            {
                return led;
            }
            set
            {
                if (value == led)
                    return;
                led = value;

                byte[] DataToSend = new byte[2];
                DataToSend[0] = (byte)DeviceCommands.LedConfig;
                DataToSend[1] = (byte)(led ? 0x01 : 0x00); // Unicode 16-bit strings are 2 bytes per character
                sendPeripheralConfigPacket(DataToSend);
            }
        }

        #endregion

        /// <summary>
        /// Quick pin accessor property
        /// </summary>
        /// <param name="index">The pin index to access</param>
        /// <returns></returns>
        public Pin this[int index]
        {
            get
            {
                return Pins[index];
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
        public IConnection Connection { get { return connection; } }
        

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
				return connection.SerialNumber;
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
				return connection.Name;
			}
		}

        /// <summary>
        /// Get a string representation of the firmware version number
        /// </summary>
        public string VersionString
        {
            get
            {
                return Utilities.BcdToString(connection.Version, 2);
            }
        }

        /// <summary>
        /// Get the firmware version number
        /// </summary>
        public int Version
        {
            get
            {
                return Utilities.BcdToInt(connection.Version);
            }
        }

        /// <summary>
        /// This list contains all the pins that belong to the board. 
        /// </summary>
        public ObservableCollection<Pin> Pins { get; set; } = new ObservableCollection<Pin>();


        /// <summary>
        /// Reboots the board.
        /// </summary>
        /// <remarks>
        /// Calling this method will automatically call the Disconnect() method, and no further communication will be possible until the board is reopened.
        /// </remarks>
        public void Reboot()
        {
            sendPeripheralConfigPacket(new byte[] { (byte)DeviceCommands.Reboot });
            Disconnect(); // This is called by the manager when the board is removed, but call it here just in case the manager isn't running.
        }

        /// <summary>
        /// Reboot the board into bootloader mode
        /// </summary>
        public void RebootIntoBootloader()
        {
            sendPeripheralConfigPacket(new byte[] { (byte)DeviceCommands.EnterBootloader });
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

            byte[] bytes = Encoding.UTF8.GetBytes(serialNumber);
            byte[] DataToSend = new byte[bytes.Length + 2];
            DataToSend[0] = (byte)DeviceCommands.FirmwareUpdateSerial;
            DataToSend[1] = (byte)(serialNumber.Length); // Unicode 16-bit strings are 2 bytes per character
            bytes.CopyTo(DataToSend, 2);
            sendPeripheralConfigPacket(DataToSend);
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
            byte[] DataToSend = new byte[deviceName.Length + 2];
            DataToSend[0] = (byte)DeviceCommands.FirmwareUpdateName;
            DataToSend[1] = (byte)(deviceName.Length); // Unicode 16-bit strings are 2 bytes per character
            byte[] stringData = Encoding.UTF8.GetBytes(deviceName);
            stringData.CopyTo(DataToSend, 2);
            sendPeripheralConfigPacket(DataToSend);
            return Task.Delay(100); // wait a bit for the flash operation to finish (global interrupts are disabled during programming)
        }

        /// <summary>
        /// Destruct a Treehopper board
        /// </summary>
        ~TreehopperUsb()
        {
            Dispose();
        }

        /// <summary>
        /// Open the TreehopperBoard. The board must be opened before any other methods are called.
        /// </summary>
        public async Task<bool> ConnectAsync()
		{
            if (Version < MinimumSupportedFirmwareVersion)
                Debug.WriteLine("NOTICE: The specified board has an old firmware version. Please use the Firmware Updater to load a firmware image with a minimum version of " + MinimumSupportedFirmwareVersion);
            bool res = await connection.OpenAsync().ConfigureAwait(false);
            if (!res)
            {
                IsConnected = false;
                return false;
            }
                

            if (IsConnected)
                return true;

			pinStateBuffer = new byte[64];

            connection.PinEventDataReceived += Connection_PinEventDataReceived; 
			
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
            sendPeripheralConfigPacket(data);
        }

        private void Connection_PinEventDataReceived(byte[] pinStateBuffer)
        {
            if (pinStateBuffer[0] != 0x00)
            {
                int i = 1;
                foreach(Pin pin in Pins)
                {
                    pin.UpdateValue(pinStateBuffer[i++], pinStateBuffer[i++]);
                }

                OnPinValuesUpdated?.Invoke(this, new EventArgs());

            }
        }

		internal void sendPinConfigPacket(byte[] data)
		{
            if(IsConnected)
			    connection.SendDataPinConfigChannel (data);
		}

		internal void sendPeripheralConfigPacket(byte[] data)
		{
            if(IsConnected)
                connection.SendDataPeripheralChannel(data);
		}

		internal Task<byte[]> receiveCommsResponsePacket(uint bytesToRead)
		{
			return connection.ReadPeripheralResponsePacket(bytesToRead);
		}

		/// <summary>
		/// Close device
		/// </summary>
		/// <remarks>
		/// This will reset all pins to inputs and then disconnect from the device. It will not free memory associated with this device.
		/// </remarks>
		public void Disconnect()
		{
            Reinitialize();
            if (connection != null)
                connection.Close();
            IsConnected = false;
		}

        /// <summary>
        /// Close device connection and free memory. 
        /// </summary>
        public void Dispose()
        {
            Disconnect();
            connection.Dispose();
        }

		/// <summary>
		/// Compares another Treehopper to this one.
		/// </summary>
		/// <param name="obj">The other Treehopper</param>
		/// <returns></returns>
		public int CompareTo(object obj)
		{
			TreehopperUsb board = obj as TreehopperUsb;
			if (board == null)
				return 1;
			return String.Compare(ToString(), board.ToString());
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
			return (other.ToString() == this.ToString());
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
			return (x.ToString() == y.ToString());
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
					return board.ToString().GetHashCode();
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
			TreehopperUsb y = obj as TreehopperUsb;
			if (y == null)
				return false;
			else
				return this.ToString() == y.ToString();
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

		private void RaisePropertyChanged(string property)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(property));
		}
	}

	/// <summary>
	/// This comparer is used to compare Treehopper boards
	/// </summary>
	public class TreehopperBoardEqualityComparer : EqualityComparer<TreehopperUsb>
	{
		/// <summary>
		/// Determines if two treehopper boards are the same
		/// </summary>
		/// <param name="x">The first board to compare</param>
		/// <param name="y">The second board to compare</param>
		/// <returns>Whether the boards are equal</returns>
		public override bool Equals(TreehopperUsb x, TreehopperUsb y)
		{
			if (x == null || y == null)
				return false;
			return (x.ToString() == y.ToString());
		}

		/// <summary>
		/// Returns the Hash of the Treehopper board
		/// </summary>
		/// <param name="obj">The Treehopper board to calculate the hash for</param>
		/// <returns>The calculated hash value</returns>
		public override int GetHashCode(TreehopperUsb obj)
		{
			if (obj != null)
				if (obj.Name != null)
					return obj.ToString().GetHashCode();
				else
					return 0;
			else
				return 0;
		}
	}

    /// <summary>
    /// Task extensions
    /// </summary>
    public static class TaskExtensions
    {
        /// <summary>
        /// Empty method which prevents VS from generating warnings from un-awaited calls. 
        /// </summary>
        /// <param name="task"></param>
        public static void Forget(this Task task)
        {
        }
    }
}
