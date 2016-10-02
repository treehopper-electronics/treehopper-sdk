using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Amis30624
{
    public enum HoldingCurrent
    {
        mA_59,
        mA_71,
        mA_84,
        mA_100,
        mA_119,
        mA_141,
        mA_168,
        mA_200,
        mA_238,
        mA_336,
        mA_400,
        mA_476,
        mA_566,
        mA_673,
        mA_0
    }

    public static partial class EnumExtensions
    {
        public static int ToInt(this HoldingCurrent current)
        {
            switch (current)
            {
                case HoldingCurrent.mA_100:
                    return 100;
                case HoldingCurrent.mA_119:
                    return 119;
                case HoldingCurrent.mA_141:
                    return 141;
                case HoldingCurrent.mA_168:
                    return 168;
                case HoldingCurrent.mA_200:
                    return 200;
                case HoldingCurrent.mA_238:
                    return 238;
                case HoldingCurrent.mA_336:
                    return 336;
                case HoldingCurrent.mA_400:
                    return 400;
                case HoldingCurrent.mA_476:
                    return 476;
                case HoldingCurrent.mA_566:
                    return 566;
                case HoldingCurrent.mA_59:
                    return 59;
                case HoldingCurrent.mA_673:
                    return 673;
                case HoldingCurrent.mA_71:
                    return 71;
                case HoldingCurrent.mA_0:
                    return 0;
                case HoldingCurrent.mA_84:
                    return 84;
                default:
                    return 0;
            }
        }
    }
}
