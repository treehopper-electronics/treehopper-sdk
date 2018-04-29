#pragma once

#include "Treehopper.h"

namespace Treehopper {
    /** The SPI burst mode to use */
    enum class SpiBurstMode {
        /** No burst -- always read the same number of bytes as transmitted */
                NoBurst,

        /** Transmit burst -- don't return any data read from the bus */
                BurstTx,

        /** Receive burst -- ignore transmitted data above 53 bytes long, but receive the full number of bytes specified */
                BurstRx
    };
}