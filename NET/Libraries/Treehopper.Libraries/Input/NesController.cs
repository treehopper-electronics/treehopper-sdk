namespace Treehopper.Libraries.Input
{
    using System.Collections;
    using System.Threading.Tasks;
    using Treehopper.Libraries.Interface.PortExpander;
    using Sensors;

    /// <summary>
    /// Nintendo Entertainment System (NES) Controller
    /// </summary>
    public class NesController : IPollable
    {
        protected SpiDevice dev;

        public Button A { get; private set; }
        public Button B { get; private set; }

        public Button Start { get; private set; }
        public Button Select { get; private set; }

        public event DPadStateEventHandler DPadStateChanged;

        protected DPadState dpad;
        public DPadState DPad
        {
            get
            {
                if (AutoUpdateWhenPropertyRead) Update().Wait();
                return dpad;
            }
        }

        public NesController(Spi spi, SpiChipSelectPin ps)
        {
            this.dev = new SpiDevice(spi, ps, ChipSelectMode.PulseHighAtBeginning);

            A = new Button(new DigitalInPeripheralPin(this));
            B = new Button(new DigitalInPeripheralPin(this));
            Start = new Button(new DigitalInPeripheralPin(this));
            Select = new Button(new DigitalInPeripheralPin(this));
        }

        public bool AutoUpdateWhenPropertyRead { get; set; } = true;

        public int AwaitPollingInterval { get; set; } = 20;

        public virtual async Task Update()
        {
            var result = await dev.SendReceive(new byte[] { 0x00 });
            BitArray values = new BitArray(result);
            ((DigitalInPeripheralPin)A.Input).DigitalValue = values[7];
            ((DigitalInPeripheralPin)B.Input).DigitalValue = values[6];
            ((DigitalInPeripheralPin)Select.Input).DigitalValue = values[5];
            ((DigitalInPeripheralPin)Start.Input).DigitalValue = values[4];

            var oldState = dpad;

            if (!values[3])
                dpad = DPadState.Up;
            else if (!values[2])
                dpad = DPadState.Down;
            else if (!values[1])
                dpad = DPadState.Left;
            else if (!values[0])
                dpad = DPadState.Right;
            else
                dpad = DPadState.None;

            if (oldState != dpad)
                RaiseDPadChanged();
        }

        protected void RaiseDPadChanged()
        {
            DPadStateChanged?.Invoke(this, new DPadStateEventArgs() { NewValue = dpad });
        }
    }
}
