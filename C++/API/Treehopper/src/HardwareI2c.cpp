#include "stdafx.h"
#include "TreehopperUsb.h"
#include "HardwareI2c.h"
#include "Utility.h"
#include "I2cTransferException.h"
#include <math.h>

using namespace std;

namespace Treehopper {
	HardwareI2c::HardwareI2c(TreehopperUsb* board)
	{
		this->device = board;
		this->speed = 100;
	}


	HardwareI2c::~HardwareI2c()
	{

	}

	void HardwareI2c::setSpeed(double value)
	{
		this->speed = value;
		sendConfig();
	}

	double HardwareI2c::getSpeed()
	{
		return this->speed;
	}

	void HardwareI2c::setEnabled(bool value)
	{
		if (this->enabled == value) return;

		this->enabled = value;

		sendConfig();
	}

	bool HardwareI2c::getEnabled()
	{
		return false;
	}

	void HardwareI2c::sendReceive(uint8_t address, uint8_t * writeBuffer, size_t numBytesToWrite, 
		uint8_t * readBuffer, size_t numBytesToRead)
	{
		uint8_t* receivedData = new uint8_t[numBytesToRead+1];

		uint8_t* dataToSend = new uint8_t[4 + numBytesToWrite];
		dataToSend[0] = (uint8_t)DeviceCommands::I2cTransaction;
		dataToSend[1] = address;
		dataToSend[2] = numBytesToWrite; // total length (0-255)
		dataToSend[3] = numBytesToRead;

		if (numBytesToWrite > 0)
			copy(writeBuffer, writeBuffer + numBytesToWrite, dataToSend + 4);

		int offset = 0;
		int bytesRemaining = 4 + numBytesToWrite;

		while (bytesRemaining > 0)
		{
			int transferLength = bytesRemaining > 64 ? 64 : bytesRemaining;
			device->sendPeripheralConfigPacket(&dataToSend[offset], transferLength);
			offset += transferLength;
			bytesRemaining -= transferLength;
		}

		if (numBytesToRead == 0)
		{
			uint8_t responseCode;
			device->receivePeripheralConfigPacket(&responseCode, 1);
			if (responseCode != 255)
			{
				I2cTransferException ex((I2cTransferError)responseCode);
				Utility::error(ex);
			}
		}
		else
		{
			bytesRemaining = numBytesToRead + 1; // received data length + status byte
			int offset = 0;

			while (bytesRemaining > 0)
			{
				int numBytesToTransfer = bytesRemaining > 64 ? 64 : bytesRemaining;
				device->receivePeripheralConfigPacket(&receivedData[offset], numBytesToTransfer);
				offset += numBytesToTransfer;
				bytesRemaining -= numBytesToTransfer;
			}

			if (receivedData[0] != 255)
			{
				I2cTransferException ex((I2cTransferError)receivedData[0]);
				Utility::error(ex);
			}
			else
			{
				copy(receivedData + 1, receivedData + numBytesToRead + 1, readBuffer);
			}
		}

		delete[] receivedData;
		delete[] dataToSend;
	}

	void HardwareI2c::sendConfig()
	{
		double th0 = round(256.0 - (4000.0 / (3.0 * speed)));
		if (th0 < 0 || th0 > 255.0)
		{
			throw "Rate out of limits. Valid rate is 62.5 kHz - 16000 kHz (16 MHz)";
		}

		uint8_t dataToSend[3];
		dataToSend[0] = (uint8_t)DeviceCommands::I2cConfig;
		dataToSend[1] = enabled;
		dataToSend[2] = (uint8_t)th0;
		device->sendPeripheralConfigPacket(dataToSend, 3);
	}
}