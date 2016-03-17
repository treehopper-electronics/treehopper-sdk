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
using Nito.AsyncEx;

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
		GetDeviceInfo,	// Not implemented
		PinConfig,	// Configures a GPIO pin as an input/output
		ComparatorConfig,	// Not implemented
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
        LedConfig
	}

	internal enum DeviceResponse : byte
	{
		Reserved = 0,
		DeviceInfo,
		CurrentReadings,
		UARTDataReceived,
		I2CDataReceived,
		SPIDataReceived
	}


    public delegate void TreehopperUsbAddedHandler(TreehopperUsb BoardAdded);

    public delegate void TreehopperUsbRemovedHandler(TreehopperUsb BoardRemoved);

    /// <summary>
    /// TreehopperBoard is the main class for interacting with Treehopper. Once constructed, it contains instances of all pins and peripherals. 
    /// <seealso cref="TreehopperManager"/>
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
        // Settings
        private static Settings settings = new Settings();
        public static Settings Settings {
            get {
                return settings;
            }
            set {
                settings = value;
            }
        }

        public Pin this[int index]
        {
            get
            {
                return Pins[index];
            }
        }

        #region Pin Definitions

        private Pin1 pin1;
		/// <summary>
		/// Instance of <see cref="Treehopper.Pin1"/>
		/// </summary>
		public Pin1 Pin1
		{
			get { return pin1; }
		}

		private Pin2 pin2;
		/// <summary>
		/// Instance of <see cref="Treehopper.Pin2"/>
		/// </summary>
		public Pin2 Pin2
		{
			get { return pin2; }
		}

		private Pin3 pin3;
		/// <summary>
		/// Instance of <see cref="Treehopper.Pin3"/>
		/// </summary>
		public Pin3 Pin3
		{
			get { return pin3; }
		}

		private Pin4 pin4;
		/// <summary>
		/// Instance of <see cref="Treehopper.Pin4"/>
		/// </summary>
		public Pin4 Pin4
		{
			get { return pin4; }
		}

		private Pin5 pin5;
		/// <summary>
		/// Instance of <see cref="Treehopper.Pin5"/>
		/// </summary>
		public Pin5 Pin5
		{
			get { return pin5; }
		}

		private Pin6 pin6;
		/// <summary>
		/// Instance of <see cref="Treehopper.Pin6"/>
		/// </summary>
		public Pin6 Pin6
		{
			get { return pin6; }
		}

		private Pin7 pin7;
		/// <summary>
		/// Instance of <see cref="Treehopper.Pin7"/>
		/// </summary>
		public Pin7 Pin7
		{
			get { return pin7; }
		}

		private Pin8 pin8;
		/// <summary>
		/// Instance of <see cref="Treehopper.Pin8"/>
		/// </summary>
		public Pin8 Pin8
		{
			get { return pin8; }
		}

		private Pin9 pin9;
		/// <summary>
		/// Instance of <see cref="Treehopper.Pin9"/>
		/// </summary>
		public Pin9 Pin9
		{
			get { return pin9; }
		}

		private Pin10 pin10;
		/// <summary>
		/// Instance of <see cref="Treehopper.Pin10"/>
		/// </summary>
		public Pin10 Pin10
		{
			get { return pin10; }
		}

		private Pin11 pin11;
		/// <summary>
		/// Instance of <see cref="Treehopper.Pin11"/>
		/// </summary>
		public Pin11 Pin11
		{
			get { return pin11; }
		}

		private Pin12 pin12;
		/// <summary>
		/// Instance of <see cref="Treehopper.Pin12"/>
		/// </summary>
		public Pin12 Pin12
		{
			get { return pin12; }
		}

		private Pin13 pin13;
		/// <summary>
		/// Instance of <see cref="Treehopper.Pin13"/>
		/// </summary>
		public Pin13 Pin13
		{
			get { return pin13; }
		}

		private Pin14 pin14;
		/// <summary>
		/// Instance of <see cref="Treehopper.Pin14"/>
		/// </summary>
		public Pin14 Pin14
		{
			get { return pin14; }
		}

        #endregion

        #region Modules

        internal readonly AsyncLock ComsMutex = new AsyncLock();

        I2c i2c;
		/// <summary>
		/// I2C module
		/// </summary>
		public I2c I2c { get { return i2c; } }

		Spi spi;
		/// <summary>
		/// SPI module
		/// </summary>
		public Spi Spi { get { return spi;	} }

        Uart uart;

        public Uart Uart { get { return uart; } }


		/// <summary>
		/// Instance of SoftPwmMgr
		/// </summary>
		internal SoftPwmManager SoftPwmMgr { get; set; }

        public PwmManager PwmManager { get; set; }

        #endregion

        private bool led = false;
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

		byte[] pinStateBuffer;

		private IConnection connection;


        public TreehopperUsb(IConnection treehopperUsbConnection)
        {
            this.connection = treehopperUsbConnection;
        }

        public IConnection Connection { get { return connection; } }
        
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
		/// will not take effect to other applications until the device is reset. This can be done by calling <see cref="Reset"/>
		/// </remarks>
		public void UpdateSerialNumber(string serialNumber)
		{
			if (serialNumber.Length > 60)
				throw new Exception("String must be 15 characters or less");

			byte[] bytes = Encoding.UTF8.GetBytes(serialNumber);
			byte[] DataToSend = new byte[bytes.Length + 2];
			DataToSend[0] = (byte)DeviceCommands.FirmwareUpdateSerial;
			DataToSend[1] = (byte)(serialNumber.Length); // Unicode 16-bit strings are 2 bytes per character
			bytes.CopyTo(DataToSend, 2);
			sendPeripheralConfigPacket(DataToSend);
            //Thread.Sleep(100); // wait a bit for the flash operation to finish (global interrupts are disabled during programming)
        }

		/// <summary>
		/// Update the name of the device.
		/// </summary>
		/// <param name="deviceName">A 60-character (or fewer) string containing the new name of the device.</param>
		/// <remarks>
		/// While the name is immediately written to the device and the Name property is updated immediately, the changes 
		/// will not take effect to other applications until the device is reset. This can be done by calling <see cref="Reset"/>
		/// </remarks>
		public void UpdateDeviceName(string deviceName)
		{
			if (deviceName.Length > 60)
				throw new Exception("Device name must be 60 characters or less");
			byte[] DataToSend = new byte[deviceName.Length + 2];
			DataToSend[0] = (byte)DeviceCommands.FirmwareUpdateName;
			DataToSend[1] = (byte)(deviceName.Length); // Unicode 16-bit strings are 2 bytes per character
			byte[] stringData = Encoding.UTF8.GetBytes(deviceName);
			stringData.CopyTo(DataToSend, 2);
			sendPeripheralConfigPacket(DataToSend);
			//Thread.Sleep(100); // wait a bit for the flash operation to finish (global interrupts are disabled during programming)
		}

		~TreehopperUsb()
		{
			Dispose();
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
		/// This property is used internally by <see cref="TreehopperManager"/> to match unique devices. Attaching two devices with the same
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
		/// This list contains all the pins that belong to the board. 
		/// </summary>
		/// <remarks>
		/// The list is zero-indexed, so <see cref="Treehopper.Pin1"/> is available at Pins[0], <see cref="Treehopper.Pin2"/> is available at Pins[1], and so on.
		/// </remarks>
		public Dictionary<int, Pin> Pins { get; set; }

		/// <summary>
		/// Open the TreehopperBoard. The board must be opened before any other methods are called.
		/// </summary>
		public async Task<bool> Connect()
		{
            bool res = await connection.Open();
            if (!res)
            {
                IsConnected = false;
                return false;
            }
                

            if (IsConnected)
                return true;

			pinStateBuffer = new byte[64];

			Pins = new Dictionary<int, Pin>();

			// Initialize Pins
			pin1	= new Pin1(this);
			pin2	= new Pin2(this);
			pin3	= new Pin3(this);
			pin4	= new Pin4(this);
			pin5	= new Pin5(this);
			pin6	= new Pin6(this);
			pin7	= new Pin7(this);
			pin8	= new Pin8(this);
			pin9	= new Pin9(this);
			pin10	= new Pin10(this);
			pin11	= new Pin11(this);
			pin12	= new Pin12(this);
			pin13	= new Pin13(this);
			pin14	= new Pin14(this);

			Pins.Add(1, pin1);
			Pins.Add(2, pin2);
			Pins.Add(3, pin3);
			Pins.Add(4, pin4);
			Pins.Add(5, pin5);
			Pins.Add(6, pin6);
			Pins.Add(7, pin7);
			Pins.Add(8, pin8);
			Pins.Add(9, pin9);
			Pins.Add(10, pin10);
			Pins.Add(11, pin11);
			Pins.Add(12, pin12);
			Pins.Add(13, pin13);
			Pins.Add(14, pin14);

			SoftPwmMgr = new SoftPwmManager(this);
            PwmManager = new PwmManager(this);

			// Initialize modules
			i2c = new I2c(this);
			spi = new Spi(this);
            uart = new Uart(this);

            //UART = new UART();


            connection.PinEventDataReceived += Connection_PinEventDataReceived; 
			
			this.IsConnected = true;
			return true;
		}


        private void Connection_PinEventDataReceived(byte[] pinStateBuffer)
        {
            if (pinStateBuffer[0] == (byte)DeviceResponse.CurrentReadings)
            {
                int i = 1;
                foreach(Pin pin in Pins.Values)
                {
                    pin.UpdateValue(pinStateBuffer[i++], pinStateBuffer[i++]);
                }


                /// Pin interrupts.
                /// TODO: This is really hacky and needs to be cleaned up.
                int PortAInterrupt = pinStateBuffer[29];
                int PortBInterrupt = pinStateBuffer[30];
                if ((PortAInterrupt & (1 << 4)) > 0) // RA4 = Pin1
                    Pin1.RaiseDigitalInValueChanged();
                if ((PortAInterrupt & (1 << 5)) > 0) // RA5 = Pin14
                    Pin14.RaiseDigitalInValueChanged();

                if ((PortBInterrupt & (1 << 7)) > 0) // RB7 = Pin8
                    Pin8.RaiseDigitalInValueChanged();
                if ((PortBInterrupt & (1 << 7)) > 0) // RB5 = Pin9
                    Pin9.RaiseDigitalInValueChanged();
                if ((PortBInterrupt & (1 << 7)) > 0) // RB4 = Pin10
                    Pin10.RaiseDigitalInValueChanged();
                if ((PortBInterrupt & (1 << 7)) > 0) // RB6 = Pin11
                    Pin11.RaiseDigitalInValueChanged();
            }
        }

        public bool IsConnected { get; internal set; }

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

		internal async Task<byte[]> receiveCommsResponsePacket(uint bytesToRead)
		{
			return await connection.ReadPeripheralResponsePacket(bytesToRead);
		}

		/// <summary>
		/// Close device
		/// </summary>
		/// <remarks>
		/// This will reset all pins to inputs and then disconnect from the device. It will not free memory associated with this device.
		/// </remarks>
		public void Disconnect()
		{
            if(connection != null)
                connection.Close();
            IsConnected = false;
			//if(usb.IsOpen)
			//{
			//	if (SoftPwmMgr != null)
			//		SoftPwmMgr.Dispose();
			//}
			//Disconnect();
			//this.IsConnected = false;
		}

        /// <summary>
        /// Close device connection and free memory. 
        /// </summary>
        public void Dispose()
        {
            Disconnect();
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
}
