#pragma once

#include "Libraries/Treehopper.Libraries.h"

namespace Treehopper {
    namespace Libraries {
        namespace Interface {
            /** Represents any object that has a flushable interface */
            class LIBRARIES_API Flushable {
            public:
                ~Flushable() {}

                /** Whether this interface should automatically flush new values or not */
                bool autoFlush;

                /** flush changed data to the port expander
                @param[in] force	whether to flush *all* data to the port expander, even if it doesn't appear to have been changed
                */
                virtual void flush(bool force = false) = 0;

                /** Gets or sets the parent flushable device (if it exists); if this property is set by this driver, it is expected that flushing the parent will also flush this device.

                This property is designed to make LED displays, which operate across groups of LEDs (and possibly groups of LED drivers), much more efficient to update. Many commonly-used LED drivers are shift registers that are chained together; since these cannot be individually addressed, any write to one must include a write to all the other ones. By properly setting the parent shift register in each chain, displays can optimize these updates.
                */
                Flushable *parent;
            };
        }
    }
}

