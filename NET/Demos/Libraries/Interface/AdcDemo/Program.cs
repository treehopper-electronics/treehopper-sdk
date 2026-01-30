using Treehopper;
using Treehopper.Libraries.IO.Adc;

var board = await ConnectionService.Instance.GetFirstDeviceAsync();
await board.ConnectAsync();

var adc = new Ads1115(board.I2c, Ads1115.ChannelMode.SingleEnded);
adc.AutoUpdateWhenPropertyRead = false;

while (!Console.KeyAvailable)
{
    await adc.UpdateAsync();
    Console.WriteLine("Data:");
    for (int i = 0; i < adc.Pins.Count; i++)
        Console.WriteLine($"{adc.Pins[i].AnalogVoltage:0.00}");

    await Task.Delay(1000);
}