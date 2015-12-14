using System;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Treehopper
{
	///  <summary>
	///  Routines for detecting devices.
	///  </summary>

	sealed internal partial class DeviceManagement
	{
		private const String ModuleName = "Device Management";

		internal static void DisplayException(String Name, Exception e)
		{

			//  Create an error message.

			String message = "Exception: " + e.Message + Environment.NewLine + "Module: " + Name + Environment.NewLine + "Method: " +
						e.TargetSite.Name;
            Debug.Print(message);
		}
	
		internal Boolean FindDeviceFromGuid(Guid myGuid, ref String devicePathName)
		{
			Int32 bufferSize = 0;
			IntPtr detailDataBuffer = IntPtr.Zero;
			var deviceInfoSet = new IntPtr();
			Boolean lastDevice = false;
			var myDeviceInterfaceData = new NativeMethods.SP_DEVICE_INTERFACE_DATA();

			try
			{
				deviceInfoSet = NativeMethods.SetupDiGetClassDevs(ref myGuid, IntPtr.Zero, IntPtr.Zero, NativeMethods.DIGCF_PRESENT | NativeMethods.DIGCF_DEVICEINTERFACE);

				Boolean deviceFound = false;
				Int32 memberIndex = 0;
			
				myDeviceInterfaceData.cbSize = Marshal.SizeOf(myDeviceInterfaceData);
				
				do
				{
					Boolean success = NativeMethods.SetupDiEnumDeviceInterfaces
						(deviceInfoSet,
						 IntPtr.Zero,
						 ref myGuid,
						 memberIndex,
						 ref myDeviceInterfaceData);

					if (!success)
					{
						lastDevice = true;

					}
					else
					{
						NativeMethods.SetupDiGetDeviceInterfaceDetail
							(deviceInfoSet,
							 ref myDeviceInterfaceData,
							 IntPtr.Zero,
							 0,
							 ref bufferSize,
							 IntPtr.Zero);

						detailDataBuffer = Marshal.AllocHGlobal(bufferSize);

						Marshal.WriteInt32(detailDataBuffer, (IntPtr.Size == 4) ? (4 + Marshal.SystemDefaultCharSize) : 8);
						
						NativeMethods.SetupDiGetDeviceInterfaceDetail
							(deviceInfoSet,
							 ref myDeviceInterfaceData,
							 detailDataBuffer,
							 bufferSize,
							 ref bufferSize,
							 IntPtr.Zero);

						// Skip over cbsize (4 bytes) to get the address of the devicePathName.

						var pDevicePathName = new IntPtr(detailDataBuffer.ToInt64() + 4);

						// Get the String containing the devicePathName.

						devicePathName = Marshal.PtrToStringAuto(pDevicePathName);
						
						deviceFound = true;
					}
					memberIndex = memberIndex + 1;
				}
				while (lastDevice != true);

				return deviceFound;
			}
					
			
			catch (Exception ex)
			{
				DisplayException(ModuleName, ex);
				throw;
			}

			finally
			{
				if (detailDataBuffer != IntPtr.Zero)
				{
					// Free the memory allocated previously by AllocHGlobal.

					Marshal.FreeHGlobal(detailDataBuffer);
				}
				if (deviceInfoSet != IntPtr.Zero)
				{
					NativeMethods.SetupDiDestroyDeviceInfoList(deviceInfoSet);
				}
			}
		}			
	}
}
