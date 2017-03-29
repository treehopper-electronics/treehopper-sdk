package io.treehopper;

/**
 * Created by jay on 12/6/2016.
 */
enum DeviceCommands {
    Reserved,    // Not implemented
    ConfigureDevice,    // Sent upon device connect/disconnect
    PwmConfig,    // Configures the hardware DAC
    UartConfig,    // Not implemented
    I2cConfig,    // Configures i2C master
    SpiConfig,    // Configures SPI master
    I2cTransaction,    // (Endpoint 2) Performs an i2C transaction
    SpiTransaction,    // (Endpoint 2) Performs an SPI transaction
    SoftPwmConfig,    //
    ServoControllerConfig,
    FirmwareUpdateSerial,    //
    FirmwareUpdateName,    //
    Reboot,    //
    EnterBootloader,    //
    LedConfig
}
