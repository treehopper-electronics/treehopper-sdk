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

        public async static Task<List<DS18B20>> FindAll(TreehopperUsb board)
        {
            SensorList.Clear();
            AddressList.Clear();
            board.Uart.Mode = UartMode.OneWire;
            board.Uart.Enabled = true;
            List<UInt64> addresses = await board.Uart.OneWireSearch();
            foreach(var address in addresses)
            {
                if((address & 0xff) == 0x28)
                {
                    AddressList.Add(address);
                    SensorList.Add(new DS18B20(board, address));
                }
            }
            return SensorList;
        }

        public async static Task<Dictionary<UInt64, double>> GetAllTemperatures(TreehopperUsb board)
        {
            Dictionary<UInt64, double> returnTemperatures = new Dictionary<ulong, double>();

            await board.Uart.OneWireReset();
            await board.Uart.Send(new byte[] { 0xCC, 0x44 });
            await Task.Delay(750);

            foreach (UInt64 address in AddressList)
            {
                await board.Uart.OneWireResetAndMatchAddress(address);
                await board.Uart.Send(0xBE);

                byte[] data = await board.Uart.Receive(2);

                await board.Uart.OneWireReset();
                double temp = ((Int16)(data[0] | (data[1] << 8))) / 16.0;
                returnTemperatures.Add(address, temp);
            }

            return returnTemperatures;
        }

        public static double CelsiusToFahrenheit(double celsius)
        {
            return celsius * 1.8 + 32.0;
        }

        TreehopperUsb board;
        ulong address;

        public DS18B20(TreehopperUsb board, ulong address)
        {
            this.board = board;
            this.address = address;
            this.board.Uart.Mode = UartMode.OneWire;
            this.board.Uart.Enabled = true;
        }

        private async Task<double> GetTemperature()
        {
            await board.Uart.OneWireResetAndMatchAddress(address);
            await board.Uart.Send(0x44);
            await Task.Delay(750);

            await board.Uart.OneWireResetAndMatchAddress(address);
            await board.Uart.Send(0xBE);

            byte[] data = await board.Uart.Receive(2);

            await board.Uart.OneWireReset();
            double temp = ((Int16)(data[0] | (data[1] << 8))) / 16.0;
            return temp;
        }
    }
}
