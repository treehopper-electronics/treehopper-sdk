using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper;

namespace Remote.Client
{
    public delegate void BoardAddedEventHandler(object sender, RemoteBoardAddedEventArgs e);
    public delegate void BoardRemovedEventHandler(object sender, RemoteBoardRemovedEventArgs e);
    public class RemoteConnectionService
    {
        private string uri;
        private HubConnection hubConnection;
        private IHubProxy hub;

        public RemoteConnectionService(string uri)
        {
            this.uri = uri;
            hubConnection = new HubConnection(uri);
            //hubConnection.Error += HubConnection_Error;
            //hubConnection.TraceLevel = TraceLevels.All;
            //hubConnection.TraceWriter = new DebugTextWriter();
            hub = hubConnection.CreateHubProxy("RemoteTreehopperHub");
            hub.On<string>("BoardAdded", BoardAddedHandler);
            hub.On<string>("BoardRemoved", BoardRemovedHandler);
        }

        private void HubConnection_Error(Exception obj)
        {
            Debug.WriteLine(obj.Message);
        }

        public void Start()
        {
            if (hubConnection.State == ConnectionState.Connected) return;

            hubConnection.Start();
        }

        private TaskCompletionSource<RemoteTreehopper> waitForFirstBoard = new TaskCompletionSource<RemoteTreehopper>();
        public Task<RemoteTreehopper> GetFirstDeviceAsync()
        {
            Start();
            return waitForFirstBoard.Task;
        }

        public event BoardAddedEventHandler BoardAdded;
        public event BoardRemovedEventHandler BoardRemoved;

        private void BoardRemovedHandler(string board)
        {
            BoardRemoved?.Invoke(this, new RemoteBoardRemovedEventArgs() { BoardRemoved = new RemoteTreehopper(board, hub) });
        }

        private void BoardAddedHandler(string board)
        {
            BoardAdded?.Invoke(this, new RemoteBoardAddedEventArgs() { BoardAdded = new RemoteTreehopper(board, hub) });
            waitForFirstBoard.TrySetResult(new RemoteTreehopper(board, hub));
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
