namespace Treehopper
{
    /// <summary>
    /// Pin1
    /// </summary>
    public class Pin1 : Pin
    {
        internal Pin1(TreehopperUSB device)
            : base(device, 1)
        {
            ioName =  "P0.1 (MISO)";
        }
    }


    /// <summary>
    /// Pin2
    /// </summary>
    public class Pin2 : Pin
    {
        internal Pin2(TreehopperUSB device)
            : base(device, 2)
        {
            ioName =  "P0.0 (SCK)";
        }
    }


    /// <summary>
    /// Pin3
    /// </summary>
    public class Pin3 : Pin
    {
        internal Pin3(TreehopperUSB device)
            : base(device, 3)
        {
            ioName =  "P0.2 (MOSI)";
        }

    }


    /// <summary>
    /// Pin4
    /// </summary>
    public class Pin4 : Pin
    {
        internal Pin4(TreehopperUSB device)
            : base(device, 4)
        {
            ioName =  "P0.3 (SDA)";
        }
    }

    /// <summary>
    /// Pin5
    /// </summary>
    public class Pin5 : Pin
    {
        internal Pin5(TreehopperUSB device)
            : base(device, 5)
        {
            ioName =  "P0.6 (SCL)";
        }
    }


    /// <summary>
    /// Pin6
    /// </summary>
    public class Pin6 : Pin
    {
        internal Pin6(TreehopperUSB device) : base(device, 6)
        {
            ioName =  "P0.4 (TX)";
        }
    }

    /// <summary>
    /// Pin7
    /// </summary>
    public class Pin7 : Pin
    {
        internal Pin7(TreehopperUSB device) : base(device, 7)
        {
            ioName =  "P0.5 (RX)";
        }

    }


    /// <summary>
    /// Pin8
    /// </summary>
    public class Pin8 : Pin
    {
        internal Pin8(TreehopperUSB device)
            : base(device, 8)
        {
            ioName =  "P0.7 (PWM1)";
        }
    }

    /// <summary>
    /// Pin9
    /// </summary>
    public class Pin9 : Pin
    {
        internal Pin9(TreehopperUSB device)
            : base(device, 9)
        {
            ioName = "P1.0 (PWM2)";
        }
    }


    /// <summary>
    /// Pin10
    /// </summary>
    public class Pin10 : Pin
    {
        internal Pin10(TreehopperUSB device)
            : base(device, 10)
        {
            ioName = "P1.1 (PWM3)";
        }
    }

    /// <summary>
    /// Pin11
    /// </summary>
    public class Pin11 : Pin
    {
        internal Pin11(TreehopperUSB device)
            : base(device, 11)
        {
            ioName = "P1.2";
        }
    }

    /// <summary>
    /// Pin11
    /// </summary>
    public class Pin12 : Pin
    {
        internal Pin12(TreehopperUSB device)
            : base(device, 12)
        {
            ioName = "P1.3";
        }
    }

    /// <summary>
    /// Pin11
    /// </summary>
    public class Pin13 : Pin
    {
        internal Pin13(TreehopperUSB device)
            : base(device, 13)
        {
            ioName = "P1.4";
        }
    }

    /// <summary>
    /// Pin11
    /// </summary>
    public class Pin14 : Pin
    {
        internal Pin14(TreehopperUSB device)
            : base(device, 14)
        {
            ioName = "P1.5";
        }
    }

    /// <summary>
    /// Pin11
    /// </summary>
    public class Pin15 : Pin
    {
        internal Pin15(TreehopperUSB device)
            : base(device, 15)
        {
            ioName = "P1.6";
        }
    }

    /// <summary>
    /// Pin11
    /// </summary>
    public class Pin16 : Pin
    {
        internal Pin16(TreehopperUSB device)
            : base(device, 16)
        {
            ioName = "P1.7";
        }
    }

    /// <summary>
    /// Pin11
    /// </summary>
    public class Pin17 : Pin
    {
        internal Pin17(TreehopperUSB device)
            : base(device, 17)
        {
            ioName = "P2.0";
        }
    }

    /// <summary>
    /// Pin11
    /// </summary>
    public class Pin18 : Pin
    {
        internal Pin18(TreehopperUSB device)
            : base(device, 18)
        {
            ioName = "P2.1";
        }
    }

    /// <summary>
    /// Pin11
    /// </summary>
    public class Pin19 : Pin
    {
        internal Pin19(TreehopperUSB device)
            : base(device, 19)
        {
            ioName = "P2.2";
        }
    }

    /// <summary>
    /// Pin11
    /// </summary>
    public class Pin20 : Pin
    {
        internal Pin20(TreehopperUSB device)
            : base(device, 20)
        {
            ioName = "P2.3";
        }
    }

}
