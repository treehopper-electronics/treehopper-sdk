using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Motors.Amis30624
{
    internal enum Command
    {
        GetFullStatus1 = 0x81,
        GetFullStatus2 = 0xfc,
        GetOtpParam = 0x82,
        GotoSecurePosition = 0x84,
        HardStop = 0x85,
        ResetPosition = 0x86,
        ResetToDefault = 0x87,
        SetDualPosition = 0x88,
        SetMotorParam = 0x89,
        SetOtp = 0x90,
        SetPosition = 0x8B,
        SetStallParam = 0x96,
        SoftStop = 0x8f,
        RunVelocity = 0x97,
        TestBemf = 0x9f
    }
}
