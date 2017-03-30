package io.treehopper;

enum DeviceCommands {
    Reserved,
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
