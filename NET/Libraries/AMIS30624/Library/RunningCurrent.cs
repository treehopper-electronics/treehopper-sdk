using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Amis30624
{
    public enum RunningCurrent
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
        mA_800
    };

    public static partial class EnumExtensions
    {
        public static int ToInt(this RunningCurrent current)
        {
            switch (current)
            {
                case RunningCurrent.mA_100:
                    return 100;
                case RunningCurrent.mA_119:
                    return 119;
                case RunningCurrent.mA_141:
                    return 141;
                case RunningCurrent.mA_168:
                    return 168;
                case RunningCurrent.mA_200:
                    return 200;
                case RunningCurrent.mA_238:
                    return 238;
                case RunningCurrent.mA_336:
                    return 336;
                case RunningCurrent.mA_400:
                    return 400;
                case RunningCurrent.mA_476:
                    return 476;
                case RunningCurrent.mA_566:
                    return 566;
                case RunningCurrent.mA_59:
                    return 59;
                case RunningCurrent.mA_673:
                    return 673;
                case RunningCurrent.mA_71:
                    return 71;
                case RunningCurrent.mA_800:
                    return 800;
                case RunningCurrent.mA_84:
                    return 84;
                default:
                    return 0;
            }
        }
    }
}
