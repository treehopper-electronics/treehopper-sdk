using Treehopper;
using Treehopper.Libraries.Displays;

var board = await ConnectionService.Instance.GetFirstDeviceAsync();
await board.ConnectAsync();
var display = new Ssd1306(board.I2c);

Console.WriteLine("Press any key to clear display and exit...");

while (!Console.KeyAvailable)
{
    await display.Write(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString());
    await Task.Delay(250);
}

await display.Clear();