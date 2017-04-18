using System.Diagnostics.CodeAnalysis;

namespace Treehopper
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal enum DeviceCommands : byte
    {
        Reserved = 0,
        ConfigureDevice,
        PwmConfig,
        UartConfig,
        I2cConfig,
        SpiConfig,
        I2cTransaction,
        SpiTransaction,
        UartTransaction,
        SoftPwmConfig,
        FirmwareUpdateSerial,
        FirmwareUpdateName,
        Reboot,
        EnterBootloader,
        LedConfig,
        ParallelConfig,
        ParallelTransaction
    }
}