using System;
using System.Collections;
using System.Linq;
using System.Reflection;

namespace Treehopper.Libraries.Utilities
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class BitfieldAttribute : Attribute
    {
        public BitfieldAttribute(uint length)
        {
            Length = length;
        }

        public uint Length { get; }
    }

    public static class BinaryConverters
    {
        public static byte[] GetBytes(this BitArray array)
        {
            var returnedData = new byte[array.Length / 8];
            for (var i = 0; i < array.Length; i++)
                if (array.Get(i))
                    returnedData[i / 8] |= (byte) (1 << (i % 8));

            return returnedData;
        }

        public static byte ToByte<T>(this T t) where T : struct
        {
            long r = 0;
            var offset = 0;

            // For every field suitably attributed with a BitfieldLength
            foreach (var f in t.GetType().GetRuntimeFields())
            {
                var attrs = f.GetCustomAttributes(typeof(BitfieldAttribute), false);
                if (attrs.Count() == 1)
                {
                    var fieldLength = ((BitfieldAttribute) attrs.First()).Length;

                    // Calculate a bitmask of the desired length
                    var mask = 0;
                    for (var i = 0; i < fieldLength; i++)
                        mask |= 1 << i;

                    if (f.FieldType == typeof(bool))
                        r |= ((bool) f.GetValue(t) ? 0x01 : 0x00) << offset;
                    else // for now, assume some sort of UInt32-castable enum...
                        r |= ((int) f.GetValue(t) & mask) << offset;

                    offset += (int) fieldLength;
                }
            }

            return (byte) r;
        }
    }
}