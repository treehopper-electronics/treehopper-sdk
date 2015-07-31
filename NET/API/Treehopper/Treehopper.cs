using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using LibUsbDotNet;
using LibUsbDotNet.Main;
using System.ComponentModel;
using System.Text.RegularExpressions;

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
		SoftPwmConfig,	// 
		SoftPwmUpdateDc,	//
		FirmwareUpdateSerial,	//
		FirmwareUpdateName,	//
		Reboot,	//
		EnterBootloader	//
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
	public class TreehopperUSB : INotifyPropertyChanged, IDisposable, IComparable, IEquatable<TreehopperUSB>, IEqualityComparer<TreehopperUSB>
	{
		// USB stuff
		UsbDevice usb;
		UsbEndpointWriter PinConfig;
		UsbEndpointReader pinState;
		UsbEndpointWriter CommsConfig;
		UsbEndpointReader CommsReceive;

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

		//public UART UART;

		I2c i2c;
		/// <summary>
		/// I2C module
		/// </summary>
		public I2c I2C { get { return i2c; } }

		Spi spi;
		/// <summary>
		/// SPI module
		/// </summary>
		public Spi SPI { get { return spi;	} }

		/// <summary>
		/// Instance of SoftPwmMgr
		/// </summary>
		internal SoftPwmManager SoftPwmMgr { get; set; }

        internal PwmManager PwmMgr { get; set; }

		// Comparator modules
		//Comparator Comparator1;
		//Comparator Comparator2;

		#endregion

		string deviceName;
		string serialNumber;

		byte[] pinStateBuffer;

		/// <summary>
		/// Gets a reference to the first board connected to the computer
		/// </summary>
		/// <returns></returns>
		public static TreehopperUSB First()
		{
			InitTreehopperManager();
			if (manager.Boards.Count > 0)
				return manager.Boards[0];
			else
				return null;
		}

		private static void InitTreehopperManager()
		{
			if (manager == null)
			{
				manager = new TreehopperManager();
				manager.BoardAdded += manager_BoardAdded;
				manager.BoardRemoved += manager_BoardRemoved;
			}
		}

		private static event BoardEventHandler PreBoardAdded;
		private static event BoardEventHandler PreBoardRemoved; 
		public static event BoardEventHandler BoardAdded
		{
			add
			{
				PreBoardAdded += value;
				InitTreehopperManager();
			}
			remove
			{
				PreBoardRemoved -= value;
			}
		}
		public static event BoardEventHandler BoardRemoved
		{
			add
			{
				PreBoardRemoved += value;
				InitTreehopperManager();
			}
			remove
			{
				PreBoardRemoved -= value;
			}
		}

		static void manager_BoardRemoved(TreehopperManager sender, TreehopperUSB board)
		{
			if (PreBoardRemoved != null) PreBoardRemoved(sender, board);
		}

		static void manager_BoardAdded(TreehopperManager sender, TreehopperUSB board)
		{
			if (PreBoardAdded != null) PreBoardAdded(sender, board);
		}

		private static TreehopperManager manager;

		internal TreehopperUSB(UsbDevice device)
		{
			usb = device;
			// TODO: Complete member initialization
			if(usb != null)
			{
				usb.GetString(out serialNumber, 0x0409, 3);
				if (serialNumber != null)
				{
					serialNumber = Regex.Replace(serialNumber, @"[^\u0000-\u007F]", string.Empty);
					serialNumber = serialNumber.Replace("\0", String.Empty);
				}

				usb.GetString(out deviceName, 0x0409, 4);
				if (deviceName != null)
					deviceName = deviceName.Replace("\0", String.Empty);
			}
		}

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
		/// <param name="serialNumber">A 15-character (or fewer) string containing the new serial number</param>
		/// <remarks>
		/// While the name is immediately written to the device and the Name property is updated immediately, the changes 
		/// will not take effect to other applications until the device is reset. This can be done by calling <see cref="Reset"/>
		/// </remarks>
		public void UpdateSerialNumber(string serialNumber)
		{
			if (serialNumber.Length > 15)
				throw new Exception("String must be 15 characters or less");

			byte[] bytes = Encoding.Unicode.GetBytes(serialNumber);

			byte[] DataToSend = new byte[bytes.Length + 3];
			DataToSend[0] = (byte)DeviceCommands.FirmwareUpdateSerial;
			DataToSend[1] = (byte)(serialNumber.Length * 2 + 2); // Unicode 16-bit strings are 2 bytes per character
			DataToSend[2] = 3;
			bytes.CopyTo(DataToSend, 3);
			sendPeripheralConfigPacket(DataToSend);
			Thread.Sleep(100); // wait a bit for the flash operation to finish (global interrupts are disabled during programming)
			this.serialNumber = serialNumber;
			RaisePropertyChanged("SerialNumber");

		}

		/// <summary>
		/// Update the name of the device.
		/// </summary>
		/// <param name="deviceName">A 15-character (or fewer) string containing the new name of the device.</param>
		/// <remarks>
		/// While the name is immediately written to the device and the Name property is updated immediately, the changes 
		/// will not take effect to other applications until the device is reset. This can be done by calling <see cref="Reset"/>
		/// </remarks>
		public void UpdateDeviceName(string deviceName)
		{
			if (deviceName.Length > 15)
				throw new Exception("Serial number must be 15 characters or less");
			byte[] DataToSend = new byte[deviceName.Length * 2 + 3];
			DataToSend[0] = (byte)DeviceCommands.FirmwareUpdateName;
			DataToSend[1] = (byte)(deviceName.Length * 2 + 2); // Unicode 16-bit strings are 2 bytes per character
			DataToSend[2] = 3;
			byte[] stringData = Encoding.Unicode.GetBytes(deviceName);
			stringData.CopyTo(DataToSend, 3);
			sendPeripheralConfigPacket(DataToSend);
			Thread.Sleep(100); // wait a bit for the flash operation to finish (global interrupts are disabled during programming)
			this.deviceName = deviceName;
			RaisePropertyChanged("DeviceName");
		}

		~TreehopperUSB()
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
				return serialNumber;
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
				return deviceName;
			}
		}

		/// <summary>
		/// A human-readable description of the board, containing the Name and serial identification.
		/// </summary>
		public string Description
		{
			get
			{
				return this.ToString();
			}
		}

		/// <summary>
		/// This list contains all the pins that belong to the board. 
		/// </summary>
		/// <remarks>
		/// The list is zero-indexed, so <see cref="Treehopper.Pin1"/> is available at Pins[0], <see cref="Treehopper.Pin2"/> is available at Pins[1], and so on.
		/// </remarks>
		public List<Pin> Pins { get; set; }

		/// <summary>
		/// Open the TreehopperBoard. The board must be opened before any other methods are called.
		/// </summary>
		public void Open()
		{
			if(usb.Open())
			{
				// If this is a "whole" usb device (libusb-win32, linux libusb)
				// it will have an IUsbDevice interface. If not (WinUSB) the 
				// variable will be null indicating this is an interface of a 
				// device.
				IUsbDevice wholeUsbDevice = usb as IUsbDevice;
				if (!ReferenceEquals(wholeUsbDevice, null))
				{
					// This is a "whole" USB device. Before it can be used, 
					// the desired configuration and interface must be selected.

					// Select config #1
					wholeUsbDevice.SetConfiguration(1);

					// Claim interface #0.
					wholeUsbDevice.ClaimInterface(0);
				}

				pinStateBuffer = new byte[64];

				Pins = new List<Pin>();

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

				Pins.Add(pin1);
				Pins.Add(pin2);
				Pins.Add(pin3);
				Pins.Add(pin4);
				Pins.Add(pin5);
				Pins.Add(pin6);
				Pins.Add(pin7);
				Pins.Add(pin8);
				Pins.Add(pin9);
				Pins.Add(pin10);
				Pins.Add(pin11);
				Pins.Add(pin12);
				Pins.Add(pin13);
				Pins.Add(pin14);

				SoftPwmMgr = new SoftPwmManager(this);
                PwmMgr = new PwmManager(this);

				// Comparator
				//Comparator1 = new Comparator(1);
				//Comparator2 = new Comparator(2);

				// Initialize modules
				i2c = new I2c(this);
				spi = new Spi(this);
				//UART = new UART();

				// Initialize endpoint readers/writers
				PinConfig	= usb.OpenEndpointWriter(WriteEndpointID.Ep01);
				pinState	= usb.OpenEndpointReader(ReadEndpointID.Ep01);
				CommsConfig	= usb.OpenEndpointWriter(WriteEndpointID.Ep02);
				CommsReceive	= usb.OpenEndpointReader(ReadEndpointID.Ep02);
			
				// Start reader events
				pinState.DataReceived += pinState_DataReceived;
				pinState.DataReceivedEnabled = true;
				this.IsConnected = true;
			}
			else
			{
				if (usb != null)
				{
					if (usb.IsOpen) usb.Close();
					usb = null;
				}
			}
		}

		public bool IsConnected { get; internal set; }

		void pinState_DataReceived(object sender, EndpointDataEventArgs e)
		{
			int transferLength;
			pinState.Read(pinStateBuffer, 1000, out transferLength);
			if (pinStateBuffer[0] == (byte)DeviceResponse.CurrentReadings)
			{
				Pin1.UpdateValue(	pinStateBuffer[1],	pinStateBuffer[2]);
				Pin2.UpdateValue(	pinStateBuffer[3],	pinStateBuffer[4]);
				Pin3.UpdateValue(	pinStateBuffer[5],	pinStateBuffer[6]);
				Pin4.UpdateValue(	pinStateBuffer[7],	pinStateBuffer[8]);
				Pin5.UpdateValue(	pinStateBuffer[9],	pinStateBuffer[10]);
				Pin6.UpdateValue(	pinStateBuffer[11],	pinStateBuffer[12]);
				Pin7.UpdateValue(	pinStateBuffer[13],	pinStateBuffer[14]);
				Pin8.UpdateValue(	pinStateBuffer[15],	pinStateBuffer[16]);
				Pin9.UpdateValue(	pinStateBuffer[17],	pinStateBuffer[18]);
				Pin10.UpdateValue(	pinStateBuffer[19],	pinStateBuffer[20]);
				Pin11.UpdateValue(	pinStateBuffer[21],	pinStateBuffer[22]);
				Pin12.UpdateValue(	pinStateBuffer[23],	pinStateBuffer[24]);
				Pin13.UpdateValue(	pinStateBuffer[25],	pinStateBuffer[26]);
				Pin14.UpdateValue(	pinStateBuffer[27],	pinStateBuffer[28]);

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

		internal void sendPinConfigPacket(byte[] data)
		{
			if (PinConfig == null)
				return;
			if (PinConfig.IsDisposed)
				return;
			if (data.Count() > 64)
				throw new Exception("Data must be less than or equal to 64 bytes");
			int transferLength;
			ErrorCode error = PinConfig.Write(data, 1000, out transferLength);
			if(error != ErrorCode.None && error != ErrorCode.IoCancelled)
			{
				Debug.WriteLine(error);
			}
		}

		internal void sendPeripheralConfigPacket(byte[] data)
		{
			if (CommsConfig == null)
				return;
			if (CommsConfig.IsDisposed)
				return;
			if (data.Count() > 64)
				throw new Exception("Data must be less than or equal to 64 bytes");
			int transferLength;
			ErrorCode error;

			if ((error = CommsConfig.Write(data, 1000, out transferLength)) != ErrorCode.None)
			{
				Debug.WriteLine(error);
			}
		}

		internal byte[] receiveCommsResponsePacket()
		{
			byte[] returnVal = new byte[64];
			int transferLength;
			if(CommsReceive != null)
				CommsReceive.Read(returnVal, 1000, out transferLength);
			return returnVal;
		}

		internal void Disconnect()
		{
			try { 
				if (PinConfig != null)
				{
					PinConfig.Abort();
					PinConfig.Dispose();
					PinConfig = null;
				}
				if (pinState != null)
				{
					pinState.DataReceivedEnabled = false;
					pinState.DataReceived -= pinState_DataReceived; // TODO: sometimes pinState is null when we get here. 
					pinState.Dispose();
					pinState = null;
				}
				if (CommsConfig != null)
				{
					CommsConfig.Abort();
					CommsConfig.Dispose();
					CommsConfig = null;
				}
				if (CommsReceive != null)
				{
					CommsReceive.Dispose();
					CommsReceive = null;
				}
			}
			catch (Exception e)
			{

			}

			if (usb != null)
			{
				if (usb.IsOpen)
				{
					IUsbDevice wholeUsbDevice = usb as IUsbDevice;
					if (!ReferenceEquals(wholeUsbDevice, null))
					{
						wholeUsbDevice.ReleaseInterface(0);
					}
					usb.Close();
				}
			}

		}

		/// <summary>
		/// Close device
		/// </summary>
		/// <remarks>
		/// This will reset all pins to inputs and then disconnect from the device. It will not free memory associated with this device.
		/// </remarks>
		public void Close()
		{
			if(usb.IsOpen)
			{
				if (SoftPwmMgr != null)
					SoftPwmMgr.Dispose();
			}
			Disconnect();
			this.IsConnected = false;
		}

		/// <summary>
		/// Close device connection and free memory. 
		/// </summary>
		public void Dispose()
		{
			Disconnect();
			UsbExit();
		}

		public static void UsbExit()
		{
			UsbDevice.Exit();
		}

		/// <summary>
		/// Compares another Treehopper to this one.
		/// </summary>
		/// <param name="obj">The other Treehopper</param>
		/// <returns></returns>
		public int CompareTo(object obj)
		{
			TreehopperUSB board = obj as TreehopperUSB;
			if (board == null)
				return 1;
			return String.Compare(this.serialNumber, board.serialNumber);
		}

		/// <summary>
		/// A human-readable string describing this instance. This is formed from the name and serial number of the device.
		/// </summary>
		/// <returns>The string</returns>
		public override string ToString()
		{
			return deviceName + " (" + serialNumber + ")";
		}

		/// <summary>
		/// Determines if another Treehopper board is the same as this one
		/// </summary>
		/// <param name="other">The other Treehopper board to compare</param>
		/// <returns>True if the two boards are the same</returns>
		public bool Equals(TreehopperUSB other)
		{
			return (other.SerialNumber == this.SerialNumber);
		}
		/// <summary>
		/// Determines if two Treehopper boards are the same
		/// </summary>
		/// <param name="x">The first board</param>
		/// <param name="y">The second board</param>
		/// <returns></returns>
		public bool Equals(TreehopperUSB x, TreehopperUSB y)
		{
			if (x == null || y == null)
				return false;
			return (x.serialNumber == y.serialNumber);
		}
		/// <summary>
		/// Hash, obtained from the serial number of the device.
		/// </summary>
		/// <param name="board">the Treehopper board to calculate the hash for</param>
		/// <returns>The hash code for the board</returns>
		/// <see cref="Object.GetHashCode"/>
		public int GetHashCode(TreehopperUSB board)
		{
			if (board != null)
				if (board.SerialNumber != null)
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
			TreehopperUSB y = obj as TreehopperUSB;
			if (y == null)
				return false;
			else
				return this.serialNumber == y.serialNumber;
		}


		/// <summary>
		/// Occurs whenever a property changes.
		/// </summary>
		/// <remarks>
		/// </remarks>
		public event PropertyChangedEventHandler PropertyChanged;

		internal void RaisePropertyChanged(string property)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(property));
		}
	}

	/// <summary>
	/// This comparer is used to compare Treehopper boards
	/// </summary>
	public class TreehopperBoardEqualityComparer : EqualityComparer<TreehopperUSB>
	{
		/// <summary>
		/// Determines if two treehopper boards are the same
		/// </summary>
		/// <param name="x">The first board to compare</param>
		/// <param name="y">The second board to compare</param>
		/// <returns>Whether the boards are equal</returns>
		public override bool Equals(TreehopperUSB x, TreehopperUSB y)
		{
			if (x == null || y == null)
				return false;
			return (x.SerialNumber == y.SerialNumber);
		}

		/// <summary>
		/// Returns the Hash of the Treehopper board
		/// </summary>
		/// <param name="obj">The Treehopper board to calculate the hash for</param>
		/// <returns>The calculated hash value</returns>
		public override int GetHashCode(TreehopperUSB obj)
		{
			if (obj != null)
				if (obj.SerialNumber != null)
					return obj.SerialNumber.GetHashCode();
				else
					return 0;
			else
				return 0;
		}
	}
}
