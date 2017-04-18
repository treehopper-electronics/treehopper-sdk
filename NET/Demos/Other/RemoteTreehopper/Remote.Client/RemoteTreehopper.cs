using Charlotte;
using Remote.Shared;
using System;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Remote.Client
{
    public class RemoteTreehopper : MqttModule
    {
        public string Name { get; private set; }
        public string SerialNumber { get; private set; }

        public Collection<RemotePin> Pins { get; set; } = new Collection<RemotePin>();

        public RemoteTreehopper(RemoteBoardInfo board, string hostname, int port, string username, string password) : base(hostname, port, username, password)
        {
            SerialNumber = board.Serial;
            Name = board.Name;
            for (int i = 0; i < 20; i++)
                Pins.Add(new RemotePin(this, i));
            I2c = new RemoteI2c(this);
            Spi = new RemoteSpi(this);

        }
        public void Open()
        {
            this.Run();
            this.Publish($"treehopper/connection/{SerialNumber}/state", "connect");
        }

        public void Close()
        {
            this.Publish($"treehopper/connection/{SerialNumber}/state", "disconnect");
        }

        private bool led = false;
        public bool Led
        {
            get { return led; }
            set
            {
                if (led == value) return;
                led = value;
                this.Publish($"treehopper/connection/{SerialNumber}/led", led.ToString());
            }
        }

        public RemoteI2c I2c { get; private set; }

        public RemoteSpi Spi { get; private set; }

        internal void RegisterCallback(string pattern, Action<dynamic> action)
        {
            pattern = $"treehopper/connection/{SerialNumber}/" + pattern;
            On[pattern] = action;
        }

        internal void Write(string topicFragment, dynamic message)
        {
            this.Publish($"treehopper/connection/{SerialNumber}/" + topicFragment, message.ToString());
        }

        public override string ToString()
        {
            return $"{Name} ({SerialNumber})";
        }
    }
}