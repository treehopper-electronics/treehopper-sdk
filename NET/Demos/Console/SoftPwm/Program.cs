using Treehopper;

Console.Write("Looking for board...");
var board = await ConnectionService.Instance.GetFirstDeviceAsync();
Console.WriteLine("Board found.");
await board.ConnectAsync();
var pin = board[19];

pin.Mode = PinMode.SoftPwm;
pin.DutyCycle = 0.8;
int step = 10;
int rate = 25;
while (true)
{
    for (int i = 0; i < 256; i = i + step)
    {
        pin.DutyCycle = i / 255.0;
        await Task.Delay(rate);
    }
    for (int i = 255; i > 0; i = i - step)
    {
        pin.DutyCycle = i / 255.0;
        await Task.Delay(rate);
    }
}