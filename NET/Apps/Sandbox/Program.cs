using Treehopper;

Console.Write("Waiting for board...");
// Get a reference to the first TreehopperUsb board connected. This will await indefinitely until a board is connected.
var board = await ConnectionService.Instance.GetFirstDeviceAsync();
Console.WriteLine("Found board: " + board);
Console.WriteLine("Version: " + board.VersionString);

// You must explicitly connect to a board before communicating with it
await board.ConnectAsync();

Console.WriteLine("Start blinking. Press any key to stop.");
while (board.IsConnected && !Console.KeyAvailable)
{
    // toggle the LED.
    board.Led = !board.Led;
    await Task.Delay(100);
}

board.Disconnect();