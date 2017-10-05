using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Treehopper.Desktop.MacUsb.IOKit;

namespace Treehopper.Desktop.MacUsb
{
    class DeviceWatcher
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

            var kr = NativeMethods.IOServiceAddMatchingNotification(
                gNotifyPort,                            // notifyPort
                IOKitFramework.kIOFirstMatchNotification,       // notificationType
                matchingDict,                           // matching
                DeviceAdded,                            // callback
                IntPtr.Zero,                            // refCon
                ref gAddedIter                          // notification
                );

            DeviceAdded(IntPtr.Zero, gAddedIter);

            NativeMethods.CFRunLoopRun();
        }

        private void DeviceRemoved(int data, IntPtr service, uint messageType, IntPtr messageArgument)
        {
            if (messageType == IOKitFramework.kIOMessageServiceIsTerminated)
            {
                Debug.WriteLine("Device Removed");
                OnDeviceRemoved(data);
            }
        }

        private async void DeviceAdded(IntPtr refCon, IntPtr iterator)
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

                var kr = NativeMethods.IOServiceAddInterestNotification(
                    gNotifyPort,                // notifyPort
                    usbDevice.Handle,                   // service
                    IOKitFramework.kIOGeneralInterest,  // interestType
                    DeviceRemoved,                      // callback
                    idx,                            // refCon
                    ref notificationPtr             // notification
                    );

                usbDevice = it.Next();
            }
        }
    }
}
