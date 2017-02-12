#pragma once
#include <string>
#include "Treehopper.h"

using namespace std;

namespace Treehopper 
{
	class TREEHOPPER_API UsbConnection
	{
	public:
		virtual ~UsbConnection()
		{

		}
		virtual bool open() = 0;
		virtual void close() = 0;
		virtual wstring getSerialNumber() = 0;
		virtual wstring getName() = 0;
		virtual wstring getDevicePath() = 0;
		virtual void sendDataPinConfigChannel(uint8_t* data, size_t len) = 0;
		virtual void sendDataPeripheralChannel(uint8_t* data, size_t len) = 0;
		virtual bool receiveDataPeripheralChannel(uint8_t* data, size_t len) = 0;
		virtual bool receivePinReportPacket(uint8_t* data) = 0;
		//virtual unique_ptr<

	protected:
		wstring serialNumber;
		wstring name;
		wstring devicePath;
		uint8_t pinReportEndpoint = 0x81;
		uint8_t peripheralResponseEndpoint = 0x82;
		uint8_t pinConfigEndpoint = 0x01;
		uint8_t peripheralConfigEndpoint = 0x02;
	};
}