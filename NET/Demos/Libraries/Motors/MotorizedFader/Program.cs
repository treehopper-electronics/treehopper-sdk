using System.Diagnostics;
using Treehopper.Utilities;
using Treehopper.Libraries.Motors;
using Treehopper;

var board = await ConnectionService.Instance.GetFirstDeviceAsync();
await board.ConnectAsync();
var hbridge = new DualHalfBridge(board.Pins[0], board.Pins[1], board.Pwm1);
var fader = new MotorizedFader(board.Pins[2], hbridge);
fader.K = 20;
fader.Enabled = true;
LogPositions(fader).Forget();
while (!Console.KeyAvailable)
{
    for (int i = 0; i <= 10; i++)
    {
        fader.GoalPosition = i / 10.0;
        await Task.Delay(500);
        if (i == 0) await Task.Delay(100); // wait extra time to allow full travel
    }
}
board.Disconnect();

async Task LogPositions(MotorizedFader fader)
{
    while (true)
    {
        Debug.WriteLine(fader.ActualPosition);
        await Task.Delay(100);
    }
}