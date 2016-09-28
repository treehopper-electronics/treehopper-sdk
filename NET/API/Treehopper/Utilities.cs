﻿using System;
using System.Linq;

namespace Treehopper
{
    /// <summary>
    /// This is a utility class that contains convenient static methods.
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        /// Map an input value from the specified range to an alternative range.
        /// </summary>
        /// <param name="input">The value to map</param>
        /// <param name="fromMin">The minimum value the input can take on</param>
        /// <param name="fromMax">The maximum value the input can take on</param>
        /// <param name="toMin">The minimum output</param>
        /// <param name="toMax">The maximum output</param>
        /// <returns>The mapped value</returns>
        /// <remarks>
        /// <para>
        /// This is a clone of the Arduino map() function (http://arduino.cc/en/reference/map). 
        /// </para>
        /// </remarks>
        /// <example>
        /// Map the AnalogIn voltage (which ranges from 0-5) to a Pwm.Value (which ranges from 0-1).
        /// <code>
        /// double pwmVal = Utilities.Map(myTreehopperBoard.Pin1.AnalogIn.Value, 0, 5, 0, 1);
        /// myTreehopperBoard.Pin2.Pwm.DutyCycle = pwmVal;
        /// </code>
        /// </example>
        public static double Map(double input, double fromMin, double fromMax, double toMin, double toMax)
        {
            return (input - fromMin) * (toMax - toMin) / (fromMax - fromMin) + toMin;
        }

        /// <summary>
        /// Constrains a number to be within a range
        /// </summary>
        /// <param name="x">the number to constrain</param>
        /// <param name="a">the lower end of the range</param>
        /// <param name="b">the upper end of the range</param>
        /// <returns>the constrained number</returns>
        public static double Constrain(double x, double a, double b)
        {
            if (x < a)
                return a;
            if (x > b)
                return b;
            return x;
        }

        public static int BcdToInt(int val)
        {
            int retVal = 0;
            for(int i=0;i<8;i++)
            {
                retVal += (int)Math.Pow(10, i) * (val >> (i * 4) & 0xF);
            }
            return retVal;
        }

        public static string BcdToString(int val, int decimalPlace = 0, bool showLeadingZeros = false)
        {
            string retVal = "";
            bool nonZeroFound = false;
            for (int i = 7; i >= 0; i--)
            {
                int num = (val >> (i * 4) & 0xF);
                if(showLeadingZeros || num != 0 || nonZeroFound || i == decimalPlace)
                    retVal += num.ToString();

                if (num != 0 || i == decimalPlace)
                    nonZeroFound = true;

                if (i == decimalPlace)
                    retVal += ".";
            }
            return retVal;
        }

        static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
