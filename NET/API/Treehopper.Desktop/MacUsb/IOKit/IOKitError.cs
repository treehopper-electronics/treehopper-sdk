using System.Diagnostics.CodeAnalysis;

namespace Treehopper.Desktop.MacUsb.IOKit
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum IOKitError : uint
    {
        // IOKit
        Success = 0,
        Error = 0xe00002bc, // general error 	
        NoMemory = 0xe00002bd, // can't allocate memory 
        NoResources = 0xe00002be, // resource shortage 
        IPCError = 0xe00002bf, // error during IPC 
        NoDevice = 0xe00002c0, // no such device 
        NotPrivileged = 0xe00002c1, // privilege violation 
        BadArgument = 0xe00002c2, // invalid argument 
        LockedRead = 0xe00002c3, // device read locked 
        LockedWrite = 0xe00002c4, // device write locked 
        ExclusiveAccess = 0xe00002c5, // exclusive access and
        BadMessageID = 0xe00002c6, // sent/received messages
        Unsupported = 0xe00002c7, // unsupported function 
        VMError = 0xe00002c8, // misc. VM failure 
        InternalError = 0xe00002c9, // internal error 
        IOError = 0xe00002ca, // General I/O error 
        CannotLock = 0xe00002cc, // can't acquire lock
        NotOpen = 0xe00002cd, // device not open 
        NotReadable = 0xe00002ce, // read not supported 
        NotWritable = 0xe00002cf, // write not supported 
        NotAligned = 0xe00002d0, // alignment error 
        BadMedia = 0xe00002d1, // Media Error 
        StillOpen = 0xe00002d2, // device(s, still open 
        RLDError = 0xe00002d3, // rld failure 
        DMAError = 0xe00002d4, // DMA failure 
        Busy = 0xe00002d5, // Device Busy 
        Timeout = 0xe00002d6, // I/O Timeout 
        Offline = 0xe00002d7, // device offline 
        NotReady = 0xe00002d8, // not ready 
        NotAttached = 0xe00002d9, // device not attached 
        NoChannels = 0xe00002da, // no DMA channels left
        NoSpace = 0xe00002db, // no space for data 
        PortExists = 0xe00002dd, // port already exists
        CannotWire = 0xe00002de, // can't wire down 
        NoInterrupt = 0xe00002df, // no interrupt attached
        NoFrames = 0xe00002e0, // no DMA frames enqueued
        MessageTooLarge = 0xe00002e1, // oversized msg received
        NotPermitted = 0xe00002e2, // not permitted
        NoPower = 0xe00002e3, // no power to device
        NoMedia = 0xe00002e4, // media not present
        UnformattedMedia = 0xe00002e5, // media not formatted
        UnsupportedMode = 0xe00002e6, // no such mode
        Underrun = 0xe00002e7, // data underrun
        Overrun = 0xe00002e8, // data overrun
        DeviceError = 0xe00002e9, // the device is not working properly!
        NoCompletion = 0xe00002ea, // a completion routine is required
        Aborted = 0xe00002eb, // operation aborted
        NoBandwidth = 0xe00002ec, // bus bandwidth would be exceeded
        NotResponding = 0xe00002ed, // device not responding
        IsoTooOld = 0xe00002ee, // isochronous I/O request for distant past!
        IsoTooNew = 0xe00002ef, // isochronous I/O request for distant future
        NotFound = 0xe00002f0, // data was not found
        Invalid = 0xe00001, // should never be seen


        // IOUSBLib
        PipeRefNotRecognized = 0xe0004061,
        TooManyPipes = 0xe0004060,
        NoAsyncPort = 0xe000405f,
        NotEnoughPipes = 0xe000405e,
        NotEnoughPower = 0xe000405d,
        EndpointNotFound = 0xe0004057,
        ConfigurationNotFound = 0xe0004056,
        TransactionTimedOut = 0xe0004051,
        TransactionReturnedToCaller = 0xe0004050,
        PipeHasStalled = 0xe000404f,
        InterfaceNotFound = 0xe000404e,
        SyncRequestMadeOnWorkloopThread = 0xe000404a
    }
}