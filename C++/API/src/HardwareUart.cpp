#include "HardwareUart.h"
#include "TreehopperUsb.h"

void Treehopper::HardwareUart::startOneWire() {
    _mode = UartMode::OneWire;
    _enabled = true;
    updateConfig();
}

void Treehopper::HardwareUart::oneWireResetAndMatchAddress(uint64_t address) {
    mode(UartMode::OneWire);
    enabled(true);
    oneWireReset();
    auto data = Utility::getBytes(address, 8);
    data.insert(data.begin(), 0x55); // MATCH ROM
    send(data);
}

std::vector<uint64_t> Treehopper::HardwareUart::oneWireSearch() {

    mode(UartMode::OneWire);
    enabled(true);

    std::vector<uint64_t> retVal;

    uint8_t data[] = {(uint8_t)TreehopperUsb::DeviceCommands::UartTransaction,
                      (uint8_t)UartCommand::OneWireScan};

    {
        _device.sendPeripheralConfigPacket(data, sizeof(data));

        while (true) {
            uint8_t receivedData[9];
            _device.receivePeripheralConfigPacket(receivedData, 9);
            if (receivedData[0] == 0xff)
                break;

            std::vector<uint8_t> receivedWord(&receivedData[1], &receivedData[9]);
            retVal.push_back(Utility::getValue(receivedWord, true));
        }
    }
    return retVal;
}

bool Treehopper::HardwareUart::oneWireReset() {
    mode(UartMode::OneWire);
    enabled(true);
    bool retVal;
    uint8_t data[] = {
            (uint8_t) TreehopperUsb::DeviceCommands::UartTransaction,
            (uint8_t) UartCommand ::OneWireReset
    };

    {
        _device.sendPeripheralConfigPacket(data, 2);
        uint8_t receivedData;
        _device.receivePeripheralConfigPacket(&receivedData, 1);
        retVal = receivedData > 0;
    }

    return retVal;
}

std::vector<uint8_t> Treehopper::HardwareUart::receive(int oneWireNumBytes) {

    uint8_t receivedData[33];
    int len;

    if (_mode == UartMode::Uart)
    {
        if(oneWireNumBytes != 0)
            Utility::error("Since the UART is not in One-Wire Mode, the oneWireNumBytes parameter is ignored");
        uint8_t data[] = {
                (uint8_t)TreehopperUsb::DeviceCommands::UartTransaction,
                (uint8_t)UartCommand::Receive
        };
        {
            _device.sendPeripheralConfigPacket(data, 2);
            _device.receivePeripheralConfigPacket(receivedData, 33);
            len = receivedData[32];
        }
    }
    else
    {
        if (oneWireNumBytes == 0)
            Utility::error("You must specify the number of bytes to receive in One-Wire Mode");
        uint8_t data[] = {
                (uint8_t) TreehopperUsb::DeviceCommands::UartTransaction,
                (uint8_t)UartCommand ::Receive,
                (uint8_t)oneWireNumBytes
        };
        {
            _device.sendPeripheralConfigPacket(data, 3);
            _device.receivePeripheralConfigPacket(receivedData, 33);
            len = receivedData[32];
        }
    }

    return std::vector<uint8_t>(receivedData, (receivedData + len));
}

void Treehopper::HardwareUart::send(std::vector<uint8_t> dataToSend) {
    if (dataToSend.size() > 63)
        Utility::error("The maximum UART length for one transaction is 63 bytes", true);

    uint8_t data[] = {
        (uint8_t)TreehopperUsb::DeviceCommands::UartTransaction,
        (uint8_t)UartCommand::Transmit,
        dataToSend.size()
    };

    dataToSend.insert(dataToSend.begin(), (uint8_t)TreehopperUsb::DeviceCommands::UartTransaction);
    dataToSend.insert(dataToSend.begin()+1, (uint8_t)UartCommand::Transmit);
    dataToSend.insert(dataToSend.begin()+2, dataToSend.size());

    uint8_t receiveData;
    {
        _device.sendPeripheralConfigPacket(dataToSend.data(), dataToSend.size());
        _device.receivePeripheralConfigPacket(&receiveData, 1);
    }
}

void Treehopper::HardwareUart::send(uint8_t dataToSend) {
    std::vector<uint8_t> data = {dataToSend};
    return send(data);
}

void Treehopper::HardwareUart::startUart() {
    _mode = UartMode::Uart;
    _enabled = true;
    updateConfig();
}

std::vector<uint8_t> Treehopper::HardwareUart::receive() {
    std::vector<uint8_t> receivedData;
    return receivedData;
}

void Treehopper::HardwareUart::baud(int baud) {
    if(_baud == baud)
        return;

    _baud = baud;
    updateConfig();
}

int Treehopper::HardwareUart::baud() {
    return _baud;
}

void Treehopper::HardwareUart::mode(Treehopper::UartMode mode) {
    if(_mode == mode)
        return;

    _mode = mode;
    updateConfig();
}

Treehopper::UartMode Treehopper::HardwareUart::mode() {
    return _mode;
}

void Treehopper::HardwareUart::enabled(bool enabled) {
    if(_enabled == enabled)
        return;

    _enabled = enabled;
    updateConfig();
}

bool Treehopper::HardwareUart::enabled() {
    return _enabled;
}

void Treehopper::HardwareUart::updateConfig() {
    if (!_enabled)
    {
        uint8_t dataToSend[2] =
                {(uint8_t)TreehopperUsb::DeviceCommands::UartConfig,
                 (uint8_t)UartConfig::Disabled};

        _device.sendPeripheralConfigPacket(dataToSend, 2);
    }
    else if (_mode == UartMode::Uart)
    {
        uint8_t timerVal;
        bool usePrescaler;

        // calculate baud with and without prescaler
        auto timerValPrescaler = (int) round(256.0 - 2000000.0 / _baud);
        auto timerValNoPrescaler = (int) round(256.0 - 24000000.0 / _baud);

        auto prescalerOutOfBounds = timerValPrescaler > 255 || timerValPrescaler < 0;
        auto noPrescalerOutOfBounds = timerValNoPrescaler > 255 || timerValNoPrescaler < 0;

        // calculate error
        double prescalerError = abs(_baud - 2000000 / (256 - timerValPrescaler));
        double noPrescalerError = abs(_baud - 24000000 / (256 - timerValNoPrescaler));

        if (prescalerOutOfBounds && noPrescalerOutOfBounds)
            Utility::error("The specified baud rate is out of bounds.", true);
        if (prescalerOutOfBounds)
        {
            usePrescaler = false;
            timerVal = (uint8_t) timerValNoPrescaler;
        }
        else if (noPrescalerOutOfBounds)
        {
            usePrescaler = true;
            timerVal = (uint8_t) timerValPrescaler;
        }
        else if (prescalerError > noPrescalerError)
        {
            usePrescaler = false;
            timerVal = (uint8_t) timerValNoPrescaler;
        }
        else
        {
            usePrescaler = true;
            timerVal = (uint8_t) timerValPrescaler;
        }

        uint8_t data[5];
        data[0] = (uint8_t) TreehopperUsb::DeviceCommands::UartConfig;
        data[1] = (uint8_t) UartConfig::Standard;
        data[2] = timerVal;
        data[3] = (uint8_t) (usePrescaler ? 0x01 : 0x00);
        data[4] = (uint8_t) (_useOpenDrainTx ? 0x01 : 0x00);
        _device.sendPeripheralConfigPacket(data, sizeof(data));
    }
    else
    {
        uint8_t data[2];
        data[0] = (uint8_t) TreehopperUsb::DeviceCommands::UartConfig;
        data[1] = (uint8_t) UartConfig::OneWire;
        _device.sendPeripheralConfigPacket(data, sizeof(data));
    }
}
