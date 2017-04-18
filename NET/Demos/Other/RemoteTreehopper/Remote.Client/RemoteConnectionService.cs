using Charlotte;
using System;
using System.Threading.Tasks;
using Remote.Shared;

namespace Remote.Client
{
    public delegate void BoardAddedEventHandler(object sender, RemoteBoardAddedEventArgs e);
    public delegate void BoardRemovedEventHandler(object sender, RemoteBoardRemovedEventArgs e);
    public class RemoteConnectionService : MqttModule
    {
        private string uri;

        public RemoteConnectionService(string hostname, int port = 1883, string username = "", string password = "") : base(hostname, port, username, password)
        {
            this.hostname = hostname;
            this.port = port;
            this.username = username;
            this.password = password;

            On["treehopper/board/added"] = msg =>
            {
                BoardAddedHandler(new RemoteBoardInfo(msg.Message));
            };

            On["treehopper/board/removed"] = msg =>
            {
                BoardRemovedHandler(new RemoteBoardInfo(msg.Message));
            };

            Run();

            Publish("treehopper/board/scan", "");
        }


        private readonly TaskCompletionSource<RemoteTreehopper> waitForFirstBoard = new TaskCompletionSource<RemoteTreehopper>();
        private readonly string hostname;
        private readonly int port;
        private readonly string username;
        private readonly string password;

        public Task<RemoteTreehopper> GetFirstDeviceAsync()
        {
            return waitForFirstBoard.Task;
        }

        public event BoardAddedEventHandler BoardAdded;
        public event BoardRemovedEventHandler BoardRemoved;

        private void BoardRemovedHandler(RemoteBoardInfo board)
        {
            BoardRemoved?.Invoke(this, new RemoteBoardRemovedEventArgs() { BoardRemoved = new RemoteTreehopper(board, hostname, port, username, password) });
        }

        private void BoardAddedHandler(RemoteBoardInfo board)
        {
            BoardAdded?.Invoke(this, new RemoteBoardAddedEventArgs() { BoardAdded = new RemoteTreehopper(board, hostname, port, username, password) });
            waitForFirstBoard.TrySetResult(new RemoteTreehopper(board, hostname, port, username, password));
        }
    }


    public class RemoteBoardAddedEventArgs : EventArgs
    {
        public RemoteTreehopper BoardAdded { get; set; }
    }

    public class RemoteBoardRemovedEventArgs : EventArgs
    {
        public RemoteTreehopper BoardRemoved { get; set; }
    }
}