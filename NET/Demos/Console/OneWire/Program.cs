using Treehopper;

Console.Write("Waiting for board...");
// Get a reference to the first TreehopperUsb board connected. This will await indefinitely until a board is connected.
TreehopperUsb Board = await ConnectionService.Instance.GetFirstDeviceAsync();
Console.WriteLine("Found board: " + Board);

// You must explicitly open a board before communicating with it
await Board.ConnectAsync();

Board.Uart.Mode = UartMode.OneWire;
Board.Uart.Enabled = true;

List<UInt64> addresses = await Board.Uart.OneWireSearchAsync();

Console.WriteLine("Found addresses: ");
foreach (var address in addresses)
    Console.WriteLine(address);

Board.Disconnect();
