using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper;

namespace Remote.Client
{
    public class RemoteI2c : I2c
    {
        private readonly RemoteTreehopper board;

        TaskCompletionSource<byte[]> dataReceived = new TaskCompletionSource<byte[]>();

        internal RemoteI2c(RemoteTreehopper board)
        {
            this.board = board;
            board.RegisterCallback("i2c/received", msg =>
            {
                var base64string = msg.Message;
                dataReceived.TrySetResult(Convert.FromBase64String(base64string));
            });
        }

        public bool enabled;
        public bool Enabled
        {
            get
            {
                return enabled;
            }

            set
            {
                if (enabled == value) return;
                enabled = value;

                board.Write("i2c/enabled", enabled);
            }
        }

        private double speed;
        public double Speed
        {
            get
            {
                return speed;
            }

            set
            {
                if (Math.Abs(speed - value) < 1) return; // don't update if we're close
                speed = value;

                board.Write("i2c/speed", speed);
            }
        }

        public Task<byte[]> SendReceive(byte address, byte[] dataToWrite, byte numBytesToRead)
        {
            dataReceived = new TaskCompletionSource<byte[]>();
            var data = new byte[] { address, numBytesToRead }.Concat(dataToWrite).ToArray();
            board.Write("i2c/send", Convert.ToBase64String(data));
            return dataReceived.Task;
        }
    }
}
