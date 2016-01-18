/*
 * serialNumber.c
 *
 *  Created on: Dec 1, 2015
 *      Author: jay
 */

#include "efm8_usb.h"
#include "flash.h"
#include "serialNumber.h"

SI_LOCATED_VARIABLE_NO_INIT(serial[64], USB_StringDescriptor_TypeDef, SI_SEG_CODE, SER_ADDR);
SI_LOCATED_VARIABLE_NO_INIT(name[64], USB_StringDescriptor_TypeDef, SI_SEG_CODE, NAME_ADDR);

void writeString(uint8_t* string, uint8_t len, uint16_t addr);

void serialNumber_init()
{
	if(serial[0] == 0xFF) // blank, program with default
	{
		serialNumber_update("00000000", 8);
	}

	if(name[0] == 0xFF)
	{
		serialNumber_updateName("MyTreehopper", 12);
	}
}

void serialNumber_update(uint8_t* string, uint8_t len)
{
	writeString(string, len, SER_ADDR);
}

void serialNumber_updateName(uint8_t* string, uint8_t len)
{
	writeString(string, len, NAME_ADDR);
}

void writeString(uint8_t* string, uint8_t len, uint16_t addr)
{
	int i;
	flash_erasePage(addr);
	flash_writeByte(addr, USB_STRING_DESCRIPTOR_UTF16LE_PACKED);
	flash_writeByte(addr+1, (len+1) * 2 );
	flash_writeByte(addr+2, USB_STRING_DESCRIPTOR);
	for(i=0;i<len;i++)
	{
		flash_writeByte(addr+3+i, string[i]);
	}
}
