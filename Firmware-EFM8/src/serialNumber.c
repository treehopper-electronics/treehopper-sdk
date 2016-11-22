/*
 * serialNumber.c
 *
 *  Created on: Dec 1, 2015
 *      Author: jay
 */

#include "efm8_usb.h"
#include "flash.h"
#include "serialNumber.h"
#include "adc.h"
#include "gpio.h"

SI_LOCATED_VARIABLE_NO_INIT( serialNumber_serial[64], USB_StringDescriptor_TypeDef,
		SI_SEG_CODE, SER_ADDR);
SI_LOCATED_VARIABLE_NO_INIT( serialNumber_name[64], USB_StringDescriptor_TypeDef,
		SI_SEG_CODE, NAME_ADDR);

void writeUsbString(uint8_t* string, uint8_t len, uint16_t addr);

uint8_t serialString[8];

uint8_t getRandomPrintableCharacter()
{
	uint8_t i;
	uint16_t ch = 0;
	for(i=0;i<8;i++)
	{
		ch ^= ADC_GetVal(i);
	}

	// scale random character so it is between 48 to 57, 65 to 90, and 97 to 122 (62 total)

	// first, compress it so it is between 0 and 63;
	ch = ch & 0x3F;
	// clip the top so it is between 0 and 61 (62 total values)
	if(ch > 61)
		ch = 61;
	if(ch < 10)
		return(ch + 48); // 0-9
	if(ch < 36)
		return(ch + 55); // A-Z
	return(ch + 61);
}

void generateRandomString()
{
	uint8_t i = 0;
	uint8_t randomChar = 0;

	// use the ADC as a random seed
	for(i=0;i<8;i++)
	{
		ADC_Enable(i, VREF_3V3);
	}

	for(i=0;i<8;i++)
	{
		serialString[i] = getRandomPrintableCharacter();
	}

	for(i=0;i<8;i++)
	{
		GPIO_MakeInput(i, true);
	}
}



void SerialNumber_Init() {
	if (serialNumber_serial[0] == 0xFF) // blank, program with default
	{
		generateRandomString();
		SerialNumber_update(serialString, 8);
	}

	if (serialNumber_name[0] == 0xFF) {
		SerialNumber_updateName("Treehopper", 10);
	}
}

void SerialNumber_update(uint8_t* string, uint8_t len) {
	writeUsbString(string, len, SER_ADDR);
}

void SerialNumber_updateName(uint8_t* string, uint8_t len) {
	writeUsbString(string, len, NAME_ADDR);
}

void writeUsbString(uint8_t* string, uint8_t len, uint16_t addr) {
	int i;
	flash_erasePage(addr);
	flash_writeByte(addr, USB_STRING_DESCRIPTOR_UTF16LE_PACKED);
	flash_writeByte(addr + 1, (len + 1) * 2);
	flash_writeByte(addr + 2, USB_STRING_DESCRIPTOR);
	for (i = 0; i < len; i++) {
		flash_writeByte(addr + 3 + i, string[i]);
	}
}
