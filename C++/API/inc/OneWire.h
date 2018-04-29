#pragma once

#include <vector>
#include <cstdint>

namespace Treehopper {
    class OneWire {
        virtual void startOneWire() = 0;

        virtual void oneWireResetAndMatchAddress(uint64_t address) = 0;

        virtual std::vector<uint64_t> oneWireSearch() = 0;

        /// <summary>
        ///     Reset the One Wire bus
        /// </summary>
        /// <returns>True if at least one device was found. False otherwise.</returns>
        virtual bool oneWireReset() = 0;

        /// <summary>
        ///     Receive bytes from the UART in One-Wire mode
        /// </summary>
        /// <param name="numBytes">The number of bytes to receive.</param>
        /// <returns>The bytes received</returns>
        virtual std::vector<uint8_t> receive(int numBytes) = 0;

        /// <summary>
        ///     Send data
        /// </summary>
        /// <param name="dataToSend">The data to send</param>
        /// <returns>An awaitable task that completes upon transmission of the data</returns>
        virtual void send(std::vector<uint8_t> dataToSend) = 0;

        /// <summary>
        ///     Send a byte out of the UART
        /// </summary>
        /// <param name="data">The byte to send</param>
        /// <returns>An awaitable task that completes upon transmission of the byte</returns>
        virtual void send(uint8_t dataToSend) = 0;
    };
}

