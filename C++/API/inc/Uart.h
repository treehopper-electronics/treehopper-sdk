#pragma once

#include <cstdint>
#include "Treehopper.h"
#include <vector>

namespace Treehopper {
    class Uart {
    public:
        /// <summary>
        /// Start the UART with the specified baud
        /// </summary>
        /// <param name="baud">The baud, in bps, to use</param>
        virtual void startUart() = 0;

        /// <summary>
        ///     Send data
        /// </summary>
        /// <param name="dataToSend">The data to send</param>
        /// <returns>An awaitable task that completes upon transmission of the data</returns>
        virtual void send(std::vector<uint8_t> dataToSend) = 0;

        /// <summary>
        ///     Receive bytes from the UART in UART mode
        /// </summary>
        /// <returns>The bytes received</returns>
        /// As soon as the UART is enabled, any received byte will be added to a 32-byte buffer. Calling this Receive() function does two things:
        ///    - sends the current contents of this buffer to this function.
        ///    - reset the pointer in the buffer to the 0th element, effectively resetting it.
        /// If the buffer fills before the Receive() function is called, the existing buffer will be reset --- discarding all data in the buffer.
        /// Consequently, it's important to call the Receive() function frequently when expecting data.
        ///
        /// Owing to how it is implemented, you can clear the buffer at any point by calling Receive(). It's common to empty the buffer before
        /// requesting data from the device attached to the UART; this way, you do not have to worry about existing gibberish data that
        /// might have been inadvertently received.
        virtual std::vector<uint8_t> receive() = 0;

        /// <summary>
        ///     Sets the baud of the UART.
        /// </summary>
        ///
        /// Baud can range from 7813 - 2400000 baud, but values less than 2000000 (2 Mbaud) are recommended.
        virtual void baud(int baud) = 0;

        /// <summary>
        ///     Gets the baud of the UART.
        /// </summary>
        virtual int baud() = 0;
    };
}
