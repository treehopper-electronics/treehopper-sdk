using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper;

namespace Remote.Server
{
    public class RemoteTreehopperHub : Hub
    {
        public override Task OnConnected()
        {
            Console.WriteLine("User connected: {0}", Context.ConnectionId);
            Console.WriteLine("Using transport: " + Context.QueryString["transport"]);
            foreach(var board in Program.Boards.Keys)
                Clients.Caller.BoardAdded(board);

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            Console.WriteLine("User disconnected: {0}", Context.ConnectionId);
            return base.OnDisconnected(stopCalled);
        }
        public Task ConnectAsync(string board)
        {
            return Program.Boards[board].ConnectAsync();
        }

        public void Disconnect(string board)
        {
            Program.Boards[board].Disconnect();
        }

        public void SetLed(string board, bool value)
        {
            Program.Boards[board].Led = value;
        }
    }
}
