namespace AdcDemo
{
    using System;
    using System.Threading.Tasks;
    using Treehopper;
    using Treehopper.Desktop;
    using Treehopper.Libraries.Interface.Adc;
    class Program
    {
        static void Main(string[] args)
        {
            App();
        }

        static async Task App()
        {
            var board = await ConnectionService.Instance.GetFirstDeviceAsync();
            await board.ConnectAsync();

            var adc = new Ads1115(board.I2c, Ads1115.ChannelMode.SingleEnded);
            adc.AutoUpdateWhenPropertyRead = false;

            while(!Console.KeyAvailable)
            {
                await adc.Update();
                Console.WriteLine("Data:");
                for(int i=0;i<adc.Pins.Count;i++)
                    Console.WriteLine($"{adc.Pins[i].AnalogVoltage:0.00}");

                await Task.Delay(1000);
            }
        }
    }
}
