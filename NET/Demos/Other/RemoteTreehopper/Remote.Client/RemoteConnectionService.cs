using Charlotte;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            On["added/{board}"] = msg =>
            {
                BoardAddedHandler(msg.board, msg.Message);
            };

            On["removed/{board}"] = msg =>
            {
                BoardRemovedHandler(msg.board);
            };

            Run();
        }


        private TaskCompletionSource<RemoteTreehopper> waitForFirstBoard = new TaskCompletionSource<RemoteTreehopper>();
        private string hostname;
        private int port;
        private string username;
        private string password;

        public Task<RemoteTreehopper> GetFirstDeviceAsync()
        {
            return waitForFirstBoard.Task;
        }

        public event BoardAddedEventHandler BoardAdded;
        public event BoardRemovedEventHandler BoardRemoved;

        private void BoardRemovedHandler(string board)
        {
            BoardRemoved?.Invoke(this, new RemoteBoardRemovedEventArgs() { BoardRemoved = new RemoteTreehopper(board, "", hostname, port, username, password) });
        }

        private void BoardAddedHandler(string board, string name)
        {
            BoardAdded?.Invoke(this, new RemoteBoardAddedEventArgs() { BoardAdded = new RemoteTreehopper(board, name, hostname, port, username, password) });
            waitForFirstBoard.TrySetResult(new RemoteTreehopper(board, name, hostname, port, username, password));
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