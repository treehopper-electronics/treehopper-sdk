﻿namespace Treehopper.Libraries.Input
{
    using System.Collections;
    using System.Threading.Tasks;
    using Interface.PortExpander;

    /// <summary>
    /// Nintendo Super NES (SNES) Controller
    /// </summary>
    public class SuperNesController : NesController
    {
        public Button X { get; private set; }
        public Button Y { get; private set; }
        public Button L { get; private set; }
        public Button R { get; private set; }
        public SuperNesController(Spi spi, SpiChipSelectPin ps) : base(spi, ps)
        {
            X = new Button(new DigitalInPeripheralPin(this));
            Y = new Button(new DigitalInPeripheralPin(this));
            L = new Button(new DigitalInPeripheralPin(this));
            R = new Button(new DigitalInPeripheralPin(this));
        }

        public override async Task Update()
        {
            var result = await dev.SendReceive(new byte[] { 0x00, 0x00 });
            BitArray values = new BitArray(result);
            ((DigitalInPeripheralPin)A.Input).DigitalValue = values[15];
            ((DigitalInPeripheralPin)B.Input).DigitalValue = values[7];
            ((DigitalInPeripheralPin)Select.Input).DigitalValue = values[5];
            ((DigitalInPeripheralPin)Start.Input).DigitalValue = values[4];

            ((DigitalInPeripheralPin)X.Input).DigitalValue = values[14];
            ((DigitalInPeripheralPin)Y.Input).DigitalValue = values[6];
            ((DigitalInPeripheralPin)L.Input).DigitalValue = values[13];
            ((DigitalInPeripheralPin)R.Input).DigitalValue = values[12];

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
    }
}
