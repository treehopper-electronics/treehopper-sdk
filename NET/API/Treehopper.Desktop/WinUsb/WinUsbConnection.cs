using Microsoft.Win32.SafeHandles;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Treehopper.Desktop.WinUsb
{
    public class WinUsbConnection : IConnection
    {
        private string name;
        private string serial;
        private SafeFileHandle deviceHandle;
        private SafeWinUsbHandle winUsbHandle;
        private Task pinListenerTask;
        private bool _disposed = false;
        const byte pinReportEndpoint = 0x81;
        const byte peripheralResponseEndpoint = 0x82;
        const byte pinConfigEndpoint = 0x01;
        const byte peripheralConfigEndpoint = 0x02;

        public WinUsbConnection(string path, string name, string serial, short version)
        {
            DevicePath = path;
            Name = name;
            Serial = serial;
            Version = version;
        }

        public bool UseOverlappedTransfers { get; set; } = true;

        public bool IsOpen { get; private set; } = false;

        public string DevicePath { get; private set; }

        public string Name { get; private set; }

        public string Serial { get; private set; }

        public int UpdateRate { get; set; }

        public short Version { get; private set; }

        public event PinEventData PinEventDataReceived;
        public event PropertyChangedEventHandler PropertyChanged;

        public void Close()
        {
            if (!IsOpen)
                return;

            if (winUsbHandle != null && !winUsbHandle.IsInvalid)
            {
                WinUsb.NativeMethods.WinUsb_AbortPipe(winUsbHandle, peripheralConfigEndpoint);
                WinUsb.NativeMethods.WinUsb_AbortPipe(winUsbHandle, peripheralResponseEndpoint);
                WinUsb.NativeMethods.WinUsb_AbortPipe(winUsbHandle, pinConfigEndpoint);
                WinUsb.NativeMethods.WinUsb_AbortPipe(winUsbHandle, pinReportEndpoint);
                winUsbHandle.Dispose();
            }

            if (deviceHandle != null && !deviceHandle.IsInvalid)
            {
                deviceHandle.Dispose();
                deviceHandle = null;
            }

            IsOpen = false;
        }

        ~WinUsbConnection()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                // Cleanup managed resources
                Close();
            }

            _disposed = true;
        }

        public async Task<bool> OpenAsync()
        {
            deviceHandle =
                Kernel32.CreateFile(DevicePath,
                        NativeFileAccess.FILE_GENERIC_WRITE | NativeFileAccess.FILE_GENERIC_READ,
                        NativeFileShare.FILE_SHARE_WRITE | NativeFileShare.FILE_SHARE_READ,
                        IntPtr.Zero,
                        NativeFileMode.OPEN_EXISTING,
                        NativeFileFlag.FILE_ATTRIBUTE_NORMAL | NativeFileFlag.FILE_FLAG_OVERLAPPED,
                        IntPtr.Zero);

            if (deviceHandle.IsInvalid || deviceHandle.IsClosed)
            {
                return false;
            }
            if (UseOverlappedTransfers)
                ThreadPool.BindHandle(deviceHandle); // needed for overlapped I/O

            winUsbHandle = new SafeWinUsbHandle();
            if ( WinUsb.NativeMethods.WinUsb_Initialize(deviceHandle, ref winUsbHandle) == false)
            {
                return false;
            }

            IsOpen = true;
            BeginRead(pinReportEndpoint, pinReportBuffer, pinReportBuffer.Length, pinStateCallback, null); // kick off our first pin read
            return true;
        }

        readonly byte[] pinReportBuffer = new byte[41];
        private void pinStateCallback(IAsyncResult result)
        {
            EndRead(result);
            PinEventDataReceived?.Invoke(pinReportBuffer);
            if (!IsOpen) return;
            try
            {
                BeginRead(pinReportEndpoint, pinReportBuffer, pinReportBuffer.Length, pinStateCallback, null); // start another read
            } catch(Exception ex)
            {
                Debug.WriteLine("Exception: " + ex.Message);
            }
        }

        public async Task<byte[]> ReadPeripheralResponsePacket(uint numBytesToRead)
        {
            byte[] array = new byte[numBytesToRead];
            if (!IsOpen) return new byte[0];
            if (UseOverlappedTransfers)
            {
                await Task.Factory.FromAsync((callback, stateObject) => BeginRead(peripheralResponseEndpoint, array, array.Length, callback, stateObject), EndRead, null).ConfigureAwait(false);

            } else
            {
                int bytesWritten = 0;
                WinUsb.NativeMethods.WinUsb_ReadPipe(winUsbHandle, peripheralResponseEndpoint, array, array.Length,
                                out bytesWritten, IntPtr.Zero);
            }
            return array;
        }

        public async Task<byte[]> ReadPinReportPacket(uint numBytesToRead)
        {
            if (!IsOpen) return new byte[0];
            byte[] array = new byte[numBytesToRead];
            if (UseOverlappedTransfers)
            {
                await Task.Factory.FromAsync((callback, stateObject) => BeginRead(pinReportEndpoint, array, array.Length, callback, stateObject), EndRead, null).ConfigureAwait(false);
            }
            else
            {
                int bytesWritten = 0;
                WinUsb.NativeMethods.WinUsb_ReadPipe(winUsbHandle, pinReportEndpoint, array, array.Length,
                                out bytesWritten, IntPtr.Zero);
            }
            return array;
        }

        public Task SendDataPeripheralChannel(byte[] data)
        {
            if (!IsOpen) return Task.FromResult(new object());
            if (UseOverlappedTransfers)
            {
                return Task.Factory.FromAsync((callback, stateObject) => BeginWrite(peripheralConfigEndpoint, data, data.Length, callback, stateObject), EndWrite, null);
            }
            else
            {
                return Task.Run(() =>
                {
                    int bytesWritten = 0;
                    WinUsb.NativeMethods.WinUsb_WritePipe(winUsbHandle, peripheralResponseEndpoint, data, data.Length,
                out bytesWritten, IntPtr.Zero);
                });
            }
        }

        public Task SendDataPinConfigChannel(byte[] data)
        {
            if (!IsOpen) return Task.FromResult(new object());
            if (UseOverlappedTransfers)
            {
                return Task.Factory.FromAsync((callback, stateObject) => BeginWrite(pinConfigEndpoint, data, data.Length, callback, stateObject), EndWrite, null);
            }
            else
            {
                return Task.Run(() =>
                {
                    int bytesWritten = 0;
                    WinUsb.NativeMethods.WinUsb_WritePipe(winUsbHandle, pinConfigEndpoint, data, data.Length,
                out bytesWritten, IntPtr.Zero);
                });
            }

        }

        private IAsyncResult BeginWrite(byte endpoint, byte[] buffer, int length, AsyncCallback userCallback, object stateObject)
        {
            USBAsyncResult result = new USBAsyncResult(userCallback, stateObject);
            try
            {
                Overlapped overlapped = new Overlapped();
                overlapped.AsyncResult = result;

                unsafe
                {
                    NativeOverlapped* pOverlapped = null;

                    int bytesWritten;
                    pOverlapped = overlapped.Pack(PipeIOCallback, buffer);

                    bool success;
                    // Buffer is pinned already by overlapped.Pack
                    fixed (byte* pBuffer = buffer)
                    {
                        success = WinUsb.NativeMethods.WinUsb_WritePipe(winUsbHandle, endpoint, buffer, length,
                                out bytesWritten, (IntPtr)pOverlapped);
                    }
                    HandleOverlappedAPI(success, "Failed to asynchronously write pipe on WinUSB device.", pOverlapped, result, (int)bytesWritten);
                }
            }
            catch (Exception e)
            {
                if (result != null)
                    result.Dispose();
                throw new Exception("Failed to write to pipe.", e);
            }

            return result;
        }

        public void EndWrite(IAsyncResult asyncResult)
        {
            if (asyncResult == null)
                throw new NullReferenceException("asyncResult cannot be null");
            if (!(asyncResult is USBAsyncResult))
                throw new ArgumentException("AsyncResult object was not created by calling BeginWrite on this class.");

            USBAsyncResult result = (USBAsyncResult)asyncResult;
            try
            {
                // todo: check duplicate end writes?

                if (!result.IsCompleted)
                    result.AsyncWaitHandle.WaitOne();

                if (result.Error != null)
                    throw new Exception("Asynchronous write to pipe has failed.", result.Error);
            }
            finally
            {
                result.Dispose();
            }
        }

        private IAsyncResult BeginRead(byte endpoint, byte[] buffer, int length, AsyncCallback userCallback, object stateObject)
        {
            USBAsyncResult result = new USBAsyncResult(userCallback, stateObject);
            try
            {
                Overlapped overlapped = new Overlapped();

                overlapped.AsyncResult = result;

                unsafe
                {
                    NativeOverlapped* pOverlapped = null;
                    int bytesRead;

                    pOverlapped = overlapped.Pack(PipeIOCallback, buffer);
                    bool success;
                    // Buffer is pinned already by overlapped.Pack

                        success = WinUsb.NativeMethods.WinUsb_ReadPipe(winUsbHandle, endpoint, buffer, length,
                                    out bytesRead, (IntPtr)pOverlapped);
                    HandleOverlappedAPI(success, "Failed to asynchronously read pipe on WinUSB device.", pOverlapped, result, (int)bytesRead);
                }
            }
            catch (Exception e)
            {
                if (result != null)
                    result.Dispose();
                throw new Exception("Failed to read from pipe.", e);
            }
            return result;
        }

        private int EndRead(IAsyncResult asyncResult)
        {
            if (asyncResult == null)
                throw new NullReferenceException("asyncResult cannot be null");
            if (!(asyncResult is USBAsyncResult))
                throw new ArgumentException("AsyncResult object was not created by calling BeginRead on this class.");

            // todo: check duplicate end reads?
            USBAsyncResult result = (USBAsyncResult)asyncResult;
            try
            {
                if (!result.IsCompleted)
                    result.AsyncWaitHandle.WaitOne();

                if (result.Error != null)
                    throw new Exception("Asynchronous read from pipe has failed.", result.Error);

                return result.BytesTransfered;
            }
            finally
            {
                result.Dispose();
            }
        }

        private unsafe void PipeIOCallback(uint errorCode, uint numBytes, NativeOverlapped* pOverlapped)
        {
            try
            {
                Exception error = null;
                if (errorCode != 0)
                {

                }
                Overlapped overlapped = Overlapped.Unpack(pOverlapped);
                USBAsyncResult result = (USBAsyncResult)overlapped.AsyncResult;
                Overlapped.Free(pOverlapped);
                pOverlapped = null;

                result.OnCompletion(false, error, (int)numBytes, true);
            }
            finally
            {
                if (pOverlapped != null)
                {
                    Overlapped.Unpack(pOverlapped);
                    Overlapped.Free(pOverlapped);
                }
            }
        }

        private unsafe void HandleOverlappedAPI(bool success, string errorMessage, NativeOverlapped* pOverlapped, USBAsyncResult result, int bytesTransfered)
        {
            if (!success)
            {
                if (Marshal.GetLastWin32Error() != Kernel32.ERROR_IO_PENDING)
                {
                    Overlapped.Unpack(pOverlapped);
                    Overlapped.Free(pOverlapped);
                    throw new Exception(errorMessage);
                }
            }
            else
            {
                // Immediate success!
                Overlapped.Unpack(pOverlapped);
                Overlapped.Free(pOverlapped);

                result.OnCompletion(true, null, bytesTransfered, false);
                // is the callback still called in this case?? todo
            }

        }

    }
}
