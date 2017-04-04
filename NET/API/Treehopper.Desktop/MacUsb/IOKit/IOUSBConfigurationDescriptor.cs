using System;
using System.Runtime.InteropServices;

namespace Treehopper.Desktop
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class IOUSBConfigurationDescriptor
	{
		public byte bLength;
		public byte bDescriptorType;
		public UInt16 wTotalLength;
		public byte bNumInterfaces;
		public byte bConfigurationValue;
		public byte iConfiguration;
		public byte bmAttributes;
		public byte MaxPower;
	}
}
