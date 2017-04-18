using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Sensors.Temperature
{
    /// <summary>
    /// Maxim DS18B20 One-Wire temperature sensor
    /// </summary>
    /// <remarks>
    /// The DS18B20 uses the IOneWire interface found on Treehopper.Uart for communication. You can use the <see cref="IOneWire.OneWireSearch()"/> method to discover the device address to pass to the <see cref="Ds18b20.Ds18b20(IOneWire, ulong)"/> constructor; setting this parameter to "0" will switch to all-call addressing, allowing the class to work with a single sensor attached to the bus without having to know its address. Don't use the all-call mode if there are multiple devices on the <see cref="IOneWire"/> bus, as this will cause interference.
    /// 
    /// If you're using multiple devices, consider using the <see cref="Ds18b20.Group"/> class, which efficiently manages collections of sensors.
    /// 
    /// For proper electrical operation, tie Tx and Rx pins together, attach to the Dq pin of the DS18B20, and pull-up with a 4.7 k resistor to 3.3V. We recommend supplying 5V power if possible -- especially if multiple sensors are attached to the bus.
    /// </remarks>
    public class Ds18b20 : TemperatureSensor
    {
        readonly IOneWire oneWire;

        /// <summary>
        /// Construct a DS18B20
        /// </summary>
        /// <param name="oneWire">The OneWire bus the sensor is attached to</param>
        /// <param name="address">The address of the DS18B20, or 0 to use the all-call mode</param>
        /// <remarks>
        /// In most applications, there is a single DS18B20 attached to the OneWire bus, and there are no other peripherals on the bus. In this case, there's no need to worry about addressing, and you can simply read from the device directly:
        /// \code
        /// var sensor = new Ds18b20(board.Uart);
        /// while(!Console.KeyAvailable)
        /// {
        ///    Console.WriteLine(sensor.Fahrenheit);
        /// }
        /// \endcode
        /// 
        /// Note that this property read will take more than 750 ms to complete, as the conversion time of this sensor is long. If using this in a GUI-based project, you should fetch the update asynchronously to avoid blocking the GUI thread:
        /// \code
        /// sensor.AutoUpdateWhenPropertyRead = false // set this once to disable updates when invoking property reads
        /// ...
        /// await sensor.Update(); // request an update -- will take ~750 ms.
        /// this.SensorValue = sensor.Celsius; // copy the updated sensor data to our data-binding property
        /// RaisePropertyChanged("SensorValue"); // inform the UWP/WPF GUI that we have new sensor data
        /// \endcode
        /// </remarks>

        public Ds18b20(IOneWire oneWire, ulong address = 0)
        {
            this.oneWire = oneWire;
            Address = address;
            oneWire.StartOneWire();
        }

        public ulong Address { get; private set; }

        /// <summary>
        /// Whether group conversion mode is enabled or not
        /// </summary>
        /// <remarks>
        /// When false (default), this temperature sensor operates independently. When true, <see cref="Update"/> will not initiate a conversion; just read the result. This means you must call <see cref="Group.StartConversionAsync"/> to request a temperature conversion when this property is true.
        /// </remarks>
        public bool EnableGroupConversion { get; set; }

        public override async Task Update()
        {
            if(!EnableGroupConversion)
            {
                if(Address == 0)
                {
                    await oneWire.OneWireReset();
                    await oneWire.Send(new byte[] { 0xCC, 0x44 });
                }
                else
                {
                    await oneWire.OneWireResetAndMatchAddress(Address);
                    await oneWire.Send(0x44);
                }
                
                await Task.Delay(750);
            }

            if (Address == 0)
            {
                await oneWire.OneWireReset();
                await oneWire.Send(new byte[] { 0xCC, 0xBE });
            }
            else
            {
                await oneWire.OneWireResetAndMatchAddress(Address);
                await oneWire.Send(0xBE);
            }

            byte[] data = await oneWire.Receive(2);

            Celsius = ((short)(data[0] | (data[1] << 8))) / 16d;
        }

        /// <summary>
        /// Manages a group of Ds18b20 sensors
        /// </summary>
        /// <remarks>
        /// This class can be used to manage one or more DS18B20 sensors efficiently. The class does this by issuing a group "start conversion" command for every <see cref="ConversionCycle"/>. If you plan to always use the <see cref="StartConversionAsync" /> method, you can simply call it whenever you want to collect a reading from one or more sensors:
        /// \code
        /// var tempGroup = new Ds18b20.Group(board.Uart);
        /// var sensors = await tempGroup.FindAll();
        /// while(true)
        /// {
        ///     await tempGroup.StartConversionAsync());
        ///     foreach (var sensor in sensors)
        ///     {
        ///         Console.WriteLine(sensor.Fahrenheit);
        ///     }
        ///     await Task.Delay(10000); // wait 10 seconds for the next conversion
        /// }
        /// \endcode
        /// 
        /// If you want to work with temperature sensors both inside and outside a collective conversion, make use of the <see cref="IDisposable"/> interface on <see cref="ConversionCycle"/>:
        /// 
        /// \code
        /// var tempGroup = new Ds18b20.Group(board.Uart);
        /// var sensors = await tempGroup.FindAll();
        /// 
        /// processData(sensors[0].Fahrenheit);
        /// await Task.Delay(5000);
        /// 
        /// // perform a group conversion
        /// using (await tempGroup.StartConversionAsync())
        /// {
        ///    foreach (var sensor in sensors)
        ///    {
        ///        Console.WriteLine(sensor.Fahrenheit);
        ///    }
        /// } // the ConversionCycle is disposed here, allowing the sensors to be addressed individually
        /// 
        /// // ...like this:
        /// processData(sensors[1].Fahrenheit);
        /// \endcode
        /// </remarks>
        public class Group
        {
            private readonly IOneWire oneWire;
            List<Ds18b20> SensorList = new List<Ds18b20>();

            /// <summary>
            /// Construct a Group of Ds18b20 sensors
            /// </summary>
            /// <param name="oneWire">The OneWire interface (UART) this group is attached to</param>
            public Group(IOneWire oneWire)
            {
                this.oneWire = oneWire;
            }

            /// <summary>
            /// Findl all sensors on the bus
            /// </summary>
            /// <returns>A list of Ds18b20 sensors</returns>
            public async Task<IList<Ds18b20>> FindAll()
            {
                SensorList = new List<Ds18b20>();
                oneWire.StartOneWire();
                List<UInt64> addresses = await oneWire.OneWireSearch();
                foreach (var address in addresses)
                {
                    if ((address & 0xff) == 0x28)
                    {
                        SensorList.Add(new Ds18b20(oneWire, address));
                    }
                }
                return SensorList;
            }

            /// <summary>
            /// Start a conversion on all DS18B20s attached to the bus
            /// </summary>
            /// <returns>An awaitable task that completes upon success</returns>
            /// <remarks>
            /// This method will await for 750 milliseconds, which is the minimum time required to complete a conversion
            /// </remarks>
            public async Task<ConversionCycle> StartConversionAsync()
            {
                foreach(var sensor in SensorList)
                {
                    sensor.EnableGroupConversion = true;
                }

                await oneWire.OneWireReset();
                await oneWire.Send(new byte[] { 0xCC, 0x44 });
                await Task.Delay(750);
                return new ConversionCycle(this);
            }

            private void endConversion()
            {
                foreach (var sensor in SensorList)
                {
                    sensor.EnableGroupConversion = false;
                }
            }

            /// <summary>
            /// Disposable object that can be used to manage the <see cref="Ds18b20.EnableGroupConversion"/> flag automatically.
            /// </summary>
            public struct ConversionCycle : IDisposable
            {
                private readonly Group ds18b20Group;

                internal ConversionCycle(Group group)
                {
                    ds18b20Group = group;
                }

                /// <summary>
                /// Call this method (usually implicitly at the end of a "using" block) to free up the sensor for individual use.
                /// </summary>
                public void Dispose()
                {
                    ds18b20Group.endConversion();
                }
            }
        }
    }
}
