% import Treehopper .NET libraries
NET.addAssembly('F:\GitHub\treehopper-sdk\Output\NET\Debug\Treehopper.dll');
NET.addAssembly('F:\GitHub\treehopper-sdk\Output\NET\Debug\Treehopper.Desktop.dll');
NET.addAssembly('F:\GitHub\treehopper-sdk\Output\NET\Debug\Treehopper.Libraries.dll');

import Treehopper.*;

board = Treehopper.Desktop.ConnectionService.Instance.GetFirstDeviceAsync.Result;
board.ConnectAsync().Wait();

for i=1:25
    board.Pins.Item(0).ToggleOutputAsync().Wait();
end

board.Disconnect();