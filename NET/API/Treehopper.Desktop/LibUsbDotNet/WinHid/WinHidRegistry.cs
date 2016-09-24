using LibUsbDotNet.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibUsbDotNet;
using LibUsbDotNet.Internal;
using System.Runtime.InteropServices;
using static LibUsbDotNet.WinHid.Hid;

namespace LibUsbDotNet.WinHid
{
    public class WinHidRegistry : UsbRegistry
    {
        private WinHidDevice hidUsbDevice = null;

        internal static Guid GetHidGuid()
        {
            Guid hidGuid = Guid.Empty;
            try
            {
                //  ***
                //  API function: 'HidD_GetHidGuid

                //  Purpose: Retrieves the interface class GUID for the HID class.

                //  Accepts: A System.Guid object for storing the GUID.
                //  ***

                NativeMethods.HidD_GetHidGuid(ref hidGuid);
            }

            catch (Exception ex)
            {
                throw;
            }
            return hidGuid;
        }


        /// <summary>
        /// Gets a list of available LibUsb devices.
        /// </summary>
        public static List<WinHidRegistry> DeviceList
        {
            get
            {
                List<WinHidRegistry> deviceList = new List<WinHidRegistry>();
                Guid hid = GetHidGuid();
                SetupApi.EnumClassDevs(null, SetupApi.DIGCF.PRESENT | SetupApi.DIGCF.DEVICEINTERFACE, HidUsbRegistryCallBack, deviceList, hid);
                return deviceList;
            }
        }

        private static bool HidUsbRegistryCallBack(IntPtr DeviceInfoSet, int deviceIndex, ref SetupApi.SP_DEVINFO_DATA DeviceInfoData, object classEnumeratorCallbackParam1)
        {
            List<WinHidRegistry> deviceList = (List<WinHidRegistry>)classEnumeratorCallbackParam1;
            Guid hid = GetHidGuid();
            int devicePathIndex = 0;
            SetupApi.SP_DEVICE_INTERFACE_DATA interfaceData = SetupApi.SP_DEVICE_INTERFACE_DATA.Empty;
            SetupApi.DeviceInterfaceDetailHelper detailHelper;

            SetupApi.SP_DEVINFO_DATA devInfoData = SetupApi.SP_DEVINFO_DATA.Empty;

            // [1]
            IntPtr deviceInfo = SetupApi.SetupDiGetClassDevs(ref hid, null, IntPtr.Zero, SetupApi.DIGCF.PRESENT | SetupApi.DIGCF.DEVICEINTERFACE);
            if (deviceInfo != IntPtr.Zero)
            {
                while ((SetupApi.SetupDiEnumDeviceInterfaces(deviceInfo, null, ref hid, devicePathIndex, ref interfaceData)))
                {
                    int length = 1024;
                    detailHelper = new SetupApi.DeviceInterfaceDetailHelper(length);
                    bool bResult = SetupApi.SetupDiGetDeviceInterfaceDetail(deviceInfo, ref interfaceData, detailHelper.Handle, length, out length, ref devInfoData);
                    if (bResult)
                    {
                        WinHidRegistry regInfo = new WinHidRegistry();

                        SetupApi.getSPDRPProperties(deviceInfo, ref devInfoData, regInfo.mDeviceProperties);

                        // Use the actual winusb device path for SYMBOLIC_NAME_KEY. This will be used to open the device.
                        regInfo.mDeviceProperties.Add(SYMBOLIC_NAME_KEY, detailHelper.DevicePath);

                        // Debug.WriteLine(detailHelper.DevicePath);

                        regInfo.mDeviceInterfaceGuids = new Guid[] { hid };

                        StringBuilder sbDeviceID = new StringBuilder(1024);
                        if (SetupApi.CM_Get_Device_ID(devInfoData.DevInst, sbDeviceID, sbDeviceID.Capacity, 0) == SetupApi.CR.SUCCESS)
                        {
                            regInfo.mDeviceProperties[DEVICE_ID_KEY] = sbDeviceID.ToString();
                        }
                        deviceList.Add(regInfo);
                    }

                    devicePathIndex++;
                }
            }

            if (devicePathIndex == 0)
                UsbError.Error(ErrorCode.Win32Error, Marshal.GetLastWin32Error(), "GetDevicePathList", typeof(SetupApi));

            if (deviceInfo != IntPtr.Zero)
                SetupApi.SetupDiDestroyDeviceInfoList(deviceInfo);

            return (devicePathIndex > 0);

        }

        /// <summary>
        /// Opens the USB device for communucation.
        /// </summary>
        /// <param name="usbDevice">Returns an opened WinUsb device on success, null on failure.</param>
        /// <returns>True on success.</returns>
        public bool Open(out WinHidDevice usbDevice)
        {
            usbDevice = null;

            if (String.IsNullOrEmpty(SymbolicName)) return false;
            if (WinHidDevice.Open(SymbolicName, out usbDevice))
            {
                usbDevice.mUsbRegistry = this;
                return true;
            }
            return false;
        }

        public override bool Open(out UsbDevice usbDevice)
        {
            usbDevice = null;
            WinHidDevice winUsbDevice;
            bool bSuccess = Open(out winUsbDevice);
            if (bSuccess)
                usbDevice = winUsbDevice;
            return bSuccess;
        }

        public override UsbDevice Device
        {
            get
            {
                if (hidUsbDevice == null)
                    Open(out hidUsbDevice);
                return hidUsbDevice;
            }
        }

        public override Guid[] DeviceInterfaceGuids
        {
            get
            {
                return mDeviceInterfaceGuids;
            }
        }

        public override bool IsAlive
        {
            get
            {
                if (String.IsNullOrEmpty(SymbolicName)) throw new UsbException(this, "A symbolic name is required for this property.");

                List<WinHidRegistry> deviceList = DeviceList;
                foreach (WinHidRegistry registry in deviceList)
                {
                    if (String.IsNullOrEmpty(registry.SymbolicName)) continue;

                    if (registry.SymbolicName == SymbolicName)
                        return true;
                }
                return false;
            }
        }
    }
}
