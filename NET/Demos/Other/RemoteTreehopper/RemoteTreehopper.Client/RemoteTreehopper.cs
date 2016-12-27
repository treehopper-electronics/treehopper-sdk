using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper;

namespace Remote
{
    public class RemoteTreehopper
    {
        public string board;
        public IHubProxy hub;
        private HubConnection hubConnection;
        public RemoteTreehopper(string boardSerialNumber, IHubProxy hub)
        {
            this.board = boardSerialNumber;
            this.hub = hub;
        }

        public Task ConnectAsync()
        {
            return hub.Invoke("ConnectAsync", board);
        }

        public void Disconnect()
        {
            hub.Invoke("Disconnect", board);
        }

        private bool led = false;
        public bool Led
        {
            get { return led; }
            set
            {
                if (led == value) return;
                led = value;
                hub.Invoke("SetLed", board, led);
            }
        }
    }
}
