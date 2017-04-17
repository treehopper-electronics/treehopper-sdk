% import Treehopper .NET libraries
NET.addAssembly('F:\GitHub\treehopper-sdk\Output\NET\Debug\Treehopper.dll');
NET.addAssembly('F:\GitHub\treehopper-sdk\Output\NET\Debug\Treehopper.Desktop.dll');
NET.addAssembly('F:\GitHub\treehopper-sdk\Output\NET\Debug\Treehopper.Libraries.dll');

import Treehopper.*;

board = Treehopper.Desktop.ConnectionService.Instance.GetFirstDeviceAsync.Result;
board.ConnectAsync().Wait();

for i=1:25
    board.Led = ~board.Led;
    pause(0.1);
end

board.Disconnect();