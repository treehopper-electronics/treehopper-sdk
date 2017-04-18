namespace Treehopper.Utilities
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;

    /// <summary>
    /// This is a utility class that contains convenient static methods.
    /// </summary>
    public static class Utility
    {
        private static readonly Random random = new Random();

        /// <summary>
        /// Do something with each item in a list, collection, or enumerable
        /// </summary>
        /// <typeparam name="T">The type of the enumerable</typeparam>
        /// <param name="enumeration">The collection, list, etc to operate over</param>
        /// <param name="action">The Action to perform on each item</param>
        public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
        {
            foreach (var item in enumeration)
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
            var retVal = string.Empty;
            var nonZeroFound = false;
            for (var i = 7; i >= 0; i--)
            {
                var num = val >> (i * 4) & 0xF;
                if (showLeadingZeros || num != 0 || nonZeroFound || i == decimalPlace)
                    retVal += num.ToString();

                if (num != 0 || i == decimalPlace)
                    nonZeroFound = true;

                if (i == decimalPlace)
                    retVal += ".";
            }

            return retVal;
        }

        /// <summary>
        /// Empty method which prevents VS from generating warnings from un-awaited calls. 
        /// </summary>
        /// <param name="task"></param>
        public static void Forget(this Task task)
        {
        }

        /// <summary>
        /// Get a random string with alphanumeric characters
        /// </summary>
        /// <param name="length">The number of characters the string should have</param>
        /// <returns>the generated string</returns>
        public static string RandomString(int length)
        {
            const string Characters = "abcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(Characters, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        /// <summary>
        /// Emit an error message
        /// </summary>
        /// <param name="message">The error to emit</param>
        /// <param name="fatal">Whether this error is fatal, and should throw an exception</param>
        public static void Error(string message, bool fatal = false)
        {
            Debug.WriteLine("ERROR: " + message);
            if (TreehopperUsb.Settings.ThrowExceptions || fatal)
                throw new TreehopperRuntimeException(message);
        }

        /// <summary>
        /// Print an error to the Debug console and throw an exception if the Settings permit
        /// </summary>
        /// <param name="ex">The exception message to print</param>
        public static void Error(Exception ex)
        {
            Debug.WriteLine("ERROR: " + ex.Message);
            if (TreehopperUsb.Settings.ThrowExceptions)
                throw ex;
        }
    }
}
