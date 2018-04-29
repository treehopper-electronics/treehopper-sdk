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
%Treehopper is a USB 2.0 Full Speed device with 20 \link Pin Pins\endlink — each of which can be used as an analog input, digital input, digital output, or soft-PWM output. Many of these pins also have dedicated peripheral functions for \link HardwareSpi SPI\endlink, \link HardwareI2C I2C\endlink, \link HardwareUart UART\endlink, and \link HardwarePwm PWM\endlink.

You'll access all the pins, peripherals, and board functions through this class, which will automatically create all peripheral instances for you.

## Getting a board reference
To obtain a reference to the board, use the \link Treehopper.ConnectionService.GetFirstDeviceAsync() ConnectionService.Instance.GetFirstDeviceAsync()\endlink method:

```{.c}
// Get a reference to the connected board
auto board = ConnectionService::instance().getFirstDevice();
// now do something with the newly-attached board
```

Or the \link Treehopper.ConnectionService.Boards ConnectionService.Instance.Boards\endlink ObservableCollection, which fires events useful for hot-plug reactions:

```
ConnectionService.Instance.Boards.CollectionChanged += async (o, e) =>
{
        var board = (TreehopperUsb)e.NewItems[0];
        // do something with the new board
}
```

\warning While you're free to create TreehopperUsb variables that reference boards, never call %TreehopperUsb's constructor yourself.

## Connect to the board
Before you use the board, you must explicitly connect to it by calling the ConnectAsync() method

```
var board = await ConnectionService.Instance.GetFirstDeviceAsync();
await board.ConnectAsync();
```

\note Once a board is connected, other applications won't be able to use it. When your app is running in Windows or macOS, you can investigate the #Name or #SerialNumber properties before calling ConnectAsync(). This will let you determine which board to connect to if you have multiple boards used by multiple applications.

## Next steps
To learn about accessing different %Treehopper peripherals, visit the doc links to the relevant classes:
 - Pin
 - HardwareSpi
 - HardwareI2C
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

        void reinitialize();

        bool isConnected;

        bool connect();

        void disconnect();

        wstring serialNumber();

        wstring name();

        void led(bool value);

        bool led();

        wstring toString() {
            wstring output = name() + L" (" + serialNumber() + L")";
            return output;
        }

        vector<Pin> pins;
        const int numberOfPins = 20;

        HardwareI2c i2c;
        HardwareSpi spi;
        HardwarePwm pwm1;
        HardwarePwm pwm2;
        HardwarePwm pwm3;
        HardwarePwmManager pwmManager;
    private:
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
    protected:
        void sendPinConfigPacket(uint8_t *data, size_t len);

        void sendPeripheralConfigPacket(uint8_t *data, size_t len);

        void receivePeripheralConfigPacket(uint8_t *data, size_t numBytesToRead);
    };
}