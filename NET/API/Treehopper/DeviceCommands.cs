namespace Treehopper
{
    internal enum DeviceCommands : byte
    {
        Reserved = 0,   // Not implemented
        ConfigureDevice,    // Sent upon device connect/disconnect
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
