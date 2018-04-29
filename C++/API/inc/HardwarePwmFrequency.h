#pragma once

#include "Treehopper.h"

namespace Treehopper {
    /** Frequencies supported by HardwarePwm */
    enum class TREEHOPPER_API HardwarePwmFrequency : uint8_t {
        /** 732 Hz frequency */
                Freq_732Hz,

        /** 183 Hz frequency */
                Freq_183Hz,

        /** 61 Hz frequency */
                Freq_61Hz
    };
}
