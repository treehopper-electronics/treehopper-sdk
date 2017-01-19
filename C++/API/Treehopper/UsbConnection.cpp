#include "stdafx.h"
#include "UsbConnection.h"

wstring UsbConnection::getSerialNumber()
{
	return serialNumber;
}

wstring UsbConnection::getDevicePath()
{
	return devicePath;
}

wstring UsbConnection::getName()
{
	return name;
}

