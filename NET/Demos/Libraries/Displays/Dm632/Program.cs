using Treehopper;
using Treehopper.Libraries.Displays;
using System.Threading;

var board = await ConnectionService.Instance.GetFirstDeviceAsync();
await board.ConnectAsync();

var driver = new Dm632(board.Spi, board.Pins[5], 6);
await driver.Clear();

driver.Leds[0].Brightness = 1;

var fader1 = new LedFadeAnimation(true);
var fader2 = new LedFadeAnimation(true);
fader1.Duration = 500;
fader2.Duration = 500;
while (!Console.KeyAvailable)
{
    for (int i = 0; i < 16; i++)
    {
        fader2.Led = fader1.Led;
        fader1.Led = driver.Leds[i];

        if (fader2.Led != null)
        {
            await fader2.RunAsync(1, 0);
        }
            
        await fader1.RunAsync(0, 1);
    }
}


board.Disconnect();