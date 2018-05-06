#pragma once

#include "Treehopper.h"
#include "Utility.h"

namespace Treehopper {
    class TREEHOPPER_API Pwm {
    public:
        virtual ~Pwm() {};

        /** Gets the duty cycle */
        virtual double dutyCycle() { return _dutyCycle; }

        /** Sets the duty cycle */
        virtual void dutyCycle(double value) {
            if (Utility::closeTo(_dutyCycle, value))
                return;

            _dutyCycle = value;
            updateDutyCycle();
        }

        virtual bool enabled() = 0;

        virtual void enabled(bool value) = 0;

        /** Gets the pulse width */
        virtual double pulseWidth() {
            // always calculate from frequency and dc
            return _dutyCycle * 1000000.0 / _frequency;
        }

        /** Sets the pulse width */
        virtual void pulseWidth(double value) {
            if (Utility::closeTo(pulseWidth(), value))
                return;

            _dutyCycle = value * _frequency / 1000000.0;
            updateDutyCycle();
        }

    protected:
        virtual void updateDutyCycle() = 0;

        double _dutyCycle;
        double _frequency;
    };
}