using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Treehopper.Desktop
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	[SuppressMessage("ReSharper", "InconsistentNaming")]
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
