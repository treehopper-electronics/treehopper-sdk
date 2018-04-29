#include "TreehopperUsb.h"
#include "HardwarePwm.h"
#include "HardwarePwmManager.h"
#include "Pin.h"

namespace Treehopper {
    HardwarePwm::HardwarePwm(TreehopperUsb &board, int pinNumber) : board(board), pinNumber(pinNumber) {
    }

    bool HardwarePwm::enabled() { return _enabled; }

    void HardwarePwm::enabled(bool value) {
        if (_enabled == value)
            return;

        _enabled = value;
        if (_enabled)
            board.pwmManager.start(*this);
        else
            board.pwmManager.stop(*this);
    }

    HardwarePwm::~HardwarePwm() {
    }

    void HardwarePwm::updateDutyCycle() {
        board.pwmManager.updateDutyCycle(*this);
    }
}