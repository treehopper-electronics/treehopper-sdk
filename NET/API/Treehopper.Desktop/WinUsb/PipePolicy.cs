namespace Treehopper.Desktop.WinUsb
{
    internal enum PipePolicy : byte
    {
        ShortPacketTerminate = 0x01,
        AutoClearStall = 0x02,
        PipeTransferTimeout = 0x03,
        IgnoreShortPackets = 0x04,
        AllowPartialReads = 0x05,
        AutoFlush = 0x06,
        RawIo = 0x07,
        MaximumTransferSize = 0x08
    }
}