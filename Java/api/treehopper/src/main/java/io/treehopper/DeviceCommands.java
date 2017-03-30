package io.treehopper;

/**
 * Created by jay on 12/6/2016.
 */
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
