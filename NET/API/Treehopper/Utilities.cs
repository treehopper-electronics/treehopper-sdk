using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

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
        /// Constrains a number to be within a range. Default range is 0-1
        /// </summary>
        /// <param name="x">the number to constrain</param>
        /// <param name="a">the lower end of the range</param>
        /// <param name="b">the upper end of the range</param>
        /// <returns>the constrained number</returns>
        public static double Constrain(double x, double a = 0.0, double b = 1.0)
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
            for(int i=0;i<8;i++)
            {
                retVal += (int)Math.Pow(10, i) * (val >> (i * 4) & 0xF);
            }
            return retVal;
        }

        /// <summary>
        /// Returns the CIE 1976 relative luminance calculated from an input intensity (brightness)
        /// </summary>
        /// <param name="Brightness">The input intensity, from 0.0-1.0, to convert</param>
        /// <returns>The CIE 1976 relative luminance, normalized to a 0-1 scale</returns>
        /// <remarks>
        /// <para>"Brightness" is perception oriented for intuitiveness. This function can be used to convert the perceptual value to a normalized luminance value that an LED driver can use to determine the correct PWM duty cycle or analog current drive strength needed to produce the associated brightness.</para>
        /// </remarks>
        public static double BrightnessToCieLuminance(double Brightness)
        {
            if (Brightness > 0.008856)
                return (15625 * Math.Pow(Brightness, 3) + 7500 * Math.Pow(Brightness, 2) + 1200 * Brightness + 64) / 24389.0;
            else
                return 1000 * Brightness / 9033.0;
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

        /// <summary>
        /// Do something with each item in a list, collection, or enumerable
        /// </summary>
        /// <typeparam name="T">The type of the enumerable</typeparam>
        /// <param name="enumeration">The collection, list, etc to operate over</param>
        /// <param name="action">The Action to perform on each item</param>
        public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
        {
            foreach (T item in enumeration)
            {
                action(item);
            }
        }

        /// <summary>
        /// Convert 4-bit BCD nibbles to a string
        /// </summary>
        /// <param name="val">The number (composed of 4-bit nibbles) to convert</param>
        /// <param name="decimalPlace">Where to draw a decimal point</param>
        /// <param name="showLeadingZeros">Whether to show leading zeros</param>
        /// <returns>The string</returns>
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

        /// <summary>
        /// Get a random string with alphanumeric characters
        /// </summary>
        /// <param name="length">The number of characters the string should have</param>
        /// <returns>the generated string</returns>
        public static string RandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        // adapted from stackoverflow.com/questions/2871/reading-a-c-c-data-structure-in-c-sharp-from-a-byte-array
        static T ByteArrayToStructure<T>(byte[] bytes) where T : struct
        {
            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            T dataStruct = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            handle.Free();
            return dataStruct;
        }

        internal static void Error(string message)
        {
            Debug.WriteLine("ERROR: " + message);
            if (TreehopperUsb.Settings.ThrowExceptions)
                throw new TreehopperRuntimeException(message);
        }

        internal static void Error(Exception ex)
        {
            Debug.WriteLine("ERROR: " + ex.Message);
            if (TreehopperUsb.Settings.ThrowExceptions)
                throw ex;
        }
    }

    public class TreehopperRuntimeException : Exception
    {
        private string message;
        public TreehopperRuntimeException(string message)
        {
            this.message = message;
        }

        public override string Message
        {
            get
            {
                return message;
            }
        }
    }
}
