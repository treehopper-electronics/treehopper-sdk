using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Utilities
{
    /// <summary>
    /// Utilities for dealing with numbers
    /// </summary>
    public static class Numbers
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
        /// Constrains a number to be within a range.
        /// </summary>
        /// <param name="x">the number to constrain</param>
        /// <param name="a">the lower end of the range</param>
        /// <param name="b">the upper end of the range</param>
        /// <returns>the constrained number</returns>
        public static T Constrain<T>(T x, T a, T b) where T : IComparable
        {
            if (x.CompareTo(a) < 0)
                return a;
            if (x.CompareTo(b) > 0)
                return b;
            return x;
        }

        // from Microsoft.Xna.Framework
        /// <summary>
        /// Linearly interpolates between two values.
        /// This method is a less efficient, more precise version of <see cref="Lerp"/>.
        /// See remarks for more info.
        /// </summary>
        /// <param name="value1">Source value.</param>
        /// <param name="value2">Destination value.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of value2.</param>
        /// <returns>Interpolated value.</returns>
        /// <remarks>This method performs the linear interpolation based on the following formula:
        /// <code>((1 - amount) * value1) + (value2 * amount)</code>.
        /// Passing amount a value of 0 will cause value1 to be returned, a value of 1 will cause value2 to be returned.
        /// This method does not have the floating point precision issue that <see cref="Lerp"/> has.
        /// i.e. If there is a big gap between value1 and value2 in magnitude (e.g. value1=10000000000000000, value2=1),
        /// right at the edge of the interpolation range (amount=1), <see cref="Lerp"/> will return 0 (whereas it should return 1).
        /// This also holds for value1=10^17, value2=10; value1=10^18,value2=10^2... so on.
        /// For an in depth explanation of the issue, see below references:
        /// Relevant Wikipedia Article: https://en.wikipedia.org/wiki/Linear_interpolation#Programming_language_support
        /// Relevant StackOverflow Answer: http://stackoverflow.com/questions/4353525/floating-point-linear-interpolation#answer-23716956
        /// </remarks>
        public static float Lerp(float value1, float value2, float amount)
        {
            return ((1 - amount) * value1) + (value2 * amount);
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
