using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using System.Diagnostics;
using System.Runtime.ConstrainedExecution;
using System.Security.Permissions;
using System.Text;

namespace Treehopper
{
    /// <summary>
    ///  Routines for the WinUsb driver.
    ///  </summary>
    ///  <remarks>
    /// </remarks>

    internal sealed partial class WinUsbCommunications
    {
        SafeWinUsbHandle winUsbHandle = new SafeWinUsbHandle();
        DeviceInfo myDeviceInfo = new DeviceInfo();
        /// <summary>
        /// Handle to pass to WinUsb_Initialize.
        /// Adapted from the example in Microsoft's SafeHandle documentation.
        /// </summary>

        [SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode = true)]
        [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
        internal class SafeWinUsbHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            // Create a SafeHandle, informing the base class
            // that this SafeHandle instance "owns" the handle,
            // and therefore SafeHandle should call
            // our ReleaseHandle method when the SafeHandle
            // is no longer in use.

            internal SafeWinUsbHandle()
                : base(true)
            {
                base.SetHandle(handle);
                this.handle = IntPtr.Zero;
            }

            /// <summary>
            /// Call WinUsb_Free on releasing the handle.
            /// </summary>
            /// <returns>
            /// True on success.
            /// </returns>
            /// 
            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
            protected override bool ReleaseHandle()
            {
                if (!this.IsInvalid)
                {
                    this.handle = IntPtr.Zero;
                }
                return NativeMethods.WinUsb_Free(handle);
            }

            // The IsInvalid property must be overridden. 

            public override bool IsInvalid
            {
                get
                {
                    if (handle == IntPtr.Zero)
                    {
                        return true;
                    }
                    if (handle == (IntPtr)(-1))
                    {
                        return true;
                    }
                    return false;
                }
            }

            public IntPtr GetHandle()
            {
                if (IsInvalid)
                {
                    throw new Exception("The handle is invalid.");
                }
                return handle;
            }
        }

        private const String ModuleName = "WinUSB Device";

        internal class DeviceInfo
        {
            internal Byte BulkInPipe;
            internal Byte BulkOutPipe;
            internal Byte InterruptInPipe;
            internal Byte InterruptOutPipe;
            internal Byte IsochronousInPipe;
            internal Byte IsochronousOutPipe;
            internal UInt32 DeviceSpeed;
        }

        ///  <summary>
        ///  Closes the device handle obtained with CreateFile and frees resources.
        ///  </summary>
        ///  <param name="deviceHandle"> Device handle obtained with CreateFile </param>
        ///  <param name="winUsbHandle"> Handle for accessing the WinUSB device </param>

        internal void CloseDeviceHandle(SafeFileHandle deviceHandle)
        {
            try
            {
                if (!winUsbHandle.IsInvalid)
                {
                    var thisLock = new Object();

                    lock (thisLock)
                    {
                        winUsbHandle.Close();
                    }
                }
                if (deviceHandle != null)
                {
                    if (!(deviceHandle.IsInvalid))
                    {
                        deviceHandle.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayException(ModuleName, ex);
                throw;
            }
        }

        ///  <summary>
        ///  Provides a central mechanism for exception handling.
        ///  Displays a message box that describes the exception.
        ///  </summary>
        ///  
        ///  <param name="name"> the module where the exception occurred. </param>
        ///  <param name="e"> the exception </param>

        internal static void DisplayException(String name, Exception e)
        {
            try
            {
                //  Create an error message.

                String message = "Exception: " + e.Message + Environment.NewLine + "Module: " + name + Environment.NewLine + "Method: " +
                          e.TargetSite.Name;

                const String caption = "Unexpected Exception";

                Debug.Write(message);
            }
            catch (Exception ex)
            {
                DisplayException(ModuleName, ex);
                throw;
            }
        }

        ///  <summary>
        ///  Initiates a Control Read transfer. Data stage is device to host.
        ///  </summary>
        /// 
        ///  <param name="winUsbHandle"> Handle for accessing the WinUSB device </param>
        ///  <param name="dataStage"> The received data </param>
        ///  
        ///  <returns>
        ///  True on success, False on failure.
        ///  </returns>

        internal Boolean DoControlReadTransfer(ref Byte[] dataStage)
        {
            UInt32 bytesReturned = 0;

            try
            {
                //  Vendor-specific request to an interface with device-to-host Data stage.

                NativeMethods.WINUSB_SETUP_PACKET setupPacket;
                setupPacket.RequestType = 0XC1;

                //  The request number that identifies the specific request.

                setupPacket.Request = 2;

                // Command-specific value to send to the device.
                // For a request directed to an interface, the WinUSB driver uses this field to specify the interface number
                // and will ignore a value set here.
                // For a request directed to the device, you can use this field for vendor-defined purposes.

                setupPacket.Index = 0;

                //  Number of bytes in the request's Data stage.

                setupPacket.Length = Convert.ToUInt16(dataStage.Length);

                //  Command-specific value to send to the device.

                setupPacket.Value = 0;

                var success = false;

                var thisLock = new Object();

                lock (thisLock)
                {
                    // ***
                    //  winusb function 

                    //  summary
                    //  Initiates a control transfer.

                    //  paramaters
                    //  Device handle returned by WinUsb_Initialize.
                    //  WINUSB_SETUP_PACKET structure 
                    //  Buffer to hold the returned Data-stage data.
                    //  Number of data bytes to read in the Data stage.
                    //  Number of bytes read in the Data stage.
                    //  Null pointer for non-overlapped.

                    //  returns
                    //  True on success.
                    //  ***            

                    if (!(winUsbHandle.IsInvalid))
                    {
                        success = NativeMethods.WinUsb_ControlTransfer(winUsbHandle, setupPacket, dataStage,
                                                                           Convert.ToUInt16(dataStage.Length), ref bytesReturned,
                                                                           IntPtr.Zero);
                    }
                    return success;
                }

            }
            catch (Exception ex)
            {
                DisplayException(ModuleName, ex);
                throw;
            }
        }

        public string GetStringDescriptor(byte stringIndex)
        {
            string stringData = string.Empty;
            IntPtr stringPtr = Marshal.AllocHGlobal(255);

            int transferLength;
            bool Success = NativeMethods.WinUsb_GetDescriptor(winUsbHandle, (byte)DescriptorType.String, stringIndex, 0, stringPtr, 255, out transferLength);
            if(Success)
            {
                if (transferLength == 255) // funky string data
                    return stringData;
                byte[] bytes = new byte[transferLength];
                Marshal.Copy(stringPtr, bytes, 0, bytes.Length);
                stringData = Encoding.Unicode.GetString(bytes, 2, bytes.Length - 2);

            }
            return stringData;
        }

        ///  <summary>
        ///  Initiates a Control Write transfer. Data stage is host to device.
        ///  </summary>
        ///  
        ///  <param name="winUsbHandle"> Handle for accessing the WinUSB device </param> 
        ///  <param name="dataStage"> The data to send. </param>
        ///  
        ///  <returns>
        ///  True on success, False on failure.
        ///  </returns>

        internal Boolean DoControlWriteTransfer(Byte[] dataStage)
        {
            uint bytesReturned = 0;
            var value = Convert.ToUInt16(0);

            try
            {
                //  Vendor-specific request to an interface with host-to-device Data stage.

                NativeMethods.WINUSB_SETUP_PACKET setupPacket;
                setupPacket.RequestType = 0X41;

                //  The request number that identifies the specific request.

                setupPacket.Request = 1;

                // Command-specific value to send to the device.
                // For a request directed to an interface, the WinUSB driver uses this field to specify the interface number.
                // and will ignore a value set here.
                // For a request directed to the device, you can use this field for vendor-defined purposes.

                setupPacket.Index = 0;

                //  Number of bytes in the request's Data stage.

                setupPacket.Length = Convert.ToUInt16(dataStage.Length);

                //  Command-specific value to send to the device.

                setupPacket.Value = value;
                var success = false;
                var thisLock = new Object();

                lock (thisLock)
                {
                    // ***
                    //  winusb function 

                    //  summary
                    //  Initiates a control transfer.

                    //  parameters
                    //  Device handle returned by WinUsb_Initialize.
                    //  WINUSB_SETUP_PACKET structure 
                    //  Buffer containing the Data-stage data.
                    //  Number of data bytes to send in the Data stage.
                    //  Number of bytes sent in the Data stage.
                    //  Null pointer for non-overlapped.

                    //  Returns
                    //  True on success.
                    //  ***

                    if (!(winUsbHandle.IsInvalid))
                    {
                        success = NativeMethods.WinUsb_ControlTransfer
                            (winUsbHandle,
                             setupPacket,
                             dataStage,
                             Convert.ToUInt16(dataStage.Length),
                             ref bytesReturned,
                             IntPtr.Zero);
                    }
                    return success;
                }
            }
            catch (Exception ex)
            {
                DisplayException(ModuleName, ex);
                throw;
            }
        }

        ///  <summary>
        ///  Requests a handle with CreateFile.
        ///  </summary>
        ///  
        ///  <param name="devicePathName"> Returned by SetupDiGetDeviceInterfaceDetail 
        ///  in an SP_DEVICE_INTERFACE_DETAIL_DATA structure. </param>
        ///  
        ///  <returns>
        ///  The handle.
        ///  </returns>

        internal SafeFileHandle GetDeviceHandle(String devicePathName)
        {
            try
            {
                // ***
                // API function

                //  summary
                //  Retrieves a handle to a device.

                //  parameters 
                //  Device path name returned by SetupDiGetDeviceInterfaceDetail
                //  Type of access requested (read/write).
                //  FILE_SHARE attributes to allow other processes to access the device while this handle is open.
                //  Security structure. Using Null for this may cause problems under Windows XP.
                //  Creation disposition value. Use OPEN_EXISTING for devices.
                //  Flags and attributes for files. The winsub driver requires FILE_FLAG_OVERLAPPED.
                //  Handle to a template file. Not used.

                //  Returns
                //  A handle or INVALID_HANDLE_VALUE.
                // ***

                SafeFileHandle deviceHandle = FileIo.CreateFile
                    (devicePathName,
                     (FileIo.GENERIC_WRITE | FileIo.GENERIC_READ),
                     FileIo.FILE_SHARE_READ | FileIo.FILE_SHARE_WRITE,
                     IntPtr.Zero,
                     FileIo.OPEN_EXISTING,
                     FileIo.FILE_ATTRIBUTE_NORMAL | FileIo.FILE_FLAG_OVERLAPPED,
                     IntPtr.Zero);
                return deviceHandle;
            }
            catch (Exception ex)
            {
                DisplayException(ModuleName, ex);
                throw;
            }
        }

        internal Boolean InitializeDevice(string path, UInt32 pipeTimeout)
        {
            SafeFileHandle handle = this.GetDeviceHandle(path);
            return InitializeDevice(handle, pipeTimeout);
        }

        ///  <summary>
        ///  Initializes a device interface and obtains information about it.
        ///  Calls these winusb API functions:
        ///    WinUsb_Initialize
        ///    WinUsb_QueryInterfaceSettings
        ///    WinUsb_QueryPipe
        ///  </summary>
        ///  
        ///  <param name="deviceHandle"> Device handle obtained with CreateFile </param>
        ///  <param name="winUsbHandle"> Handle for accessing the WinUSB device </param>
        ///  <param name="myDeviceInfo"> devInfo structure for the device </param>	
        ///  <param name="pipeTimeout"> desired timeout in milliseconds for transfers </param>
        ///  
        ///  <returns>
        ///  True on success, False on failure.
        ///  </returns>
        internal Boolean InitializeDevice(SafeFileHandle deviceHandle, UInt32 pipeTimeout)
        {
            try
            {
                // These arrays hold descriptors and pipe information for interface zero, alternate settings 0 and 1.

                var ifaceDescriptors = new NativeMethods.USB_INTERFACE_DESCRIPTOR[2];
                var pipeInfo = new NativeMethods.WINUSB_PIPE_INFORMATION[2];

                // ***
                //  winusb function 

                //  summary
                //  get a handle for communications with a winusb device        '

                //  parameters
                //  Handle returned by CreateFile.
                //  Device handle to be returned.

                //  returns
                //  True on success.
                //  ***

                // Lock access while attempting to create winUsbHandle.

                var thisLock = new Object();

                lock (thisLock)
                {
                    var success = NativeMethods.WinUsb_Initialize
                        (deviceHandle, ref winUsbHandle);

                    if (success)
                    {
                        // ***
                        //  winusb function 

                        //  summary
                        //  Get a structure with information about the device interface.

                        //  parameters
                        //  handle returned by WinUsb_Initialize
                        //  alternate interface setting number
                        //  USB_INTERFACE_DESCRIPTOR structure to be returned.

                        //  returns
                        //  True on success.

                        // Get information for the interface, including alternate setting 1 if present.
                        // Switch to alternate setting 1 if present.

                        for (var interfaceSetting = 0; interfaceSetting <= 1; interfaceSetting++)
                        {
                            success = NativeMethods.WinUsb_QueryInterfaceSettings
                                (winUsbHandle,
                                 (Byte)interfaceSetting,
                                 ref ifaceDescriptors[interfaceSetting]);

                            if (success)
                            {
                                //  Get the transfer type, endpoint number, and direction for the interface's
                                //  endpoints. Set pipe policies. Repeat for the interface's alternate interface number.

                                // ***
                                //  winusb function 

                                //  summary
                                //  returns information about a USB pipe (endpoint address)

                                //  parameters
                                //  Handle returned by WinUsb_Initialize
                                //  Alternate interface setting number
                                //  Number of an endpoint address associated with the interface. 
                                //  (The values count up from zero and are NOT the same as the endpoint address
                                //  in the endpoint descriptor.)
                                //  WINUSB_PIPE_INFORMATION structure to be returned

                                //  returns
                                //  True on success   
                                // ***

                                for (var i = 0; i <= ifaceDescriptors[interfaceSetting].bNumEndpoints - 1; i++)
                                {
                                    NativeMethods.WinUsb_QueryPipe
                                        (winUsbHandle,
                                         (Byte)interfaceSetting,
                                         Convert.ToByte(i),
                                         ref pipeInfo[interfaceSetting]);

                                    if (((pipeInfo[interfaceSetting].PipeType ==
                                          NativeMethods.USBD_PIPE_TYPE.UsbdPipeTypeBulk) &
                                         UsbEndpointDirectionIn(pipeInfo[interfaceSetting].PipeId)))
                                    {
                                        myDeviceInfo.BulkInPipe = pipeInfo[interfaceSetting].PipeId;

                                        SetPipePolicy(
                                             myDeviceInfo.BulkInPipe,
                                             Convert.ToUInt32(NativeMethods.POLICY_TYPE.IGNORE_SHORT_PACKETS),
                                             Convert.ToByte(false));

                                        SetPipePolicy(
                                             myDeviceInfo.BulkInPipe,
                                             Convert.ToUInt32(NativeMethods.POLICY_TYPE.PIPE_TRANSFER_TIMEOUT),
                                             pipeTimeout);
                                    }
                                    else if (((pipeInfo[interfaceSetting].PipeType ==
                                               NativeMethods.USBD_PIPE_TYPE.UsbdPipeTypeBulk) &
                                              UsbEndpointDirectionOut(pipeInfo[interfaceSetting].PipeId)))
                                    {
                                        myDeviceInfo.BulkOutPipe = pipeInfo[interfaceSetting].PipeId;

                                        SetPipePolicy
                                            (
                                             myDeviceInfo.BulkOutPipe,
                                             Convert.ToUInt32(NativeMethods.POLICY_TYPE.IGNORE_SHORT_PACKETS),
                                             Convert.ToByte(false));

                                        SetPipePolicy
                                            (
                                             myDeviceInfo.BulkOutPipe,
                                             Convert.ToUInt32(NativeMethods.POLICY_TYPE.PIPE_TRANSFER_TIMEOUT),
                                             pipeTimeout);
                                    }
                                    else if ((pipeInfo[interfaceSetting].PipeType ==
                                              NativeMethods.USBD_PIPE_TYPE.UsbdPipeTypeInterrupt) &
                                             UsbEndpointDirectionIn(pipeInfo[interfaceSetting].PipeId))
                                    {
                                        myDeviceInfo.InterruptInPipe = pipeInfo[interfaceSetting].PipeId;

                                        SetPipePolicy
                                            (
                                             myDeviceInfo.InterruptInPipe,
                                             Convert.ToUInt32(NativeMethods.POLICY_TYPE.IGNORE_SHORT_PACKETS),
                                             Convert.ToByte(false));

                                        SetPipePolicy
                                            (
                                             myDeviceInfo.InterruptInPipe,
                                             Convert.ToUInt32(NativeMethods.POLICY_TYPE.PIPE_TRANSFER_TIMEOUT),
                                             pipeTimeout);
                                    }
                                    else if ((pipeInfo[interfaceSetting].PipeType ==
                                              NativeMethods.USBD_PIPE_TYPE.UsbdPipeTypeInterrupt) &
                                             UsbEndpointDirectionOut(pipeInfo[interfaceSetting].PipeId))
                                    {
                                        myDeviceInfo.InterruptOutPipe = pipeInfo[interfaceSetting].PipeId;

                                        SetPipePolicy
                                            (
                                             myDeviceInfo.InterruptOutPipe,
                                             Convert.ToUInt32(NativeMethods.POLICY_TYPE.IGNORE_SHORT_PACKETS),
                                             Convert.ToByte(false));

                                        SetPipePolicy
                                            (
                                             myDeviceInfo.InterruptOutPipe,
                                             Convert.ToUInt32(NativeMethods.POLICY_TYPE.PIPE_TRANSFER_TIMEOUT),
                                             pipeTimeout);
                                    }

                                    else if ((pipeInfo[interfaceSetting].PipeType ==
                                              NativeMethods.USBD_PIPE_TYPE.UsbdPipeTypeIsochronous) &
                                             UsbEndpointDirectionIn(pipeInfo[interfaceSetting].PipeId))
                                    {
                                        myDeviceInfo.IsochronousInPipe = pipeInfo[interfaceSetting].PipeId;

                                        SetPipePolicy
                                            (
                                             myDeviceInfo.IsochronousInPipe,
                                             Convert.ToUInt32(NativeMethods.POLICY_TYPE.IGNORE_SHORT_PACKETS),
                                             Convert.ToByte(false));

                                        SetPipePolicy
                                            (
                                             myDeviceInfo.IsochronousInPipe,
                                             Convert.ToUInt32(NativeMethods.POLICY_TYPE.PIPE_TRANSFER_TIMEOUT),
                                             pipeTimeout);
                                    }
                                    else if ((pipeInfo[interfaceSetting].PipeType ==
                                              NativeMethods.USBD_PIPE_TYPE.UsbdPipeTypeIsochronous) &
                                             UsbEndpointDirectionOut(pipeInfo[interfaceSetting].PipeId))
                                    {
                                        myDeviceInfo.IsochronousOutPipe = pipeInfo[interfaceSetting].PipeId;

                                        SetPipePolicy
                                            (
                                             myDeviceInfo.IsochronousOutPipe,
                                             Convert.ToUInt32(NativeMethods.POLICY_TYPE.IGNORE_SHORT_PACKETS),
                                             Convert.ToByte(false));

                                        SetPipePolicy
                                            (
                                             myDeviceInfo.IsochronousOutPipe,
                                             Convert.ToUInt32(NativeMethods.POLICY_TYPE.PIPE_TRANSFER_TIMEOUT),
                                             pipeTimeout);
                                    }
                                }
                            }
                            if (interfaceSetting == 1)
                            {
                                // If possible, switch to alternate setting 1 to enable isochrononous transfers.

                                success = NativeMethods.WinUsb_SetCurrentAlternateSetting
                                    (winUsbHandle,
                                     ifaceDescriptors[1].bAlternateSetting);
                            }
                        }
                    }
                    return success;
                }
            }

            catch (Exception ex)
            {
                DisplayException(ModuleName, ex);
                throw;
            }
        }

        ///  <summary>
        ///  Gets a value that corresponds to a USB_DEVICE_SPEED. 
        ///  </summary>
        ///  
        ///  <param name="winUsbHandle"> Handle for accessing the WinUSB device </param>
        ///  <param name="myDevInfo"> devInfo structure for the device </param>

        internal Boolean QueryDeviceSpeed()
        {
            UInt32 length = 1;
            var speed = new Byte[1];

            try
            {
                var success = false;
                var thisLock = new Object();

                lock (thisLock)
                {
                    // ***
                    //  winusb function 

                    //  summary
                    //  Get the device speed. 
                    //  (Normally not required but can be nice to know.)

                    //  parameters
                    //  Handle returned by WinUsb_Initialize
                    //  Requested information type.
                    //  Number of bytes to read.
                    //  Information to be returned.

                    //  returns
                    //  True on success.
                    // ***           			

                    if (!(winUsbHandle.IsInvalid))
                    {
                        success = NativeMethods.WinUsb_QueryDeviceInformation
                            (winUsbHandle,
                             NativeMethods.DEVICE_SPEED,
                             ref length,
                             ref speed[0]);
                    }
                    if (success)
                    {
                        myDeviceInfo.DeviceSpeed = Convert.ToUInt32(speed[0]);
                    }

                    return success;
                }
            }
            catch (Exception ex)
            {
                DisplayException(ModuleName, ex);
                throw;
            }
        }

        ///  <summary>
        ///  Attempts to read data from a bulk IN endpoint.
        ///  </summary>
        ///  
        ///  <param name="winUsbHandle"> Handle for accessing the WinUSB device </param>
        ///  <param name="myDeviceInfo"> devInfo structure for the device </param>		
        ///  <param name="bytesToRead"> Number of bytes to read. </param>
        ///  <param name="dataBuffer"> Buffer for storing the bytes read. </param>
        ///  <param name="bytesRead"> Number of bytes read. </param>
        ///  <param name="success"> Success or failure status. </param>
        ///  
        internal void ReceiveDataViaBulkTransfer(UInt32 bytesToRead,
                                                 ref Byte[] dataBuffer, ref UInt32 bytesRead, ref Boolean success)
        {
            try
            {
                var thisLock = new Object();

                lock (thisLock)
                {
                    // ***
                    //  winusb function 

                    //  summary
                    //  Attempts to read data from a device interface.

                    //  parameters
                    //  Device handle returned by WinUsb_Initialize.
                    //  Endpoint address.
                    //  Buffer to store the data.
                    //  Maximum number of bytes to return.
                    //  Number of bytes read.
                    //  Null pointer for non-overlapped.

                    //  Returns
                    //  True on success.
                    // ***

                    success = false;

                    if (!(winUsbHandle.IsInvalid))
                    {
                        success = NativeMethods.WinUsb_ReadPipe
                            (winUsbHandle,
                             myDeviceInfo.BulkInPipe,
                             dataBuffer,
                             bytesToRead,
                             ref bytesRead,
                             IntPtr.Zero);
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayException(ModuleName, ex);
                throw;
            }
        }

        ///  <summary>
        ///  Attempts to read data from an interrupt IN endpoint. 
        ///  </summary>
        ///  
        ///  <param name="winUsbHandle"> Handle for accessing the WinUSB device </param>
        ///  <param name="myDeviceInfo"> devInfo structure for the device </param>		
        ///  <param name="bytesToRead"> Number of bytes to read. </param>
        ///  <param name="dataBuffer"> Buffer for storing the bytes read. </param>
        ///  <param name="bytesRead"> Number of bytes read. </param>
        ///  
        ///  <returns> true on success, false on failure </returns>
        ///  
        internal Boolean ReceiveDataViaInterruptTransfer(UInt32 bytesToRead,
                                                         ref Byte[] dataBuffer, ref UInt32 bytesRead)
        {
            try
            {
                var success = false;
                var thisLock = new Object();

                lock (thisLock)
                {
                    // ***
                    //  winusb function 

                    //  summary
                    //  Attempts to read data from a device interface.

                    //  parameters
                    //  Device handle returned by WinUsb_Initialize.
                    //  Endpoint address.
                    //  Buffer to store the data.
                    //  Maximum number of bytes to return.
                    //  Number of bytes read.
                    //  Null pointer for non-overlapped.

                    //  Returns
                    //  True on success.
                    // ***

                    if (!(winUsbHandle.IsInvalid))
                    {
                        success = NativeMethods.WinUsb_ReadPipe
                            (winUsbHandle,
                             myDeviceInfo.InterruptInPipe,
                             dataBuffer,
                             bytesToRead,
                             ref bytesRead,
                             IntPtr.Zero);
                    }

                    return success;
                }
            }
            catch (Exception ex)
            {
                DisplayException(ModuleName, ex);
                throw;
            }
        }

        ///  <summary>
        ///  Attempts to read data from an isochronous IN endpoint. 
        ///  </summary>
        ///  
        ///  <param name="winUsbHandle"> Handle for accessing the WinUSB device </param>
        ///  <param name="myDeviceInfo"> devInfo structure for the device </param>		
        ///  <param name="bytesToRead"> Number of bytes to read. </param>
        ///  <param name="dataInBuffer"> Buffer for storing the bytes read. </param>
        ///  <param name="bytesRead"> Number of bytes read. </param>
        ///  
        ///  <returns> true on success, false on failure </returns>
        ///  
        internal Boolean ReceiveDataViaIsochronousTransfer(UInt32 bytesToRead,
                                                         ref Byte[] dataInBuffer, ref UInt32 bytesRead, UInt32 numberOfPackets)
        {
            try
            {
                var success = false;
                var thisLock = new Object();

                // Array that holds information about each received packet.

                var isoPacketDescriptors = new NativeMethods.USBD_ISO_PACKET_DESCRIPTOR[numberOfPackets];

                lock (thisLock)
                {
                    if (!(winUsbHandle.IsInvalid))
                    {
                        IntPtr bufferHandle;

                        success = NativeMethods.WinUsb_RegisterIsochBuffer
                            (winUsbHandle,
                            myDeviceInfo.IsochronousInPipe,
                            dataInBuffer,
                            (UInt32)dataInBuffer.Length,
                            out bufferHandle);

                        success = NativeMethods.WinUsb_ReadIsochPipeAsap
                            (bufferHandle,
                            0,
                            (UInt32)dataInBuffer.Length,
                            false,
                            numberOfPackets,
                            ref isoPacketDescriptors[0],
                            IntPtr.Zero);

                        System.Console.WriteLine(Marshal.GetLastWin32Error());

                        success = NativeMethods.WinUsb_UnregisterIsochBuffer(bufferHandle);

                        for (var i = 0; i <= numberOfPackets - 1; i++)
                        {
                            Debug.WriteLine("packet offset = " + isoPacketDescriptors[i].Offset);
                            Debug.WriteLine("packet length = " + isoPacketDescriptors[i].Length);
                            Debug.WriteLine("packet status = " + isoPacketDescriptors[i].Status);
                            for (var j = 0; j < isoPacketDescriptors[i].Length; j++)
                            {
                                Debug.WriteLine(dataInBuffer[j]);
                            }
                        }
                    }
                    return success;
                }
            }
            catch (Exception ex)
            {
                DisplayException(ModuleName, ex);
                throw;
            }
        }

        ///  <summary>
        ///  Attempts to send data via a bulk OUT endpoint.
        ///  </summary>
        ///  
        ///  <param name="winUsbHandle"> Handle for accessing the WinUSB device </param>
        ///  <param name="myDeviceInfo"> devInfo structure for the device </param> 		
        ///  <param name="bytesToWrite"> Number of bytes to write. </param>
        ///  <param name="dataBuffer"> Buffer containing the bytes to write. </param>
        ///  <param name="bytesWritten"> The number of bytes written </param>
        ///  <param name="success"> True on success </param>

        internal void SendDataViaBulkTransfer(UInt32 bytesToWrite,
                                              Byte[] dataBuffer, ref UInt32 bytesWritten, ref Boolean success)
        {
            try
            {
                var thisLock = new Object();

                lock (thisLock)
                {
                    // ***
                    //  winusb function 

                    //  summary
                    //  Attempts to write data to a device interface.

                    //  parameters
                    //  Device handle returned by WinUsb_Initialize.
                    //  Endpoint address.
                    //  Buffer with data to write.
                    //  Number of bytes to write.
                    //  Number of bytes written.
                    //  IntPtr.Zero for non-overlapped I/O.

                    //  Returns
                    //  True on success.
                    //  ***

                    success = false;

                    if (!(winUsbHandle.IsInvalid))
                    {
                        success = NativeMethods.WinUsb_WritePipe
                            (winUsbHandle,
                             myDeviceInfo.BulkOutPipe,
                             dataBuffer,
                             bytesToWrite,
                             ref bytesWritten,
                             IntPtr.Zero);
                    }
                    if (!success)
                    {
                        System.Console.WriteLine(Marshal.GetLastWin32Error());
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayException(ModuleName, ex);
                throw;
            }
        }

        ///  <summary>
        ///  Attempts to send data via an interrupt OUT endpoint.
        ///  </summary>
        ///
        ///  <param name="winUsbHandle"> Handle for accessing the WinUSB device </param>  
        ///  <param name="myDeviceInfo"> devInfo structure for the device </param> 		
        ///  <param name="bytesToWrite"> Number of bytes to write. </param>
        ///  <param name="dataBuffer"> Buffer containing the bytes to write. </param>
        ///  <param name="bytesWritten"> The number of bytes written </param>		
        ///  
        ///  <returns>
        ///  True on success, False on failure.
        ///  </returns>

        internal Boolean SendDataViaInterruptTransfer(UInt32 bytesToWrite,
                                                      Byte[] dataBuffer, ref UInt32 bytesWritten)
        {
            try
            {
                var success = false;

                var thisLock = new Object();

                lock (thisLock)
                {
                    // ***
                    //  winusb function 

                    //  summary
                    //  Attempts to write data to a device interface.

                    //  parameters
                    //  Device handle returned by WinUsb_Initialize.
                    //  Endpoint address.
                    //  Buffer with data to write.
                    //  Number of bytes to write.
                    //  Number of bytes written.
                    //  IntPtr.Zero for non-overlapped I/O.

                    //  Returns
                    //  True on success.
                    //  ***

                    if (!(winUsbHandle.IsInvalid))
                    {
                        success = NativeMethods.WinUsb_WritePipe
                            (winUsbHandle,
                             myDeviceInfo.InterruptOutPipe,
                             dataBuffer,
                             bytesToWrite,
                             ref bytesWritten,
                             IntPtr.Zero);
                    }
                    return success;
                }
            }
            catch (Exception ex)
            {
                DisplayException(ModuleName, ex);
                throw;
            }
        }

        ///  <summary>
        ///  Attempts to send data via an isochronous OUT endpoint.
        ///  </summary>
        ///
        ///  <param name="winUsbHandle"> Handle for accessing the WinUSB device </param>  
        ///  <param name="myDeviceInfo"> devInfo structure for the device </param> 		
        ///  <param name="bytesToWrite"> Number of bytes to write. </param>
        ///  <param name="dataOutBuffer"> Buffer containing the bytes to write. </param>
        ///  <param name="bytesWritten"> The number of bytes written </param>		
        ///  
        ///  <returns>
        ///  True on success, False on failure.
        ///  </returns>

        internal Boolean SendDataViaIsochronousTransfer(UInt32 bytesToWrite,
                                                      Byte[] dataOutBuffer, ref UInt32 bytesWritten)
        {
            try
            {
                var success = false;

                var thisLock = new Object();

                lock (thisLock)
                {
                    if (!(winUsbHandle.IsInvalid))
                    {
                        IntPtr bufferHandle;

                        success = NativeMethods.WinUsb_RegisterIsochBuffer
                            (winUsbHandle,
                            myDeviceInfo.IsochronousOutPipe,
                            dataOutBuffer,
                            (UInt32)dataOutBuffer.Length,
                            out bufferHandle);

                        success = NativeMethods.WinUsb_WriteIsochPipeAsap
                            (bufferHandle,
                            0,
                            (UInt32)dataOutBuffer.Length,
                            false,
                            IntPtr.Zero);

                        success = NativeMethods.WinUsb_UnregisterIsochBuffer(bufferHandle);
                    }
                    return success;
                }
            }
            catch (Exception ex)
            {
                DisplayException(ModuleName, ex);
                throw;
            }
        }

        ///  <summary>
        ///  Sets pipe policy.
        ///  Used when the value parameter is a Byte (all except PIPE_TRANSFER_TIMEOUT).
        ///  </summary>
        /// 
        ///  <param name="winUsbHandle"> Handle for accessing the WinUSB device </param>
        /// <param name="pipeId"> Pipe to set a policy for. </param>
        ///  <param name="policyType"> POLICY_TYPE member. </param>
        ///  <param name="value"> Policy value. </param>
        ///  
        ///  <returns>
        ///  True on success, False on failure.
        ///  </returns>
        ///  
        private Boolean SetPipePolicy(Byte pipeId, UInt32 policyType, Byte value)
        {
            try
            {
                var success = false;

                var thisLock = new Object();

                lock (thisLock)
                {
                    // ***
                    //  winusb function 

                    //  summary
                    //  sets a pipe policy 

                    //  parameters
                    //  handle returned by WinUsb_Initialize
                    //  identifies the pipe
                    //  POLICY_TYPE member.
                    //  length of value in bytes
                    //  value to set for the policy.

                    //  returns
                    //  True on success 
                    // ***

                    if (!(winUsbHandle.IsInvalid))
                    {
                        success = NativeMethods.WinUsb_SetPipePolicy
                            (winUsbHandle,
                             pipeId,
                             policyType,
                             1,
                             ref value);
                    }

                    return success;
                }
            }
            catch (Exception ex)
            {
                DisplayException(ModuleName, ex);
                throw;
            }
        }

        ///  <summary>
        ///  Sets pipe policy.
        ///  Used when the value parameter is a UInt32 (PIPE_TRANSFER_TIMEOUT only).
        ///  </summary>
        /// 
        /// <param name="winUsbHandle"> Handle for accessing the WinUSB device </param>
        /// <param name="pipeId"> Pipe to set a policy for. </param>
        ///  <param name="policyType"> POLICY_TYPE member. </param>
        ///  <param name="value"> Policy value. </param>
        ///  
        ///  <returns>
        ///  True on success, False on failure.
        ///  </returns>
        ///  
        private Boolean SetPipePolicy(Byte pipeId, UInt32 policyType, UInt32 value)
        {
            try
            {
                var success = false;

                var thisLock = new Object();

                lock (thisLock)
                {
                    // ***
                    //  winusb function 

                    //  summary
                    //  sets a pipe policy 

                    //  parameters
                    //  handle returned by WinUsb_Initialize
                    //  identifies the pipe
                    //  POLICY_TYPE member.
                    //  length of value in bytes
                    //  value to set for the policy.

                    //  returns
                    //  True on success 
                    // ***

                    if (!(winUsbHandle.IsInvalid))
                    {
                        success = NativeMethods.WinUsb_SetPipePolicy1
                            (winUsbHandle,
                             pipeId,
                             policyType,
                             4,
                             ref value);
                    }
                    return success;
                }
            }
            catch (Exception ex)
            {
                DisplayException(ModuleName, ex);
                throw;
            }
        }

        ///  <summary>
        ///  Is the endpoint's direction IN (device to host)?
        ///  </summary>
        ///  
        ///  <param name="endpointAddress"> The endpoint address. </param>
        ///  
        ///  <returns>
        ///  True if IN (device to host), False if OUT (host to device)
        ///  </returns> 

        private Boolean UsbEndpointDirectionIn(Int32 endpointAddress)
        {
            try
            {

                var directionIn = false;

                if (((endpointAddress & 0X80) == 0X80))
                {
                    directionIn = true;
                }
                return directionIn;
            }

            catch (Exception ex)
            {
                DisplayException(ModuleName, ex);
                throw;
            }
        }

        ///  <summary>
        ///  Is the endpoint's direction OUT (host to device)?
        ///  </summary>
        ///  
        ///  <param name="addr"> The endpoint address. </param>
        ///  
        ///  <returns>
        ///  True if OUT (host to device, False if IN (device to host)
        ///  </returns>

        private Boolean UsbEndpointDirectionOut(Int32 addr)
        {
            try
            {
                var directionOut = false;

                if (((addr & 0X80) == 0))
                {
                    directionOut = true;
                }
                return directionOut;
            }


            catch (Exception ex)
            {
                DisplayException(ModuleName, ex);
                throw;
            }
        }

    }

    /// <summary> Standard USB descriptor types.
    /// </summary> 
    [Flags]
    public enum DescriptorType : byte
    {
        /// <summary>
        /// Device descriptor type.
        /// </summary>
        Device = 1,
        /// <summary>
        /// Configuration descriptor type.
        /// </summary>
        Configuration = 2,
        /// <summary>
        /// String descriptor type.
        /// </summary>
        String = 3,
        /// <summary>
        /// Interface descriptor type.
        /// </summary>
        Interface = 4,
        /// <summary>
        /// Endpoint descriptor type.
        /// </summary>
        Endpoint = 5,
        /// <summary>
        /// Device Qualifier descriptor type.
        /// </summary>
        DeviceQualifier = 6,
        /// <summary>
        /// Other Speed Configuration descriptor type.
        /// </summary>
        OtherSpeedConfiguration = 7,
        /// <summary>
        /// Interface Power descriptor type.
        /// </summary>
        InterfacePower = 8,
        /// <summary>
        /// OTG descriptor type.
        /// </summary>
        OTG = 9,
        /// <summary>
        /// Debug descriptor type.
        /// </summary>
        Debug = 10,
        /// <summary>
        /// Interface Association descriptor type.
        /// </summary>
        InterfaceAssociation = 11,

        ///<summary> HID descriptor</summary>
        Hid = 0x21,

        ///<summary> HID report descriptor</summary>
        HidReport = 0x22,

        ///<summary> Physical descriptor</summary>
        Physical = 0x23,

        ///<summary> Hub descriptor</summary>
        Hub = 0x29
    }
}
