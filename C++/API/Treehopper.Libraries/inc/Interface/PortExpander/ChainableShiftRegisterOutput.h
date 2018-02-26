#pragma once

#include "Treehopper.Libraries.h"
#include "Interface/Flushable.h"
#include "Spi.h"
#include "SpiDevice.h"
#include <vector>

namespace Treehopper {
    namespace Libraries {
        namespace Interface {
            namespace PortExpander {
                /// <summary>
                /// Any shift-register-like device that can be daisy-chained onto other shift registers.
                /// </summary>
                /// <remarks>
                /// <para>
                /// Note that this class doesn't expose a collection of <see cref="DigitalOutPin"/>s  or <see cref="Displays.Led"/>s, and instead, represents any writable shift register device. See <see cref="ShiftOut"/> for an implementation of a pin-based shift register.
                /// </para>
                /// </remarks>

                class LIBRARIES_API ChainableShiftRegisterOutput : Flushable {
                public:
                    /// <summary>
                    /// Set up a ChainableShiftRegisterOutput connected to an SPI port.
                    /// </summary>
                    /// <param name="spiModule">the SPI module to use</param>
                    /// <param name="latchPin">The latch pin to use</param>
                    /// <param name="numBytes">The number of bytes to write to this device</param>
                    /// <param name="mode">The SPI mode to use for all shift registers in this chain</param>
                    /// <param name="csMode">The ChipSelectMode to use for all shift registers in this chain</param>
                    /// <param name="speedMhz">The speed to use for all shift registers in this chain</param>
                    ChainableShiftRegisterOutput(Spi &spiModule, SpiChipSelectPin *latchPin, int numBytes = 1,
                                                 double speedMhz = 5, SpiMode mode = SpiMode::Mode00,
                                                 ChipSelectMode csMode = ChipSelectMode::PulseHighAtEnd);

                    /// <summary>
                    /// Set up a ChainableSHiftRegisterOutput connected to another ChainableShiftRegisterOutput device
                    /// </summary>
                    /// <param name="upstreamDevice">The upstream device this device is attached to</param>
                    /// <param name="numBytes">The number of bytes this device occupies in the chain</param>
                    ChainableShiftRegisterOutput(ChainableShiftRegisterOutput &upstreamDevice, int numBytes = 1);

                    /// <summary>
                    /// Immediately update all the pins at once with the given value. flush() will be implicity called.
                    /// </summary>
                    /// <param name="value">A value representing the data to write to the port</param>
                    /// <returns>An awaitable task that completes upon successfully writing the value.</returns>
                    void write(uint8_t *value);

                    /// <summary>
                    /// flush data to the port
                    /// </summary>
                    /// <param name="force">Whether to flush all data to the port, even if it doesn't appear to have changed.</param>
                    /// <returns>An awaitable task that completes when finished</returns>
                    void flush(bool force = false);


                    /** The current value of the port.
                    Note that this value is numBytes wide.
                    */
                    uint8_t *currentValue;

                    /** The number of bytes wide this port is */
                    int numBytes;
                protected:
                    /// <summary>
                    /// Classes extending this class should call this function after the internal pin data structure is updated.
                    /// </summary>
                    void flushIfAutoFlushEnabled();

                    /// <summary>
                    /// Update internal data structures from current value
                    /// </summary>
                    virtual void updateFromCurrentValue() = 0;

                    /// <summary>
                    /// called to request a write to the device in chain (subsequently updating all devices in the chain)
                    /// </summary>
                    /// <returns></returns>
                    static void requestWrite();

                private:
                    void setupSpi(Spi &spiModule, SpiChipSelectPin *latchPin, double speedMhz, SpiMode mode,
                                  ChipSelectMode csMode);

                    uint8_t *lastValues;
                    static SpiDevice *spiDevice;
                    static vector<ChainableShiftRegisterOutput *> shiftRegisters;
                };
            }
        }
    }
}