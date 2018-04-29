#pragma once

#include <stdexcept>
#include <string>
#include "Treehopper.h"
#include "I2cTransferError.h"

using namespace std;

namespace Treehopper {
    class I2cTransferException : public std::runtime_error {
    public:
        I2cTransferException(I2cTransferError responseCode) : runtime_error(GetError(responseCode)) {

        }

        static string GetError(I2cTransferError code) {
            switch (code) {
                case I2cTransferError::ArbitrationLostError:
                    return string("I2c arbitration lost error");
                    break;

                case I2cTransferError::NackError:
                    return string("I2c nack error");
                    break;

                case I2cTransferError::Success:
                    return string("I2c success");
                    break;

                case I2cTransferError::TxunderError:
                    return string("I2c Tx underrun error");
                    break;

                default:
                case I2cTransferError::UnknownError:
                    return string("I2c unknown error");
            }
        }
    };
}
