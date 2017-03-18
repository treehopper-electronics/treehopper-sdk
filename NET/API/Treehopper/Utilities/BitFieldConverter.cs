using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Utilities
{
    [global::System.AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class BitfieldAttribute : Attribute
    {
        uint length;

        public BitfieldAttribute(uint length)
        {
            this.length = length;
        }
        public uint Length { get { return length; } }
    }
    public static class BitFieldConverter
    {
        public static byte ToByte<T>(this T t) where T : struct
        {
            long r = 0;
            int offset = 0;

            // For every field suitably attributed with a BitfieldLength
            foreach (FieldInfo f in t.GetType().GetRuntimeFields())
            {
                var attrs = f.GetCustomAttributes(typeof(BitfieldAttribute), false);
                if (attrs.Count() == 1)
                {
                    uint fieldLength = ((BitfieldAttribute)attrs.First()).Length;

                    // Calculate a bitmask of the desired length
                    int mask = 0;
                    for (int i = 0; i < fieldLength; i++)
                        mask |= 1 << i;

                    if (f.FieldType == typeof(Boolean))
                        r |= ((bool)f.GetValue(t) ? 0x01 : 0x00) << offset;
                    else // for now, assume some sort of UInt32-castable enum...
                        r |= ((int)f.GetValue(t) & mask) << offset;

                    offset += (int)fieldLength;
                }
            }

            return (byte)r;
        }

    }
}
