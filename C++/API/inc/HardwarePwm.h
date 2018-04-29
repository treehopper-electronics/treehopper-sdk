#pragma once

#include "Treehopper.h"
#include "Pwm.h"

namespace Treehopper {
    class Pin;

    class TreehopperUsb;

    /** A hardware PWM pin */
    class TREEHOPPER_API HardwarePwm : public Pwm {
        friend class HardwarePwmManager;

    public:
        HardwarePwm(TreehopperUsb &board, int pinNumber);

        ~HardwarePwm() override;

        /** Gets whether this hardware PWM pin is enabled */
        bool enabled() override;

        /** Sets whether this hardware PWM pin is enabled */
        void enabled(bool value) override;

    protected:
        void updateDutyCycle() override;

        bool _enabled;
        int pinNumber;
        TreehopperUsb &board;
    };
}

