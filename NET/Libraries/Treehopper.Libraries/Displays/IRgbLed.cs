using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Displays
{
    interface IRgbLed
    {
        float RedGain { get; set; }
        float GreenGain { get; set; }
        float BlueGain { get; set; }

        void SetRgb(Color color);
        void SetRgb(float red, float green, float blue);
        void SetHsl(float hue, float saturation, float luminance);
    }
}
