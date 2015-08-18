using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Usb;
using Windows.Foundation;
using Windows.Storage.Streams;

namespace Treehopper
{


    public class TreehopperUsbConnection : ITreehopperConnection
    {

        UsbDevice usbDevice;

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

        UsbBulkInPipe pinEventPipe;
        UsbBulkInPipe peripheralInPipe;
        UsbBulkOutPipe pinConfigPipe;
        UsbBulkOutPipe peripheralOutPipe;

        DataReader pinEventReader;
        DataWriter pinConfigWriter;
        DataReader peripheralReader;
        DataWriter peripheralWriter;

        async void pinEventListerner()
        {
            UInt32 bytesRead = 0;
            while (true)
            {
                bytesRead = await pinEventReader.LoadAsync(64);

                if(bytesRead ==64)
                {
                    byte[] data = new byte[64];
                    pinEventReader.ReadBytes(data);
                    if( this.PinEventDataReceived != null )
                    {
                        PinEventDataReceived(data);
                    }
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

            Task t = new Task(pinEventListerner);
            t.Start();

            Debug.WriteLine("Device opened");
            return true;
        }

        public void Close()
        {
            throw new NotImplementedException();
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
            deviceWatcher = DeviceInformation.CreateWatcher( UsbDevice.GetDeviceSelector( 0x04d8, 0xf426 ));


            // Hook up handlers for the watcher events before starting the watcher

            handlerAdded = new TypedEventHandler<DeviceWatcher, DeviceInformation>(async (watcher, deviceInfo) =>
            {



                UsbDevice usbDevice = await UsbDevice.FromIdAsync(deviceInfo.Id);
                Debug.WriteLine("Device added");
                if (TreehopperUsbConnection.ConnectionAdded != null) {
                    TreehopperUsbConnection.ConnectionAdded(new TreehopperUsbConnection(usbDevice));
                }
              

                });
          

            deviceWatcher.Added += handlerAdded;

            handlerUpdated = new TypedEventHandler<DeviceWatcher, DeviceInformationUpdate>(async (watcher, deviceInfoUpdate) =>
            {
                Debug.WriteLine("Device updated");
            });
            deviceWatcher.Updated += handlerUpdated;

            handlerRemoved = new TypedEventHandler<DeviceWatcher, DeviceInformationUpdate>(async (watcher, deviceInfoUpdate) =>
            {
                Debug.WriteLine("Device removed");
            });
            deviceWatcher.Removed += handlerRemoved;

            handlerEnumCompleted = new TypedEventHandler<DeviceWatcher, Object>(async (watcher, obj) =>
            {
                Debug.WriteLine("Enum completed");
            });
            deviceWatcher.EnumerationCompleted += handlerEnumCompleted;

            handlerStopped = new TypedEventHandler<DeviceWatcher, Object>(async (watcher, obj) =>
            {
                Debug.WriteLine("Device or something stopped");
            });
            deviceWatcher.Stopped += handlerStopped;

            Debug.WriteLine("Starting the wutchah");
            deviceWatcher.Start();
          
        }

    }
}

