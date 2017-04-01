using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper;

namespace Remote.Client
{
    public class RemotePin : DigitalIOPin, SpiChipSelectPin
    {
        internal RemotePin(RemoteTreehopper board, int pinNumber)
        {
            this.Board = board;
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

                Board.Write(string.Format("pins/{0}/mode", PinNumber), (int)mode);
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

                Board.Write(string.Format("pins/{0}/digital", PinNumber), digitalValue);
            }
        }
        public RemoteTreehopper Board { get; private set; }
        public int PinNumber { get; private set; }

        public Spi SpiModule
        {
            get
            {
                return Board.Spi;
            }
        }

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
