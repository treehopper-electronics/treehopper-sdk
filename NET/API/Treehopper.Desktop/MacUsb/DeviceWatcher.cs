using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Treehopper.Desktop.MacUsb.IOKit;
using System.Runtime.InteropServices;

namespace Treehopper.Desktop.MacUsb
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class DeviceWatcherCallbackReference
    {
        public IntPtr removedCallback;
        public int deviceIndex;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class DeviceWatcher
    {
        protected DeviceAdded OnDeviceAdded = null;
        protected DeviceRemoved OnDeviceRemoved = null;
        IntPtr notificationPtr = new IntPtr();

        //readonly List<GCHandle> handles = new List<GCHandle>();

        IntPtr gNotifyPort;
        IntPtr gRunLoop;

        public DeviceWatcher(DeviceAdded onAdded, DeviceRemoved onRemoved)
        {
            this.OnDeviceAdded = onAdded;
            this.OnDeviceRemoved = onRemoved;
        }

        public void StopScanner()
        {
            NativeMethods.CFRunLoopStop(gRunLoop);
        }

        public void RunScanner()
        {
            int usbVendor = (int)TreehopperUsb.Settings.Vid;
            int usbProduct = (int)TreehopperUsb.Settings.Pid;

            IntPtr gAddedIter = IntPtr.Zero;

            var matchingDict = NativeMethods.IOServiceMatching(IOKitFramework.kIOUSBDeviceClassName);

            if (matchingDict == IntPtr.Zero)
                throw new Exception("IOServiceMatching returned NULL");

            var numberRef = NativeMethods.CFNumberCreate(IOKitFramework.kCFAllocatorDefault, CFNumberType.kCFNumberSInt32Type, ref usbVendor);
            NativeMethods.CFDictionarySetValue(matchingDict, NativeMethods.__CFStringMakeConstantString(IOKitFramework.kUSBVendorID), numberRef);
            NativeMethods.CFRelease(numberRef);

            numberRef = NativeMethods.CFNumberCreate(IOKitFramework.kCFAllocatorDefault, CFNumberType.kCFNumberSInt32Type, ref usbProduct);
            NativeMethods.CFDictionarySetValue(matchingDict, NativeMethods.__CFStringMakeConstantString(IOKitFramework.kUSBProductID), numberRef);
            NativeMethods.CFRelease(numberRef);

            gNotifyPort = NativeMethods.IONotificationPortCreate(IOKitFramework.kIOMasterPortDefault);
            var runLoopSource = NativeMethods.IONotificationPortGetRunLoopSource(gNotifyPort);

            gRunLoop = NativeMethods.CFRunLoopGetCurrent();
            NativeMethods.CFRunLoopAddSource(gRunLoop, runLoopSource, NativeMethods.__CFStringMakeConstantString(IOKitFramework.kCFRunLoopDefaultMode));

            IOServiceMatchingCallback deviceAdded = new IOServiceMatchingCallback(DeviceAdded);

            DeviceWatcherCallbackReference callbackRef = new DeviceWatcherCallbackReference();


            var kr = NativeMethods.IOServiceAddMatchingNotification(
                gNotifyPort,                            // notifyPort
                IOKitFramework.kIOFirstMatchNotification,       // notificationType
                matchingDict,                           // matching
                DeviceAdded,                            // callback
                callbackRef,                            // refCon
                ref gAddedIter                          // notification
                );

            DeviceAdded(callbackRef, gAddedIter);

            NativeMethods.CFRunLoopRun();
        }

        private void DeviceRemoved(DeviceWatcherCallbackReference data, IntPtr service, uint messageType, IntPtr messageArgument)
        {
            if (messageType == IOKitFramework.kIOMessageServiceIsTerminated)
            {
                Debug.WriteLine("Device Removed");
                var removed = (DeviceRemoved)(GCHandle.FromIntPtr(data.removedCallback).Target);
                removed(data.deviceIndex);
            }
        }

        // why does this have to be async?
        private async void DeviceAdded(DeviceWatcherCallbackReference callbackRef, IntPtr iterator)
        {
            var it = new IOIterator(iterator);

            var usbDevice = it.Next();

            while (usbDevice != null)
            {

                var vendorString = usbDevice.GetCFPropertyString(IOKitFramework.kUSBVendorString);
                var productString = usbDevice.GetCFPropertyString(IOKitFramework.kUSBProductString);
                var serialNumber = usbDevice.GetCFPropertyString(IOKitFramework.kUSBSerialNumberString);

                Debug.WriteLine("Found Device:");
                Debug.WriteLine("Vendor: " + vendorString);
                Debug.WriteLine("Product: " + productString);
                Debug.WriteLine("Serial Number: " + serialNumber);
                Debug.WriteLine("");

                var idx = OnDeviceAdded(usbDevice, productString, serialNumber);

                var ptr = GCHandle.ToIntPtr(GCHandle.Alloc(this.OnDeviceRemoved, GCHandleType.Pinned));
                var removedCallback = new DeviceWatcherCallbackReference() { deviceIndex = idx, removedCallback = ptr };

                GCHandle.Alloc(removedCallback, GCHandleType.Pinned);
                GCHandle.Alloc(this, GCHandleType.Pinned);
                GCHandle.Alloc(this.OnDeviceRemoved, GCHandleType.Pinned);

                var kr = NativeMethods.IOServiceAddInterestNotification(
                    gNotifyPort,                // notifyPort
                    usbDevice.Handle,                   // service
                    IOKitFramework.kIOGeneralInterest,  // interestType
                    DeviceRemoved,                      // callback
                    removedCallback,                            // refCon
                    ref notificationPtr             // notification
                    );

                usbDevice = it.Next();
            }
        }
    }
}
