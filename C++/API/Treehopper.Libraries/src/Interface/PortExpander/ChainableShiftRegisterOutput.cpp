#include <cstring>
#include "Interface/PortExpander/ChainableShiftRegisterOutput.h"

namespace Treehopper {
	namespace Libraries {
		namespace Interface {
			namespace PortExpander {
				ChainableShiftRegisterOutput::ChainableShiftRegisterOutput(Spi& spiModule, SpiChipSelectPin* latchPin, int numBytes, double speedMhz, SpiMode mode, ChipSelectMode csMode)
				{
					ChainableShiftRegisterOutput::setupSpi(spiModule, latchPin, speedMhz, mode, csMode);
					shiftRegisters.push_back(this);
					this->numBytes = numBytes;
					currentValue = new uint8_t[numBytes]();
					lastValues = new uint8_t[numBytes]();
				}

				ChainableShiftRegisterOutput::ChainableShiftRegisterOutput(ChainableShiftRegisterOutput& upstreamDevice, int numBytes)
				{
					if (upstreamDevice.parent != NULL)
						parent = upstreamDevice.parent;
					else
						parent = &upstreamDevice;

					shiftRegisters.push_back(this);
					this->numBytes = numBytes;
					currentValue = new uint8_t[numBytes]();
					lastValues = new uint8_t[numBytes]();
				}

				void ChainableShiftRegisterOutput::write(uint8_t* value)
				{
					bool savedAutoFlushValue = autoFlush;
					autoFlush = false;
					copy(value, value + numBytes, currentValue);
					updateFromCurrentValue(); // tell our parent to update its pins, LEDs, or whatever.

					flush(true);
					autoFlush = savedAutoFlushValue;
				}

				void ChainableShiftRegisterOutput::flush(bool force)
				{
					if ((memcmp(currentValue, lastValues, numBytes) != 0) || force)
					{
						ChainableShiftRegisterOutput::requestWrite();
						copy(currentValue, currentValue + numBytes, lastValues);
					}
				}

				void ChainableShiftRegisterOutput::flushIfAutoFlushEnabled()
				{
					if (autoFlush)
						flush();
				}

				void ChainableShiftRegisterOutput::requestWrite()
				{
					// build the byte array to flush out of the port
					vector<uint8_t> bytes;
					for (auto it = shiftRegisters.begin(); it != shiftRegisters.end(); ++it)
					{
						for (int i = 0; i < (*it)->numBytes; i++)
							bytes.push_back((*it)->currentValue[i]);
					}
					
					// all these bytes are backwards, so flip them
					reverse(bytes.begin(), bytes.end());
					spiDevice->sendReceive(&bytes[0], bytes.size(), NULL, SpiBurstMode::BurstTx);

				}

				void ChainableShiftRegisterOutput::setupSpi(Spi & spiModule, SpiChipSelectPin* latchPin, double speedMhz, SpiMode mode, ChipSelectMode csMode)
				{
					ChainableShiftRegisterOutput::spiDevice = new SpiDevice(spiModule, latchPin, csMode, speedMhz, mode);
				}

				SpiDevice* ChainableShiftRegisterOutput::spiDevice;
				vector<ChainableShiftRegisterOutput*> ChainableShiftRegisterOutput::shiftRegisters;
			} 
		} 
	} 
}

