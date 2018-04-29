#include <Libraries/RegisterManager.h>
#include <Utility.h>
#include <Libraries/Register.h>

namespace Treehopper {
    namespace Libraries {
        Register::Register(RegisterManager &regManager, int address, int width, bool isBigEndian)
                : regManager(regManager), isBigEndian(isBigEndian), address(address), width(width) {}

        void Register::write() {
            regManager.write(*this);
        }

        void Register::read() {
            regManager.read(*this);
        }

        std::vector<uint8_t> Register::getBytes() {
            std::vector<uint8_t> bytes(width);

            for (auto i = 0; i < width; i++)
                bytes[i] = (uint8_t) ((getValue() >> (8 * i)) & 0xFF);

            if (Utility::isBigEndian() ^ isBigEndian)
                std::reverse(std::begin(bytes), std::end(bytes));

            return bytes;
        }

        void Register::setBytes(std::vector<uint8_t> bytes) {
            if (Utility::isBigEndian() ^ isBigEndian)
                std::reverse(std::begin(bytes), std::end(bytes));

            long regVal = 0;

            for (auto i = 0; i < bytes.size(); i++)
                regVal |= bytes[i] << (i * 8);

            setValue(regVal);
        }
    }
}