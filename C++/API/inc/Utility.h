#pragma once

#include <string>

#include "Treehopper.h"
#include <stdexcept>
#include <vector>

namespace Treehopper {
    class TREEHOPPER_API Utility {
    public:
        static void error(std::runtime_error &message, bool fatal = false);

        static void error(std::string message, bool fatal = false) {
            auto err = std::runtime_error(message);
            error(err, fatal);
        }

        static bool closeTo(double a, double b, double error = 0.001);

        static bool isBigEndian();

        static int nthOccurrence(const std::wstring &str, const std::wstring &findMe, int nth);

        static std::vector<uint8_t> getBytes(uint64_t value, int width, bool isBigEndian = false);

        static uint64_t getValue(std::vector<uint8_t> bytes, bool isBigEndian = false);
    };
}
