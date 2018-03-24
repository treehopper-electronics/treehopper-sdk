using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WinApi.Kernel32
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct SystemInfo
    {
        internal ushort ProcessorArchitecture;
        ushort Reserved;
        internal uint PageSize;
        internal IntPtr MinimumApplicationAddress;
        internal IntPtr MaximumApplicationAddress;
        internal IntPtr ActiveProcessorMask;
        internal uint NumberOfProcessors;
        internal uint ProcessorType;
        internal uint AllocationGranularity;
        internal ushort ProcessorLevel;
        internal ushort ProcessorRevision;
        internal uint OemId => ((uint) this.ProcessorArchitecture << 8) | this.Reserved;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct SecurityAttributes
    {
        internal uint Length;
        internal IntPtr SecurityDescriptor;
        internal uint IsHandleInheritedValue;

        internal bool IsHandleInherited => this.IsHandleInheritedValue > 0;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct FileTime
    {
        internal uint Low;
        internal uint High;

        internal ulong Value => ((ulong) this.High << 32) | this.Low;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct SystemTime
    {
        internal ushort Year;
        internal ushort Month;
        internal ushort DayOfWeek;
        internal ushort Day;
        internal ushort Hour;
        internal ushort Minute;
        internal ushort Second;
        internal ushort Milliseconds;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct FileAttributeData
    {
        internal FileAttributes Attributes;
        internal FileTime CreationTime;
        internal FileTime LastAccessTime;
        internal FileTime LastWriteTime;

        internal uint FileSizeHigh;
        internal uint FileSizeLow;

        internal ulong FileSize => ((ulong) this.FileSizeHigh << 32) | this.FileSizeLow;
    }
}