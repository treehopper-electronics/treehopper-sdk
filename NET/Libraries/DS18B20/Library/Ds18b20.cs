using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries
{
    public class DS18B20
    {
        public static List<DS18B20> SensorList = new List<DS18B20>();

        public static List<ulong> AddressList = new List<ulong>();

        public async static Task<List<DS18B20>> FindAll(IOneWire oneWire)
        {
            SensorList.Clear();
            AddressList.Clear();
            oneWire.StartOneWire();
            List<UInt64> addresses = await oneWire.OneWireSearch();
            foreach(var address in addresses)
            {
                if((address & 0xff) == 0x28)
                {
                    AddressList.Add(address);
                    SensorList.Add(new DS18B20(oneWire, address));
                }
            }
            return SensorList;
        }

        public async static Task<Dictionary<UInt64, double>> GetAllTemperatures(IOneWire oneWire)
        {
            Dictionary<UInt64, double> returnTemperatures = new Dictionary<ulong, double>();

            oneWire.StartOneWire();
            await oneWire.OneWireReset();
            await oneWire.Send(new byte[] { 0xCC, 0x44 });
            await Task.Delay(750);

            foreach (UInt64 address in AddressList)
            {
                await oneWire.OneWireResetAndMatchAddress(address);
                await oneWire.Send(0xBE);

                byte[] data = await oneWire.Receive(2);

                await oneWire.OneWireReset();
                double temp = ((Int16)(data[0] | (data[1] << 8))) / 16.0;
                returnTemperatures.Add(address, temp);
            }

            return returnTemperatures;
        }

        public static double CelsiusToFahrenheit(double celsius)
        {
            return celsius * 1.8 + 32.0;
        }

        IOneWire oneWire;
        ulong address;

        public DS18B20(IOneWire oneWire, ulong address)
        {
            this.oneWire = oneWire;
            this.address = address;
            oneWire.StartOneWire();
        }

        private async Task<double> GetTemperature()
        {
            await oneWire.OneWireResetAndMatchAddress(address);
            await oneWire.Send(0x44);
            await Task.Delay(750);

            await oneWire.OneWireResetAndMatchAddress(address);
            await oneWire.Send(0xBE);

            byte[] data = await oneWire.Receive(2);

            await oneWire.OneWireReset();
            double temp = ((Int16)(data[0] | (data[1] << 8))) / 16.0;
            return temp;
        }
    }
}
