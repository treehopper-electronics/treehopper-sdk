using System;
using System.Threading.Tasks;
using Treehopper;

namespace Remote.Client
{
    public class RemotePin : DigitalIO, SpiChipSelectPin
    {
        internal RemotePin(RemoteTreehopper board, int pinNumber)
        {
            Board = board;
            PinNumber = pinNumber;
        }

        private PinMode mode;
        public PinMode Mode
        {
            get { return mode; }
            set
            {
                if (mode == value) return;
                mode = value;

                Board.Write($"pins/{PinNumber}/mode", (int)mode);
            }
        }

        private bool digitalValue;

        public event OnDigitalInValueChanged DigitalValueChanged;

        public bool DigitalValue
        {
            get { return digitalValue; }
            set {
                if (digitalValue == value) return;

                if (mode != PinMode.PushPullOutput && mode != PinMode.OpenDrainOutput)
                    Mode = PinMode.PushPullOutput;

                digitalValue = value;

                Board.Write($"pins/{PinNumber}/digital", digitalValue);
            }
        }
        public RemoteTreehopper Board { get; private set; }
        public int PinNumber { get; private set; }

        public Spi SpiModule => Board.Spi;

        public Task<bool> AwaitDigitalValueChange()
        {
            throw new NotImplementedException();
        }

        public async Task MakeDigitalIn()
        {
            Mode = PinMode.DigitalInput;
        }

        public async Task ToggleOutputAsync()
        {
            DigitalValue = !DigitalValue;
        }

        public async Task MakeDigitalPushPullOut()
        {
            Mode = PinMode.PushPullOutput;
        }
    }
}
