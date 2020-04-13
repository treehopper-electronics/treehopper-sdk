#include <algorithm>
#include "Libraries/RegisterManager.h"
#include "Libraries/Register.h"

namespace Treehopper {
    namespace Libraries {
        RegisterManager::RegisterManager(SMBusDevice &dev, bool multiRegisterAccess)
			: dev(dev), 
			multiRegisterAccess(multiRegisterAccess)
		{

        }

        void RegisterManager::write(Register &reg) {
            dev.writeBufferData((uint8_t) reg.address, reg.getBytes());
        }

        void RegisterManager::writeRange(Register &start, Register &end) {
            if (multiRegisterAccess) {
                auto count = (end.address + end.width) - start.address;
                std::vector<uint8_t> bytes;

                for (auto &reg : registers) {
                    if (reg->address < start.address)
                        continue;

                    if (reg->address > end.address)
                        break;

                    auto data = reg->getBytes();
                    bytes.insert(bytes.end(), data.begin(), data.end());
                }

                dev.writeBufferData((uint8_t) start.address, bytes);
            } else {
                for (auto &reg : registers) {
                    if (reg->address < start.address)
                        continue;

                    if (reg->address > end.address)
                        break;

                    write(*reg);
                }
            }
        }

        void RegisterManager::read(Register &reg) {
            auto data = dev.readBufferData((uint8_t) reg.address, (size_t) reg.width);
            reg.setBytes(data);
        }

        void RegisterManager::readRange(Register &start, Register &end) {
            if (multiRegisterAccess) {
                auto count = (end.address + end.width) - start.address;
                auto bytes = dev.readBufferData((uint8_t) start.address, (size_t) count);
                int i = 0;

                for (auto &reg : registers) {
                    if (reg->address < start.address)
                        continue;

                    if (reg->address > end.address)
                        break;

                    std::vector<uint8_t> slice(bytes.begin() + i, bytes.begin() + i + reg->width);
                    reg->setBytes(slice);
                    i += reg->width;
                }
            } else {
                for (auto &reg : registers) {
                    if (reg->address < start.address)
                        continue;

                    if (reg->address > end.address)
                        break;

                    read(*reg);
                }
            }
        }
    }
}