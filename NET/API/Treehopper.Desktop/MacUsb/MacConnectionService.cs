using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Threading;
using Treehopper.Desktop.MacUsb.IOKit;

namespace Treehopper.Desktop.MacUsb
{
    public class MacUsbConnectionService : ConnectionService
    {
        private readonly List<GCHandle> handles = new List<GCHandle>();

        private readonly Thread listener;
        private IntPtr gNotifyPort;
        private IntPtr gRunLoop;

        private Dispatcher mainDispatcher;

        private IntPtr notificationPtr;

        public MacUsbConnectionService()
        {
            mainDispatcher = Dispatcher.CurrentDispatcher;

            var usbVendor = (int) TreehopperUsb.Settings.Vid;
            var usbProduct = (int) TreehopperUsb.Settings.Pid;

            var gAddedIter = IntPtr.Zero;

            var matchingDict = NativeMethods.IOServiceMatching(IOKitFramework.kIOUSBDeviceClassName);

            if (matchingDict == IntPtr.Zero)
                throw new Exception("IOServiceMatching returned NULL");

            var numberRef = NativeMethods.CFNumberCreate(IOKitFramework.kCFAllocatorDefault,
                CFNumberType.kCFNumberSInt32Type, ref usbVendor);
            NativeMethods.CFDictionarySetValue(matchingDict,
                NativeMethods.__CFStringMakeConstantString(IOKitFramework.kUSBVendorID), numberRef);
            NativeMethods.CFRelease(numberRef);

            numberRef = NativeMethods.CFNumberCreate(IOKitFramework.kCFAllocatorDefault,
                CFNumberType.kCFNumberSInt32Type, ref usbProduct);
            NativeMethods.CFDictionarySetValue(matchingDict,
                NativeMethods.__CFStringMakeConstantString(IOKitFramework.kUSBProductID), numberRef);
            NativeMethods.CFRelease(numberRef);

            // Create a notification port and add its run loop event source to our run loop
            // This is how async notifications get set up.


            listener = new Thread(() =>
            {
                gNotifyPort = NativeMethods.IONotificationPortCreate(IOKitFramework.kIOMasterPortDefault);
                var runLoopSource = NativeMethods.IONotificationPortGetRunLoopSource(gNotifyPort);

                gRunLoop = NativeMethods.CFRunLoopGetCurrent();
                NativeMethods.CFRunLoopAddSource(gRunLoop, runLoopSource,
                    NativeMethods.__CFStringMakeConstantString(IOKitFramework.kCFRunLoopDefaultMode));

                IOServiceMatchingCallback deviceAdded = DeviceAdded;

                var kr = NativeMethods.IOServiceAddMatchingNotification(
                    gNotifyPort, // notifyPort
                    IOKitFramework.kIOFirstMatchNotification, // notificationType
                    matchingDict, // matching
                    DeviceAdded, // callback
                    IntPtr.Zero, // refCon
                    ref gAddedIter // notification
                );

                DeviceAdded(IntPtr.Zero, gAddedIter);

                NativeMethods.CFRunLoopRun();
            });

            listener.Name = "CFRunLoop thread";

            listener.Start();
        }

        private void DeviceRemoved(int data, IntPtr service, uint messageType, IntPtr messageArgument)
        {
            if (messageType == IOKitFramework.kIOMessageServiceIsTerminated)
            {
                Debug.WriteLine("Device removed: " + Boards[data].SerialNumber);
                Boards[data].Dispose();
                Boards.RemoveAt(data);
            }
        }

        private void DeviceAdded(IntPtr refCon, IntPtr iterator)
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
                var board = new TreehopperUsb(new MacUsbConnection(usbDevice, productString, serialNumber));
                var idx = -1;

                Boards.Add(board);
                idx = Boards.IndexOf(board);

                var data = new TreehopperData {SerialNumber = board.SerialNumber};

                var handle = GCHandle.Alloc(data, GCHandleType.Pinned); // pin the object to prevent release

                handles.Add(handle);

                var kr = NativeMethods.IOServiceAddInterestNotification(
                    gNotifyPort, // notifyPort
                    usbDevice.Handle, // service
                    IOKitFramework.kIOGeneralInterest, // interestType
                    DeviceRemoved, // callback
                    idx, // refCon
                    ref notificationPtr // notification
                );

                usbDevice = it.Next();
            }
        }

        public override void Dispose()
        {
            NativeMethods.CFRunLoopStop(gRunLoop);
        }
    }
}