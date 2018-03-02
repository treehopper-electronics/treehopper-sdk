using System.Collections;
using System.ComponentModel;
using System.Threading.Tasks;
using Treehopper.Libraries.IO.PortExpander;
using Treehopper.Libraries.Sensors;

namespace Treehopper.Libraries.Input
{
    /// <summary>
    ///     Nintendo Entertainment System (NES) Controller
    /// </summary>
    [Supports("Nintendo", "NES Controller")]
    public class NesController : IPollable, IPolledEvents
    {
        protected SpiDevice dev;

        protected DPadState dpad;

        public NesController(Spi spi, SpiChipSelectPin ps)
        {
            dev = new SpiDevice(spi, ps, ChipSelectMode.PulseHighAtBeginning);

            A = new Button(new DigitalInPeripheralPin(this));
            B = new Button(new DigitalInPeripheralPin(this));
            Start = new Button(new DigitalInPeripheralPin(this));
            Select = new Button(new DigitalInPeripheralPin(this));
        }

        public Button A { get; }
        public Button B { get; }

        public Button Start { get; }
        public Button Select { get; }

        public DPadState DPad
        {
            get
            {
                if (AutoUpdateWhenPropertyRead) UpdateAsync().Wait();
                return dpad;
            }
        }

        public bool AutoUpdateWhenPropertyRead { get; set; } = true;

        public int AwaitPollingInterval { get; set; } = 25;

        /// <summary>
        /// Requests a reading from the controller and updates its data properties with the gathered values.
        /// </summary>
        /// <returns>An awaitable Task</returns>
        /// <remarks>
        /// Note that when #AutoUpdateWhenPropertyRead is `true` (which it is, by default), this method is implicitly 
        /// called when any sensor data property is read from --- there's no need to call this method unless you set
        /// AutoUpdateWhenPropertyRead to `false`.
        /// 
        /// Unless otherwise noted, this method updates all sensor data simultaneously, which can often lead to more efficient
        /// bus usage (as well as reducing USB chattiness).
        /// </remarks>
        public virtual async Task UpdateAsync()
        {
            var result = await dev.SendReceive(new byte[] {0x00});
            var values = new BitArray(result);
            ((DigitalInPeripheralPin) A.Input).DigitalValue = values[7];
            ((DigitalInPeripheralPin) B.Input).DigitalValue = values[6];
            ((DigitalInPeripheralPin) Select.Input).DigitalValue = values[5];
            ((DigitalInPeripheralPin) Start.Input).DigitalValue = values[4];

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
            {
                RaiseDPadChanged();
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DPad)));
        }

        public event DPadStateEventHandler DPadStateChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaiseDPadChanged()
        {
            DPadStateChanged?.Invoke(this, new DPadStateEventArgs {NewValue = dpad});
        }
    }
}