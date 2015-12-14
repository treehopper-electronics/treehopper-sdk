using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Usb;
using Windows.Foundation;
using Windows.Storage.Streams;

namespace Treehopper
{

    /// <summary>
    /// Tasking is done following the example here: https://msdn.microsoft.com/en-us/library/dd997396(v=vs.110).aspx
    /// </summary>
    public class TreehopperUsbConnection : ITreehopperConnection
    {


        UsbDevice usbDevice;

        UsbBulkInPipe pinEventPipe;
        UsbBulkInPipe peripheralInPipe;
        UsbBulkOutPipe pinConfigPipe;
        UsbBulkOutPipe peripheralOutPipe;

        DataReader pinEventReader;
        DataWriter pinConfigWriter;
        DataReader peripheralReader;
        DataWriter peripheralWriter;

        Task pinEventListernerTask;
        CancellationTokenSource pinEventListernerTaskCancellationTokenSource;
        Task peripheralListenerTask;
        CancellationTokenSource peripheralListenerTaskCancellationTokenSource;


        public TreehopperUsbConnection(UsbDevice usbDevice)
        {
            this.usbDevice = usbDevice;
        }

        public event PinEventData PinEventDataReceived;



        public string SerialNumber
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public string DeviceName
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }


        /// <summary>
        /// Task function constantly polling the device's output (PCs input lol) buffer
        /// waiting for the incoming 64-bytes of USB data 
        /// </summary>
        async void pinEventListerner()
        {
            CancellationToken Token = pinEventListernerTaskCancellationTokenSource.Token;
            UInt32 bytesRead = 0;
            Token.ThrowIfCancellationRequested();
            while (true)
            {

                if(Token.IsCancellationRequested)
                {
                    Token.ThrowIfCancellationRequested();
                }

                try {
                    bytesRead = await pinEventReader.LoadAsync(64);
                    if (bytesRead == 64)
                    {
                        byte[] data = new byte[64];
                        pinEventReader.ReadBytes(data);
                        if (this.PinEventDataReceived != null)
                        {
                            PinEventDataReceived(data);
                        }
                    }
                }catch(Exception e)
                {
                    Debug.WriteLine("Unable to read pin event due to exception: " + e.Message);
                    break;
                }
            }
        }

        async void peripheralListener()
        {
            UInt32 bytesRead = 0;
            CancellationToken Token = peripheralListenerTaskCancellationTokenSource.Token;
            Token.ThrowIfCancellationRequested();

            while (true)
            {

                if(Token.IsCancellationRequested)
                {
                    Token.ThrowIfCancellationRequested();
                }

                try {
                    bytesRead = await peripheralReader.LoadAsync(64);
                    if (bytesRead == 64)
                    {
                        byte[] data = new byte[64];
                        pinEventReader.ReadBytes(data);
                        if (this.PinEventDataReceived != null)
                        {
                            Debug.WriteLine("Peripheral data received!");
                            // Call the function when data received on peripheral
                        }
                    }
                }catch(Exception e)
                {
                    Debug.WriteLine("Unable to read the peripheral due to exception: " + e.Message);
                    break;
                }
            }
        }

        public bool Open()
        {
            // https://msdn.microsoft.com/en-us/library/windows/hardware/dn303346(v=vs.85).aspx


            pinConfigPipe = usbDevice.DefaultInterface.BulkOutPipes[0];

            pinConfigPipe.WriteOptions |= UsbWriteOptions.ShortPacketTerminate;
            peripheralOutPipe = usbDevice.DefaultInterface.BulkOutPipes[1];
            peripheralOutPipe.WriteOptions |= UsbWriteOptions.ShortPacketTerminate;

            pinConfigWriter = new DataWriter(pinConfigPipe.OutputStream);
            peripheralWriter = new DataWriter(peripheralOutPipe.OutputStream);


            pinEventPipe = usbDevice.DefaultInterface.BulkInPipes[0];
            pinEventReader = new DataReader(pinEventPipe.InputStream);
            peripheralInPipe = usbDevice.DefaultInterface.BulkInPipes[1];
            peripheralReader = new DataReader(peripheralInPipe.InputStream);

            pinEventListernerTaskCancellationTokenSource = new CancellationTokenSource();
            peripheralListenerTaskCancellationTokenSource = new CancellationTokenSource();

            pinEventListernerTask = new Task(pinEventListerner, pinEventListernerTaskCancellationTokenSource.Token);
            peripheralListenerTask = new Task(peripheralListener, peripheralListenerTaskCancellationTokenSource.Token);

            pinEventListernerTask.Start();
            peripheralListenerTask.Start();


            Debug.WriteLine("Device opened");
            return true;
        }

        /// <summary>
        /// Proper cleanup is necessary here
        /// </summary>
        public void Close()
        {
            
            /*
                        UsbDevice usbDevice;

                        UsbBulkInPipe pinEventPipe;
                        UsbBulkInPipe peripheralInPipe;
                        UsbBulkOutPipe pinConfigPipe;
                        UsbBulkOutPipe peripheralOutPipe;

                        DataReader pinEventReader;
                        DataWriter pinConfigWriter;
                        DataReader peripheralReader;
                        DataWriter peripheralWriter;

                        Task pinEventListernerTask;
                        Task peripheralListenerTask;
                        */

            // Cancel all the tasks
            pinEventListernerTaskCancellationTokenSource.Cancel();
            peripheralListenerTaskCancellationTokenSource.Cancel();

            // Wait for the tasks to complete
            pinEventListernerTask.Wait();
            peripheralListenerTask.Wait();

//            pinEventReader.DetachStream();
 //           peripheralReader.DetachStream();
  //          pinConfigWriter.DetachStream();
   //         peripheralWriter.DetachStream();
        }

        async public void SendDataPinConfigChannel(byte[] data)
        {
            try {
                pinConfigWriter.WriteBytes(data);
                await pinConfigWriter.StoreAsync();
            }catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        async public void SendDataPeripheralChannel(byte[] data)
        {
            try
            {
                peripheralWriter.WriteBytes(data);
                await peripheralWriter.StoreAsync();
            } catch(Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }


        static Dictionary<String, TreehopperUsbConnection> UsbConnections = new Dictionary<string, TreehopperUsbConnection>();

        static uint VendorId = 0x04d8;
        static uint ProductId = 0xf426;
        public static List<TreehopperUsbConnection> ConnectedDevices { get; set; }
        public static event ConnectionAddedHandler ConnectionAdded;
        public static event ConnectionRemovedHandler ConnectionRemoved;
        public static void AddBoard() { }
        public static TreehopperUsbConnection ConnectedDevice { get; set; }
        static DeviceWatcher deviceWatcher;
        static TypedEventHandler<DeviceWatcher, DeviceInformation> handlerAdded = null;
        static TypedEventHandler<DeviceWatcher, DeviceInformationUpdate> handlerUpdated = null;
        static TypedEventHandler<DeviceWatcher, DeviceInformationUpdate> handlerRemoved = null;
        static TypedEventHandler<DeviceWatcher, Object> handlerEnumCompleted = null;
        static TypedEventHandler<DeviceWatcher, Object> handlerStopped = null;

    

        public static void StartConnectionManager() {
            StartWatcher();
        }



        static void StartWatcher()
        {
            // Hook up handlers for the watcher events before starting the watcher
            handlerAdded = new TypedEventHandler<DeviceWatcher, DeviceInformation>(async (watcher, deviceInfo) =>
            {
                String deviceInfoId = deviceInfo.Id;
                UsbDevice usbDevice = await UsbDevice.FromIdAsync(deviceInfoId);
                if (usbDevice == null)
                {
                    Debug.WriteLine("Returned device = null! " + deviceInfoId);
                }
                else
                {
                    Debug.WriteLine("Device added");
                    if (TreehopperUsbConnection.ConnectionAdded != null)
                    {
                        TreehopperUsbConnection newTreehopperUsbConnection = new TreehopperUsbConnection(usbDevice);
                        UsbConnections.Add(deviceInfoId, newTreehopperUsbConnection);
                        TreehopperUsbConnection.ConnectionAdded(newTreehopperUsbConnection);
                    }
                }
                
        });


            handlerUpdated = new TypedEventHandler<DeviceWatcher, DeviceInformationUpdate>(async (watcher, deviceInfoUpdate) =>
            {
                Debug.WriteLine("Device updated");
            });


            handlerRemoved = new TypedEventHandler<DeviceWatcher, DeviceInformationUpdate>(async (watcher, deviceInfoUpdate) =>
            {
                String deviceInfoId = deviceInfoUpdate.Id;
                Debug.WriteLine("Device removed, id: " + deviceInfoId);

                TreehopperUsbConnection removedTreehopperUsbConnection = UsbConnections[deviceInfoId];

                TreehopperUsbConnection.ConnectionRemoved(removedTreehopperUsbConnection);
                UsbConnections.Remove(deviceInfoId);
            });
            

            handlerEnumCompleted = new TypedEventHandler<DeviceWatcher, Object>(async (watcher, obj) =>
            {
                Debug.WriteLine("Enum completed");
            });
           
            handlerStopped = new TypedEventHandler<DeviceWatcher, Object>(async (watcher, obj) =>
            {
                Debug.WriteLine("Device or something stopped");
            });


            deviceWatcher = DeviceInformation.CreateWatcher(UsbDevice.GetDeviceSelector(VendorId, ProductId));
            deviceWatcher.Added += handlerAdded;
            deviceWatcher.Updated += handlerUpdated;
            deviceWatcher.Removed += handlerRemoved;
            deviceWatcher.EnumerationCompleted += handlerEnumCompleted;
            deviceWatcher.Stopped += handlerStopped;
            deviceWatcher.Start();
          
        }

    }
}

