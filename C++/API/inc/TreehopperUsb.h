#pragma once

#include "Treehopper.h"
#include "UsbConnection.h"
#include <memory>
#include <vector>
#include "Pin.h"
#include "HardwarePwmManager.h"
#include "HardwarePwm.h"
#include "HardwareI2c.h"
#include "HardwareSpi.h"
#include "HardwareUart.h"
#include <thread>

using namespace std;

namespace Treehopper {
    class Pwm;

    class I2c;

/** The core class for communicating with %Treehopper USB boards.

![Treehopper pinout](images/treehopper-hardware.svg)
# Core Hardware {#core-hardware}
%Treehopper is a USB 2.0 Full Speed device with 20 \link Pin Pins\endlink — each of which can be used as an analog input, digital input, digital output, or soft-PWM output. Many of these pins also have dedicated peripheral functions for \link HardwareSpi SPI\endlink, \link HardwareI2c I2C\endlink, \link HardwareUart UART\endlink, and \link HardwarePwm PWM\endlink.

You'll access all the pins, peripherals, and board functions through this class, which will automatically create all peripheral instances for you.

## Getting a board reference
To obtain a reference to the board, use the \link ConnectionService.getFirstDevice() ConnectionService::instance().getFirstDevice()\endlink method:

```{.cpp}
auto board = ConnectionService::instance().getFirstDevice();
```

Or the \link ConnectionService.boards ConnectionService::instance().boards\endlink vector.

\warning While you're free to create TreehopperUsb variables that reference boards, never call %TreehopperUsb's constructor yourself.

## Connect to the board
Before you use the board, you must explicitly connect to it by calling the ConnectAsync() method

```{.c}
auto board = ConnectionService::instance().getFirstDevice();
board.connect();
```

\note Once a board is connected, other applications won't be able to use it. When your app is running in Windows or macOS, you can investigate the #name or #serialNumber properties before calling connect(). This will let you determine which board to connect to if you have multiple boards used by multiple applications.

## Next steps
To learn about accessing different %Treehopper peripherals, visit the doc links to the relevant classes:
 - Pin
 - HardwareSpi
 - HardwareI2c
 - HardwareUart
 - HardwarePwm
*/
    class TREEHOPPER_API TreehopperUsb {
        friend Pin;
        friend HardwareI2c;
        friend UsbConnection;
        friend HardwarePwmManager;
        friend HardwareSpi;
        friend HardwareUart;

    public:
        TreehopperUsb(UsbConnection &connection);

        ~TreehopperUsb();

        TreehopperUsb &operator=(const TreehopperUsb &rhs) {
            connection = rhs.connection;
            isConnected = rhs.isConnected;

            return *this;
        }

        TreehopperUsb(const TreehopperUsb &rhs) : TreehopperUsb(rhs.connection) {
            connection = rhs.connection;
            isConnected = rhs.isConnected;
        }

        /// <summary>
        ///     Reinitialize the board, setting all pins as high-impedance
        /// </summary>
        void reinitialize();

        /** Gets whether or not the board is connected */
        bool isConnected;

        /** Establish a connection with the board */
        bool connect();

        /** Disconnect from the board */
        void disconnect();

        /** Gets the serial number of the board */
        wstring serialNumber();

        /** Gets the name of the board */
        wstring name();

        /** Sets the state of the LED */
        void led(bool value);

        /** Gets the state of the LED */
        bool led();

        /** Gets a string representation of the board */
        wstring toString() {
            wstring output = name() + L" (" + serialNumber() + L")";
            return output;
        }

        /** Gets a collection of pins on the board */
        vector<Pin> pins;

        /** Gets the number of pins on the board (20) */
        const int numberOfPins = 20;

        /** Gets the HardwareI2c interface */
        HardwareI2c i2c;

        /** Gets the HardwareSpi interface */
        HardwareSpi spi;

        /** Gets the HardwarePwm PWM1 pin */
        HardwarePwm pwm1;

        /** Gets the HardwarePwm PWM2 pin */
        HardwarePwm pwm2;

        /** Gets the HardwarePwm PWM3 pin */
        HardwarePwm pwm3;

        /** Gets the HardwarePwmManager module */
        HardwarePwmManager pwmManager;
    private:
        void sendPinConfigPacket(uint8_t *data, size_t len);

        void sendPeripheralConfigPacket(uint8_t *data, size_t len);

        void receivePeripheralConfigPacket(uint8_t *data, size_t numBytesToRead);

        enum class DeviceCommands {
            Reserved = 0,   // Not implemented
            ConfigureDevice,    // Sent upon board connect/disconnect
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
        };
        UsbConnection &connection;
        thread pinListenerThread;

        void pinStateListener();

        uint8_t buffer[41];
        bool _led;
        bool _isDestroyed = false;
    };
}