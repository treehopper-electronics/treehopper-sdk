using Charlotte;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper;

namespace Remote.Server
{
    public class RemoteTreehopperServer : MqttModule
    {
        Dictionary<string, TreehopperUsb> Boards { get; set; } = new Dictionary<string, TreehopperUsb>();
        public RemoteTreehopperServer(string hostname, int port = 1883, string username = "", string password = "") : base(hostname, port, username, password)
        {
            On["connection/{board}/state"] = msg =>
            {
                if(msg.Message == "connect")
                    Boards[msg.board].ConnectAsync();
                else
                    Boards[msg.board].Disconnect();
            };

            On["connection/{board}/led"] = msg =>
            {
                Boards[msg.board].Led = bool.Parse(msg.Message);
            };

            wirePins();
            wireI2c();
        }


        void wirePins()
        {
            On["connection/{board}/pins/{pinNumber}/digital"] = msg =>
            {
                string serial = msg.board;
                int pinNumber = int.Parse(msg.pinNumber);

                if (Boards[serial].Pins[pinNumber].Mode == PinMode.DigitalInput) return; // ignore inputs (which we sent!)

                Boards[serial].Pins[pinNumber].DigitalValue = bool.Parse(msg.Message);
            };

            On["connection/{board}/pins/{pinNumber}/mode"] = msg =>
            {
                string serial = msg.board;
                int pinNumber = int.Parse(msg.pinNumber);

                Boards[serial].Pins[pinNumber].Mode = (PinMode)int.Parse(msg.Message);
            };
        }

        void wireI2c()
        {
            On["connection/{board}/i2c/enabled"] = msg =>
            {
                string serial = msg.board;
                bool enabled = bool.Parse(msg.Message);

                Boards[serial].I2c.Enabled = enabled;
            };

            On["connection/{board}/i2c/speed"] = msg =>
            {
                string serial = msg.board;
                double speed = double.Parse(msg.Message);

                Boards[serial].I2c.Speed = speed;
            };

            On["connection/{board}/i2c/send"] = msg =>
            {
                string serial = msg.board;
                var data = Convert.FromBase64String(msg.Message);
                byte address = data[0];
                byte numBytesToRead = data[1];
                byte[] dataToSend = new byte[data.Length - 2];
                Array.Copy(data, 2, dataToSend, 0, dataToSend.Length);

                var response = Boards[serial].I2c.SendReceive(address, dataToSend, numBytesToRead).Result;
                Publish(string.Format("connection/{0}/i2c/received", serial), Convert.ToBase64String(response));
            };
        }

        public void Start()
        {
            Run();

            ConnectionService.Instance.Boards.CollectionChanged += Boards_CollectionChanged;

            foreach (var board in ConnectionService.Instance.Boards)
            {
                AddBoard(board);
            }
        }
        private void Boards_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var item in e.NewItems)
                {
                    var board = (TreehopperUsb)item;
                    AddBoard(board);
                }

            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var item in e.OldItems)
                {
                    var board = (TreehopperUsb)item;
                    RemoveBoard(board);
                }
            }
        }

        private void RemoveBoard(TreehopperUsb board)
        {
            Boards.Remove(board.SerialNumber);
            Publish("removed/"+board.SerialNumber, board.Name, true);
            Console.WriteLine("Removing board: " + board);
        }

        internal void AddBoard(TreehopperUsb board)
        {
            Boards.Add(board.SerialNumber, board);
            Publish("added/"+board.SerialNumber, board.Name, true);
            Console.WriteLine("Adding board: " + board);
        }
    }
}
