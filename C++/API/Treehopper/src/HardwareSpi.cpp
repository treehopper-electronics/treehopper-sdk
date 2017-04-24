#include "HardwareSpi.h"
#include "TreehopperUsb.h"
#include "SpiChipSelectPin.h"
#include <cmath>

namespace Treehopper {
	HardwareSpi::HardwareSpi(TreehopperUsb& board) : board(board)
	{

	}

	bool HardwareSpi::enabled()
	{
		return _enabled;
	}
	void HardwareSpi::enabled(bool value)
	{
		if (value == _enabled) return;

		_enabled = value;

		uint8_t dataToSend[2];
		dataToSend[0] = (uint8_t)TreehopperUsb::DeviceCommands::SpiConfig;
		dataToSend[1] = (uint8_t)(_enabled ? 0x01 : 0x00);
		board.sendPeripheralConfigPacket(dataToSend, sizeof(dataToSend));

		if (_enabled) {
			board.pins[0].mode(PinMode::Reserved);
			board.pins[1].mode(PinMode::Reserved);
			board.pins[2].mode(PinMode::Reserved);
		} else {
			board.pins[0].mode(PinMode::Unassigned);
			board.pins[1].mode(PinMode::Unassigned);
			board.pins[2].mode(PinMode::Unassigned);
		}
	}

	void HardwareSpi::sendReceive(uint8_t * dataToWrite, int numBytesToTransfer, uint8_t * readBuffer, SpiChipSelectPin* chipSelect, ChipSelectMode chipSelectMode, double speed, SpiBurstMode burstMode, SpiMode spiMode)
	{
		if (_enabled != true)
		{
			Utility::error("SPI module must be enabled before starting transaction", true);
		}

		if ((chipSelect != NULL) && (chipSelect->spiModule != (Spi*)this))
		{
			Utility::error("Chip select pin must belong to this SPI module", true);
		}

		int spi0ckr = (int)round((24.0 / speed) - 1);
		if (spi0ckr > 255.0)
		{
			spi0ckr = 255;
			Utility::error("NOTICE: Requested SPI frequency is below the minimum frequency, and will be clipped to 0.09375 MHz (93.75 kHz).");
		}
		else if (spi0ckr < 0)
		{
			spi0ckr = 0;
			Utility::error("NOTICE: Requested SPI frequency is above the maximum frequency, and will be clipped to 24 MHz.");
		}

		double actualFrequency = 48.0 / (2.0 * (spi0ckr + 1.0));

		if (abs(actualFrequency - speed) > 1)
			Utility::error("NOTICE: SPI module actual frequency is more than 1 MHz away from the requested frequency of {1} MHz");

		if (numBytesToTransfer > 255)
		{
			Utility::error("Maximum packet length is 255 bytes");
			numBytesToTransfer = 255; // clip
		}

		if (readBuffer == NULL && burstMode != SpiBurstMode::BurstTx)
		{
			Utility::error("Read buffer was NULL; forcing burstMode to BurstTx");
			burstMode = SpiBurstMode::BurstTx;
		}

		uint8_t* dataToSend = new uint8_t[7 + numBytesToTransfer];

		dataToSend[0] = (uint8_t)TreehopperUsb::DeviceCommands::SpiTransaction;

		if (chipSelect != NULL)
			dataToSend[1] = chipSelect->pinNumber;
		else
			dataToSend[1] = 255;

		dataToSend[2] = (uint8_t)chipSelectMode;
		dataToSend[3] = (uint8_t)spi0ckr;
		dataToSend[4] = (uint8_t)spiMode;
		dataToSend[5] = (uint8_t)burstMode;
		dataToSend[6] = (uint8_t)numBytesToTransfer;

		// just send the header
		if (burstMode == SpiBurstMode::BurstRx)
		{
			board.sendPeripheralConfigPacket(dataToSend, 7);
		}
		else
		{
			copy(dataToWrite, dataToWrite + numBytesToTransfer, dataToSend + 7);

			int bytesRemaining = numBytesToTransfer + 7;
			int offset = 0;
			while (bytesRemaining > 0)
			{
				int transferLength = bytesRemaining > 64 ? 64 : bytesRemaining;
				board.sendPeripheralConfigPacket(dataToSend + offset, transferLength);
				offset += transferLength;
				bytesRemaining -= transferLength;
			}
		}

		// no need to wait if we're not reading anything
		if (burstMode != SpiBurstMode::BurstTx)
		{
			int bytesRemaining = numBytesToTransfer;
			int offset = 0;
			while (bytesRemaining > 0)
			{
				int numBytesToTransfer = bytesRemaining > 64 ? 64 : bytesRemaining;
				board.receivePeripheralConfigPacket(readBuffer + offset, numBytesToTransfer);
				offset += numBytesToTransfer;
				bytesRemaining -= numBytesToTransfer;
			}
		}

		delete[] dataToSend;
	}
}