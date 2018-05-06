#pragma once

#include "Treehopper.h"
#include "Pwm.h"

namespace Treehopper {
    class Pin;

    class TreehopperUsb;

/** Built-in hardware PWM channels

\note
Treehopper has two types of PWM support --- Hardware and Software PWM. For information on software PWM functionality, visit Treehopper.Pin.

Treehopper has three 16-bit hardware PWM channels labeled *PWM1*, *PWM2*, and *PWM3*. Like all peripherals, these are accessible from the TreehopperUsb instance.

 */
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

