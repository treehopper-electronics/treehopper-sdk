using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Usb;
using Windows.Foundation;
using Windows.Storage.Streams;

namespace Treehopper
{
    public class FirmwareConnection : IFirmwareConnection
    {
        private DeviceWatcher deviceWatcher;
        private TaskCompletionSource<bool> deviceAddedTCS = new TaskCompletionSource<bool>();
        private TaskCompletionSource<bool> readerTCS = new TaskCompletionSource<bool>();
        private UsbDevice usbDevice;
        private UsbInterruptOutPipe outPipe;
        private UsbInterruptInPipe inPipe;
        private DataWriter writer;

        public FirmwareConnection()
        {
            deviceWatcher = DeviceInformation.CreateWatcher(UsbDevice.GetDeviceSelector(TreehopperUsb.Settings.BootloaderVid, TreehopperUsb.Settings.BootloaderPid));
            deviceWatcher.Added += DeviceWatcher_Added;
            deviceWatcher.Updated += DeviceWatcher_Updated;
            deviceWatcher.Removed += DeviceWatcher_Removed;
            deviceWatcher.EnumerationCompleted += DeviceWatcher_EnumerationCompleted;
        }

        private void DeviceWatcher_EnumerationCompleted(DeviceWatcher sender, object args)
        {
            readerTCS.TrySetResult(false);
        }

        private void DeviceWatcher_Removed(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            throw new NotImplementedException();
        }

        private void DeviceWatcher_Updated(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            throw new NotImplementedException();
        }

        private async void DeviceWatcher_Added(DeviceWatcher sender, DeviceInformation args)
        {
            usbDevice = await UsbDevice.FromIdAsync(args.Id);

            outPipe = usbDevice.DefaultInterface.InterruptOutPipes[0];

            inPipe = usbDevice.DefaultInterface.InterruptInPipes[0];

            writer = new DataWriter(outPipe.OutputStream);
            inPipe.DataReceived += InPipe_DataReceived;

            deviceAddedTCS.TrySetResult(true);
        }

        byte[] bytes;

        private void InPipe_DataReceived(UsbInterruptInPipe sender, UsbInterruptInEventArgs args)
        {
            bytes = args.InterruptData.ToArray();
            readerTCS.TrySetResult(true);
        }

        public void Close()
        {
            usbDevice.Dispose();
        }

        public Task<bool> OpenAsync()
        {
            deviceWatcher.Start();
            return deviceAddedTCS.Task;
        }

        public async Task<byte[]> Read(int numBytes)
        {
            if (bytes != null)
                return bytes;

            await readerTCS.Task;
            var retVal = new byte[bytes.Length];
            bytes.CopyTo(retVal, 0);

            bytes = null;

            return retVal;
        }

        public async Task<bool> Write(byte[] data)
        {
            writer.WriteBytes(data);
            var result = await writer.StoreAsync();
            return result > 0;
        }
    }
}
