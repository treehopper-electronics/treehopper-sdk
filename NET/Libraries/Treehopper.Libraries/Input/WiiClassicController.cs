using System.Collections;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Treehopper.Libraries.Interface.PortExpander;
using Treehopper.Libraries.Sensors;

namespace Treehopper.Libraries.Input
{
    /// <summary>
    ///     Nintendo Wii Classic Controller
    /// </summary>
    public class WiiClassicController : IPollable
    {
        private readonly SMBusDevice dev;
        private DPadState dPad;
        private Vector2 leftStick;
        private float leftTriggerForce;
        private Vector2 rightStick;
        private float rightTriggerForce;

        public WiiClassicController(I2c i2c)
        {
            dev = new SMBusDevice(0x52, i2c);
            dev.WriteData(new byte[] {0xF0, 0x55}).Wait();
            dev.WriteData(new byte[] {0xFB, 0x00}).Wait();

            L = new Button(new DigitalInPeripheralPin(this), false);
            R = new Button(new DigitalInPeripheralPin(this), false);
            ZL = new Button(new DigitalInPeripheralPin(this), false);
            ZR = new Button(new DigitalInPeripheralPin(this), false);
            Home = new Button(new DigitalInPeripheralPin(this), false);
            Plus = new Button(new DigitalInPeripheralPin(this), false);
            Minus = new Button(new DigitalInPeripheralPin(this), false);
            A = new Button(new DigitalInPeripheralPin(this), false);
            B = new Button(new DigitalInPeripheralPin(this), false);
            X = new Button(new DigitalInPeripheralPin(this), false);
            Y = new Button(new DigitalInPeripheralPin(this), false);
        }

        public Button R { get; }
        public Button L { get; }
        public Button ZL { get; }
        public Button ZR { get; }
        public Button Home { get; }
        public Button Plus { get; }
        public Button Minus { get; }
        public Button A { get; }
        public Button B { get; }
        public Button X { get; }
        public Button Y { get; }


        public DPadState DPad
        {
            get
            {
                if (AutoUpdateWhenPropertyRead) Update().Wait();
                return dPad;
            }
        }

        public Vector2 LeftStick
        {
            get
            {
                if (AutoUpdateWhenPropertyRead) Update().Wait();
                return leftStick;
            }
        }


        public Vector2 RightStick
        {
            get
            {
                if (AutoUpdateWhenPropertyRead) Update().Wait();
                return rightStick;
            }
        }

        public double LeftTriggerForce
        {
            get
            {
                if (AutoUpdateWhenPropertyRead) Update().Wait();
                return leftTriggerForce;
            }
        }

        public double RightTriggerForce
        {
            get
            {
                if (AutoUpdateWhenPropertyRead) Update().Wait();
                return rightTriggerForce;
            }
        }

        public bool AutoUpdateWhenPropertyRead { get; set; } = true;

        public int AwaitPollingInterval { get; set; } = 20;

        public async Task Update()
        {
            await dev.WriteByte(0x00).ConfigureAwait(false);
            var response = await dev.ReadData(6).ConfigureAwait(false);
            var lx = (response[0] & 0x3F) - 32;
            var ly = (response[1] & 0x3F) - 32;
            var rx = (response[2] >> 7) | ((response[1] & 0xC0) >> 5) | (((response[0] & 0xC0) >> 3) - 16);
            var ry = (response[2] & 0x1F) - 16;

            leftStick.X = lx > 0 ? lx / 31f : lx / 32f;
            leftStick.Y = ly > 0 ? ly / 31f : ly / 32f;

            rightStick.X = rx > 0 ? rx / 15f : rx / 16f;
            rightStick.Y = ry > 0 ? ry / 15f : ry / 16f;

            var lt = ((response[2] & 0x60) >> 2) | (response[3] >> 5);
            var rt = response[3] & 0x1F;

            leftTriggerForce = lt / 31f;
            rightTriggerForce = rt / 31f;

            var array = new BitArray(response.Skip(4).Take(2).ToArray());
            ((DigitalInPeripheralPin) R.Input).DigitalValue = !array[1];
            ((DigitalInPeripheralPin) Plus.Input).DigitalValue = !array[2];
            ((DigitalInPeripheralPin) Home.Input).DigitalValue = !array[3];
            ((DigitalInPeripheralPin) Minus.Input).DigitalValue = !array[4];
            ((DigitalInPeripheralPin) L.Input).DigitalValue = !array[5];

            // D-Pad stuff
            var temp = dPad;
            if (!array[6])
                dPad = DPadState.Down;
            else if (!array[7])
                dPad = DPadState.Right;
            else if (!array[8])
                dPad = DPadState.Up;
            else if (!array[9])
                dPad = DPadState.Left;
            else
                dPad = DPadState.None;

            if (temp != dPad)
                DPadStateChanged?.Invoke(this, new DPadStateEventArgs {NewValue = dPad});

            ((DigitalInPeripheralPin) ZR.Input).DigitalValue = !array[10];
            ((DigitalInPeripheralPin) X.Input).DigitalValue = !array[11];
            ((DigitalInPeripheralPin) A.Input).DigitalValue = !array[12];
            ((DigitalInPeripheralPin) Y.Input).DigitalValue = !array[13];
            ((DigitalInPeripheralPin) B.Input).DigitalValue = !array[14];
            ((DigitalInPeripheralPin) ZL.Input).DigitalValue = !array[15];
        }

        public event DPadStateEventHandler DPadStateChanged;
    }
}