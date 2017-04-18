using Remote.Shared;
using System;
using System.Threading.Tasks;
using Treehopper;

namespace Remote.Client
{
    public class RemoteSpi : Spi
    {
        private readonly RemoteTreehopper board;
        readonly TaskCompletionSource<byte[]> dataReceived = new TaskCompletionSource<byte[]>();
        public RemoteSpi(RemoteTreehopper board)
        {
            this.board = board;
            board.RegisterCallback("spi/receive", (msg) =>
            {
                var base64string = msg.Message;
                dataReceived.TrySetResult(Convert.FromBase64String(base64string));
            });
        }
        private bool enabled;
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
                board.Write("spi/enabled", enabled);
            }
        }

        public async Task<byte[]> SendReceive(byte[] dataToWrite, SpiChipSelectPin chipSelect = null, ChipSelectMode chipSelectMode = ChipSelectMode.SpiActiveLow, double speedMhz = 1, SpiBurstMode burstMode = SpiBurstMode.NoBurst, SpiMode spiMode = SpiMode.Mode00)
        {
            var transaction = new SpiTransaction() { DataToWrite = dataToWrite, ChipSelectPinNumber = chipSelect.PinNumber, ChipSelectMode = chipSelectMode, Burst = burstMode, Speed = speedMhz, SpiMode = spiMode };
            board.Write("spi/send", transaction.ToJsonString());
            if (burstMode == SpiBurstMode.BurstTx)
                return new byte[0];

            return await dataReceived.Task;
        }
    }
}
