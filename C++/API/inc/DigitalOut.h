#pragma once

#include "Treehopper.h"
#include "Event.h"

namespace Treehopper {
    /** Digital output pin abstract class.
    This abstract class provides digital output support used by Pin, and can also be extended by GPIO expanders and other peripherals that provide DigitalOut -like functionality.
    */
    class TREEHOPPER_API DigitalOut {
    public:
        /** Make the pin a push-pull output */
        virtual void makePushPullOutput() = 0;

        /** Retrieve the currently-set digital value of the pin */
        virtual bool digitalValue() {
            return _digitalValue;
        }

        /** Set the pin's digital value */
        virtual void digitalValue(bool value) {
            if (_digitalValue == value) return;

            _digitalValue = value;
            writeOutputValue();
        }

    protected:
        bool _digitalValue;

        virtual void writeOutputValue() = 0;
    };
}