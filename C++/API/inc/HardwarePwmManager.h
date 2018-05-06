#pragma once

#include "Treehopper.h"
#include "HardwarePwmFrequency.h"
#include "HardwarePwm.h"
#include "Pin.h"

namespace Treehopper {
    class TreehopperUsb;

/// <summary>
///     Hardware PWM manager
/// </summary>
/**
This class wouldn't typically be exposed to the user, but it provides one useful property: #HardwarePwmFrequency.

With it, you can change the frequency that PWM1, PWM2, and PWM3 operate at. By default, the board uses a frequency of 732Hz, which helps avoid any trace of flicker when driving LEDs or other lighting. If you're driving motors or other inductive loads, you may wish to lower the frequency to reduce switching losses, or to lower the tone of the motor.  
*/
    class TREEHOPPER_API HardwarePwmManager {
        friend class HardwarePwm;

    public:
        HardwarePwmManager(TreehopperUsb &board);

        /** Sets the HardwarePwmFrequency to be used by the HardwarePwm pins */
        void frequency(HardwarePwmFrequency value);

        /** Gets the current HardwarePwmFrequency being used */
        HardwarePwmFrequency frequency();

    protected:
        enum class EnableMode : uint8_t {
            None,
            Pin7,
            Pin7_Pin8,
            Pin7_Pin8_Pin9
        };

        struct Configuration {
            uint8_t opcode;
            EnableMode mode;
            HardwarePwmFrequency frequency;
            uint8_t DutyCycle7_Lo;
            uint8_t DutyCycle7_Hi;
            uint8_t DutyCycle8_Lo;
            uint8_t DutyCycle8_Hi;
            uint8_t DutyCycle9_Lo;
            uint8_t DutyCycle9_Hi;
        } config;

        TreehopperUsb &board;

        void sendConfig();

        void updateDutyCycle(HardwarePwm &pin);

        void start(HardwarePwm &pin);

        void stop(HardwarePwm &pin);
    };
}