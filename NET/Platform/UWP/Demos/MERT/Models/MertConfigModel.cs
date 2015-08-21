using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MERT
{
    public class MertConfigModel
    {
        public int NumTrials;
        public int TrialDurationSec;
        public double SamplingRateHz;
        public double DisplayScaleFactorX;
        public double DisplayScaleFactorY;
        public double DisplayOffsetY;
        public double TargetValue;

        public int WarmupTimeSec;

        public double PlottingWindowWidthPx;
        public double PlottingWindowHeightPx;

        public double MaxAirFlowValue;

        public static int DefaultNumTrials = 1;
        public static int DefaultTrialDurationSec = 7;
        public static double DefaultSamplingRateHz = 10;
        public static double DefaultDisplayScaleFactorX = 100;
        public static double DefaultDisplayScaleFactorY = -100;
        public static double DefaultDisplayOffsetY = 100;
        public static double DefaultTargetValue = 0.5;

        public static double DefaultPlottingWindowWidthPx = 700;
        public static double DefaultPlottingWindowHeightPx = 270;
        public static int DefaultWarmupTimeSec = 1;

        public static double DefaultMaxAirFlowValue = 1.5;

        public MertConfigModel()
        {
            NumTrials = DefaultNumTrials;
            TrialDurationSec = DefaultTrialDurationSec;
            SamplingRateHz = DefaultSamplingRateHz;

            DisplayScaleFactorY = DefaultDisplayScaleFactorY;
            DisplayScaleFactorX = DefaultDisplayScaleFactorX;
            DisplayOffsetY = DefaultDisplayOffsetY;
            TargetValue = DefaultTargetValue;

            PlottingWindowWidthPx = DefaultPlottingWindowWidthPx;
            PlottingWindowHeightPx = DefaultPlottingWindowHeightPx;

            MaxAirFlowValue = DefaultMaxAirFlowValue;

            WarmupTimeSec = DefaultWarmupTimeSec;
        }
    }
}
