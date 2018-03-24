using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Treehopper.Libraries.Utilities
{
    public enum Endianness
    {
        BigEndian,
        LittleEndian
    }

    /// <summary>
    ///     Convert structs between byte arrays, with controlled endianness.
    /// </summary>
    /** 
    This class can be used to marshal between byte arrays and structs containing multi-byte numbers. This is
    somewhat challenging because:

     - **.NET preserves the endianness of the processor**. While most (99%?) of Treehopper .NET code runs on x86 or x64 machines (which are
    little-endian), this code base is designed to be cross-platform, and runs well on ARM processors (which are traditionally big-endian). Little-endian byte arrays must be converted to big-endian when executing on architectures that are big-endian before being marshaled
     - **Different applications expect different endianness**. If a configuration struct is to be passed to, say, an <see cref="SMBusDevice" /> peripheral, the bytes produced by the conversion must match the endianness the peripheral expects; this is not standardized for multibyte transactions.

    This class solves both of these issues by allowing the user to select the desired endianness, then converting (if necessary) by using Reflection to inspect the struct's fields.

    This class is adapted from http://stackoverflow.com/questions/2480116.
     */
    public static class StructConverter
    {
        private static void MaybeAdjustEndianness(Type type, byte[] data, Endianness endianness, int startOffset = 0)
        {
            if (BitConverter.IsLittleEndian == (endianness == Endianness.LittleEndian))
                return;

            foreach (var field in type.GetRuntimeFields())
            {
                var fieldType = field.FieldType;
                if (field.IsStatic)
                    // don't process static fields
                    continue;

                if (fieldType == typeof(string))
                    // don't swap bytes for strings
                    continue;

                var offset = Marshal.OffsetOf(type, field.Name).ToInt32();

                // handle enums
                if (fieldType == typeof(Enum))
                    fieldType = Enum.GetUnderlyingType(fieldType);

                // check for sub-fields to recurse if necessary
                var subFields = fieldType.GetRuntimeFields()
                    .Where(subField => subField.IsStatic == false && subField.IsPublic)
                    .ToArray();

                var effectiveOffset = startOffset + offset;

                if (subFields.Length == 0)
                    Array.Reverse(data, effectiveOffset, Marshal.SizeOf(fieldType));
                else
                    MaybeAdjustEndianness(fieldType, data, endianness, effectiveOffset);
            }
        }

        public static T BytesToStruct<T>(this byte[] rawData, Endianness endianness) where T : struct
        {
            var result = default(T);

            MaybeAdjustEndianness(typeof(T), rawData, endianness);

            var handle = GCHandle.Alloc(rawData, GCHandleType.Pinned);

            try
            {
                var rawDataPtr = handle.AddrOfPinnedObject();
                result = (T) Marshal.PtrToStructure(rawDataPtr, typeof(T));
            }
            finally
            {
                handle.Free();
            }

            return result;
        }

        public static byte[] StructToBytes<T>(this T data, Endianness endianness) where T : struct
        {
            var rawData = new byte[Marshal.SizeOf(data)];
            var handle = GCHandle.Alloc(rawData, GCHandleType.Pinned);
            try
            {
                var rawDataPtr = handle.AddrOfPinnedObject();
                Marshal.StructureToPtr(data, rawDataPtr, false);
            }
            finally
            {
                handle.Free();
            }

            MaybeAdjustEndianness(typeof(T), rawData, endianness);

            return rawData;
        }
    }
}