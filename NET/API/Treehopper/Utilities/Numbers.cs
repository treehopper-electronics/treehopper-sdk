using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Utilities
{
    public static class Numbers
    {
        public static byte[] GetBytes(this BitArray array)
        {
            var returnedData = new byte[array.Length / 8];
            for (int i = 0; i < array.Length; i++)
            {
                if (array.Get(i))
                    returnedData[i / 8] |= (byte)(1 << (i % 8));
            }

            return returnedData;
        }
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
            return (((input - fromMin) * (toMax - toMin)) / (fromMax - fromMin)) + toMin;
        }

        /// <summary>
        /// Constrains a number to be within a range. Default range is 0-1
        /// </summary>
        /// <param name="x">the number to constrain</param>
        /// <param name="a">the lower end of the range</param>
        /// <param name="b">the upper end of the range</param>
        /// <returns>the constrained number</returns>
        public static double Constrain(this double x, double a = 0.0, double b = 1.0)
        {
            if (x < a)
                return a;
            if (x > b)
                return b;
            return x;
        }

        /// <summary>
        /// Convert 4-bit BCD nibbles into an integer
        /// </summary>
        /// <param name="val">The value (concatenated 4-bit nibbles) to convert.</param>
        /// <returns>The integer number</returns>
        public static int BcdToInt(int val)
        {
            int retVal = 0;
            for (int i = 0; i < 8; i++)
            {
                retVal += (int)Math.Pow(10, i) * (val >> (i * 4) & 0xF);
            }

            return retVal;
        }


        /// <summary>
        /// Determines if this number is close to another. Useful for comparing doubles and floats
        /// </summary>
        /// <param name="val">This number</param>
        /// <param name="comparedTo">The other number</param>
        /// <param name="error">The allowable error; defaults to something sensible</param>
        /// <returns>True if the two numbers are within "error" of each other; false otherwise</returns>
        public static bool CloseTo(this double val, double comparedTo, double error = 0.0001)
        {
            return Math.Abs(val - comparedTo) < error;
        }

        /// <summary>
        /// Determines if this number is close to another. Useful for comparing doubles and floats
        /// </summary>
        /// <param name="val">This number</param>
        /// <param name="comparedTo">The other number</param>
        /// <param name="error">The allowable error; defaults to something sensible</param>
        /// <returns>True if the two numbers are within "error" of each other; false otherwise</returns>
        public static bool CloseTo(this float val, float comparedTo, double error = 0.0001)
        {
            return Math.Abs(val - comparedTo) < error;
        }

        /// <summary>
        /// Determines if this number is close to another. Useful for comparing doubles and floats
        /// </summary>
        /// <param name="val">This number</param>
        /// <param name="comparedTo">The other number</param>
        /// <param name="error">The allowable error; defaults to something sensible</param>
        /// <returns>True if the two numbers are within "error" of each other; false otherwise</returns>
        public static bool CloseTo(this int val, int comparedTo, double error = 0.0001)
        {
            return Math.Abs(val - comparedTo) < error;
        }
    }
}
