#include "TreehopperUsb.h"
#include "HardwarePwmManager.h"
#include "HardwarePwm.h"
#include "Pin.h"
#include <cmath>

namespace Treehopper {

    HardwarePwmManager::HardwarePwmManager(TreehopperUsb &board) : board(board) {
        config.opcode = (uint8_t) TreehopperUsb::DeviceCommands::PwmConfig;
    }

    void HardwarePwmManager::frequency(HardwarePwmFrequency value) {
        config.frequency = value;
    }

    HardwarePwmFrequency HardwarePwmManager::frequency() {
        return HardwarePwmFrequency();
    }

    void HardwarePwmManager::updateDutyCycle(HardwarePwm &pin) {
        uint16_t registerValue = (uint16_t) round(pin.dutyCycle() * (double) (UINT16_MAX));
        switch (pin.pinNumber) {
            case 7:
                config.DutyCycle7_Lo = registerValue & 0xFF;
                config.DutyCycle7_Hi = registerValue >> 8;
                break;
            case 8:
                config.DutyCycle8_Lo = registerValue & 0xFF;
                config.DutyCycle8_Hi = registerValue >> 8;
                break;
            case 9:
                config.DutyCycle9_Lo = registerValue & 0xFF;
                config.DutyCycle9_Hi = registerValue >> 8;
                break;
        }

        sendConfig();
    }

    void HardwarePwmManager::start(HardwarePwm &pin) {
        // first check to make sure the previous PWM pins have been enabled first.
        if ((pin.pinNumber == 8) && (config.mode != EnableMode::Pin7))
            throw "You must enable PWM functionality on Pin 8 (PWM1) before you enable PWM functionality on Pin 9 (PWM2). See http://treehopper.io/pwm";
        if ((pin.pinNumber == 9) && (config.mode != EnableMode::Pin7_Pin8))
            throw "You must enable PWM functionality on Pin 8 and 9 (PWM1 and PWM2) before you enable PWM functionality on Pin 10 (PWM3). See http://treehopper.io/pwm";

        switch (pin.pinNumber) {
            case 7:
                config.mode = EnableMode::Pin7;
                break;
            case 8:
                config.mode = EnableMode::Pin7_Pin8;
                break;
            case 9:
                config.mode = EnableMode::Pin7_Pin8_Pin9;
                break;
        }

        sendConfig();
    }

    void HardwarePwmManager::stop(HardwarePwm &pin) {
        // first check to make sure the higher PWM pins have been disabled first
        if ((pin.pinNumber == 8) && (config.mode != EnableMode::Pin7_Pin8))
            throw "You must disable PWM functionality on Pin 10 (PWM3) before disabling Pin 9's PWM functionality. See http://treehopper.io/pwm";
        if ((pin.pinNumber == 7) && (config.mode != EnableMode::Pin7))
            throw "You must disable PWM functionality on Pin 9 and 10 (PWM2 and PWM3) before disabling Pin 8's PWM functionality. See http://treehopper.io/pwm";

        switch (pin.pinNumber) {
            case 7:
                config.mode = EnableMode::None;
                break;
            case 8:
                config.mode = EnableMode::Pin7;
                break;
            case 9:
                config.mode = EnableMode::Pin7_Pin8;
                break;
        }

        sendConfig();
    }

    void HardwarePwmManager::sendConfig() {
        board.sendPeripheralConfigPacket((uint8_t *) &config, sizeof(config));
    }

}
