namespace Treehopper
{
    /// <summary>
    /// Pin1
    /// </summary>
    public class Pin1 : Pin, IAnalogInPin
    {
        /// <summary>
        /// See <see cref="Treehopper.AnalogIn"/>
        /// </summary>
        public AnalogIn AnalogIn { get; set; }

        internal Pin1(TreehopperBoard device)
            : base(device, 1)
        {
            ioName =  "RA4";
            AnalogIn = new AnalogIn(this);
        }

        internal override void UpdateValue(byte highByte, byte lowByte)
        {
            switch (this.State)
            {
                case PinState.AnalogInput:
                    AnalogIn.UpdateAnalogValue(highByte, lowByte);
                    break;
                case PinState.DigitalInput:
                    base.UpdateValue(highByte, lowByte);
                    break;
            }
        }
    }


    /// <summary>
    /// Pin2
    /// </summary>
    public class Pin2 : Pin, IPwmPin
    {
        /// <summary>
        /// See <see cref="Treehopper.Pwm"/>
        /// </summary>
        public Pwm Pwm { get; set; }

        internal Pin2(TreehopperBoard device)
            : base(device, 2)
        {
            ioName =  "RC5";
            Pwm = new Pwm(this);
        }

        public override PinInterruptMode InterruptMode
        {
            get
            {
                return PinInterruptMode.NoInterrupt;
            }
        }
    }


    /// <summary>
    /// Pin3
    /// </summary>
    public class Pin3 : Pin
    {
        internal Pin3(TreehopperBoard device)
            : base(device, 3)
        {
            ioName =  "RC4";
        }

        public override PinInterruptMode InterruptMode
        {
            get
            {
                return PinInterruptMode.NoInterrupt;
            }
        }
    }


    /// <summary>
    /// Pin4
    /// </summary>
    public class Pin4 : Pin, IAnalogInPin
    {
        /// <summary>
        /// See <see cref="Treehopper.AnalogIn"/>
        /// </summary>
        public AnalogIn AnalogIn { get; set; }

        internal Pin4(TreehopperBoard device)
            : base(device, 4)
        {
            ioName =  "RC3";
            AnalogIn = new AnalogIn(this);
        }

        internal override void UpdateValue(byte highByte, byte lowByte)
        {
            switch (this.State)
            {
                case PinState.AnalogInput:
                    AnalogIn.UpdateAnalogValue(highByte, lowByte);
                    break;
                case PinState.DigitalInput:
                    base.UpdateValue(highByte, lowByte);
                    break;
            }
        }

        public override PinInterruptMode InterruptMode
        {
            get
            {
                return PinInterruptMode.NoInterrupt;
            }
        }
    }

    /// <summary>
    /// Pin5
    /// </summary>
    public class Pin5 : Pin, IPwmPin, IAnalogInPin
    {
        /// <summary>
        /// See <see cref="Treehopper.AnalogIn"/>
        /// </summary>
        public AnalogIn AnalogIn { get; set; }

        /// <summary>
        /// See <see cref="Treehopper.Pwm"/>
        /// </summary>
        public Pwm Pwm { get; set; }

        internal Pin5(TreehopperBoard device)
            : base(device, 5)
        {
            ioName =  "RC6";
            AnalogIn = new AnalogIn(this);
            Pwm = new Pwm(this);
        }

        public override PinInterruptMode InterruptMode
        {
            get
            {
                return PinInterruptMode.NoInterrupt;
            }
        }

        internal override void UpdateValue(byte highByte, byte lowByte)
        {
            switch (this.State)
            {
                case PinState.AnalogInput:
                    AnalogIn.UpdateAnalogValue(highByte, lowByte);
                    break;
                case PinState.DigitalInput:
                    base.UpdateValue(highByte, lowByte);
                    break;
            }
        }
    }


    /// <summary>
    /// Pin6
    /// </summary>
    public class Pin6 : Pin, IAnalogInPin
    {
        /// <summary>
        /// See <see cref="Treehopper.AnalogIn"/>
        /// </summary>
        public AnalogIn AnalogIn { get; set; }

        internal Pin6(TreehopperBoard device) : base(device, 6)
        {
            ioName =  "RC7";
            AnalogIn = new AnalogIn(this);
        }

        internal override void UpdateValue(byte highByte, byte lowByte)
        {
            switch (this.State)
            {
                case PinState.AnalogInput:
                    AnalogIn.UpdateAnalogValue(highByte, lowByte);
                    break;
                case PinState.DigitalInput:
                    base.UpdateValue(highByte, lowByte);
                    break;
            }
        }

        public override PinInterruptMode InterruptMode
        {
            get
            {
                return PinInterruptMode.NoInterrupt;
            }
        }
    }


    /// <summary>
    /// Pin7
    /// </summary>
    public class Pin7 : Pin, IAnalogInPin
    {
        /// <summary>
        /// See <see cref="Treehopper.AnalogIn"/>
        /// </summary>
        public AnalogIn AnalogIn { get; set; }

        internal Pin7(TreehopperBoard device) : base(device, 7)
        {
            ioName =  "RC2";
            AnalogIn = new AnalogIn(this);
        }

        internal override void UpdateValue(byte highByte, byte lowByte)
        {
            switch (this.State)
            {
                case PinState.AnalogInput:
                    AnalogIn.UpdateAnalogValue(highByte, lowByte);
                    break;
                case PinState.DigitalInput:
                    base.UpdateValue(highByte, lowByte);
                    break;
            }
        }

        public override PinInterruptMode InterruptMode
        {
            get
            {
                return PinInterruptMode.NoInterrupt;
            }
        }
    }


    /// <summary>
    /// Pin8
    /// </summary>
    public class Pin8 : Pin
    {
        internal Pin8(TreehopperBoard device)
            : base(device, 8)
        {
            ioName =  "RB7";
        }
    }


    /// <summary>
    /// Pin9
    /// </summary>
    public class Pin9 : Pin, IAnalogInPin
    {
        /// <summary>
        /// See <see cref="Treehopper.AnalogIn"/>
        /// </summary>
        public AnalogIn AnalogIn { get; set; }

        internal Pin9(TreehopperBoard device) : base(device, 9)
        {
            ioName =  "RB5";
            AnalogIn = new AnalogIn(this);
        }

        internal override void UpdateValue(byte highByte, byte lowByte)
        {
            switch (this.State)
            {
                case PinState.AnalogInput:
                    AnalogIn.UpdateAnalogValue(highByte, lowByte);
                    break;
                case PinState.DigitalInput:
                    base.UpdateValue(highByte, lowByte);
                    break;
            }
        }
    }


    /// <summary>
    /// Pin10
    /// </summary>
    public class Pin10 : Pin, IAnalogInPin
    {
        /// <summary>
        /// See <see cref="Treehopper.AnalogIn"/>
        /// </summary>
        public AnalogIn AnalogIn { get; set; }

        internal Pin10(TreehopperBoard device)
            : base(device, 10)
        {
            ioName =  "RB4";
            AnalogIn = new AnalogIn(this);
        }

        internal override void UpdateValue(byte highByte, byte lowByte)
        {
            switch (this.State)
            {
                case PinState.AnalogInput:
                    AnalogIn.UpdateAnalogValue(highByte, lowByte);
                    break;
                case PinState.DigitalInput:
                    base.UpdateValue(highByte, lowByte);
                    break;
            }
        }


    }


    /// <summary>
    /// Pin11
    /// </summary>
    public class Pin11 : Pin
    {
        internal Pin11(TreehopperBoard device)
            : base(device, 11)
        {
            ioName =  "RB6";
        }
    }


    /// <summary>
    /// Pin12
    /// </summary>
    public class Pin12 : Pin, IAnalogInPin
    {
        /// <summary>
        /// See <see cref="Treehopper.AnalogIn"/>
        /// </summary>
        public AnalogIn AnalogIn { get; set; }

        internal Pin12(TreehopperBoard device)
            : base(device, 12)
        {
            ioName = "RC1";
            AnalogIn = new AnalogIn(this);
        }

        internal override void UpdateValue(byte highByte, byte lowByte)
        {
            switch (this.State)
            {
                case PinState.AnalogInput:
                    AnalogIn.UpdateAnalogValue(highByte, lowByte);
                    break;
                case PinState.DigitalInput:
                    base.UpdateValue(highByte, lowByte);
                    break;
            }
        }

        public override PinInterruptMode InterruptMode
        {
            get
            {
                return PinInterruptMode.NoInterrupt;
            }
        }
    }


    /// <summary>
    /// Pin13
    /// </summary>
    public class Pin13 : Pin, IAnalogInPin
    {
        /// <summary>
        /// See <see cref="Treehopper.AnalogIn"/>
        /// </summary>
        public AnalogIn AnalogIn { get; set;  }

        internal Pin13(TreehopperBoard device)
            : base(device, 13)
        {
            ioName =  "RC0";
            AnalogIn = new AnalogIn(this);
        }

        internal override void UpdateValue(byte highByte, byte lowByte)
        {
            switch (this.State)
            {
                case PinState.AnalogInput:
                    AnalogIn.UpdateAnalogValue(highByte, lowByte);
                    break;
                case PinState.DigitalInput:
                    base.UpdateValue(highByte, lowByte);
                    break;
            }
        }

        public new PinInterruptMode InterruptMode
        {
            get
            {
                return PinInterruptMode.NoInterrupt;
            }
        }
    }


    /// <summary>
    /// Pin14
    /// </summary>
    public class Pin14 : Pin
    {
        internal Pin14(TreehopperBoard device)
            : base(device, 14)
        {
            ioName = "RA5";
        }
    }
}
