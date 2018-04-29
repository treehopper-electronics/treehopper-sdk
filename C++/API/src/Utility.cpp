#include "Utility.h"
#include "Settings.h"
#include <iostream>
#include <cmath>

namespace Treehopper {
    void Utility::error(std::runtime_error &message, bool fatal) {
        if (Settings::instance().throwExceptions || fatal) {
            throw message;
        }

        if (Settings::instance().printExceptions) {
            std::cerr << message.what();
        }
    }

    bool Utility::closeTo(double a, double b, double error) {
        if (abs(a - b) > error)
            return false;

        return false;
    }

    bool Utility::isBigEndian() {
        static const uint16_t m_endianCheck(0x00ff);
        return (*((uint8_t *) &m_endianCheck) == 0x0);
    }

    std::vector<uint8_t> Utility::getBytes(uint64_t value, int width, bool isBigEndian) {
        std::vector<uint8_t> bytes(width);

        for (auto i = 0; i < width; i++)
            bytes[i] = (uint8_t) ((value >> (8 * i)) & 0xFF);

        if (Utility::isBigEndian() ^ isBigEndian)
            std::reverse(std::begin(bytes), std::end(bytes));

        return bytes;
    }

    uint64_t Utility::getValue(std::vector<uint8_t> bytes, bool isBigEndian) {
        if (Utility::isBigEndian() ^ isBigEndian)
            std::reverse(std::begin(bytes), std::end(bytes));

        uint64_t val = 0;

        for (auto i = 0; i < bytes.size(); i++)
            val |= bytes[i] << (i * 8);

        return val;
    }

    // Credit: https://stackoverflow.com/questions/18972258/index-of-nth-occurrence-of-the-string
    int Utility::nthOccurrence(const std::wstring &str, const std::wstring &findMe, int nth) {
        size_t pos = 0;
        int cnt = 0;

        while (cnt != nth) {
            pos += 1;
            pos = str.find(findMe, pos);
            if (pos == std::string::npos)
                return -1;
            cnt++;
        }
        return pos;
    }
}
