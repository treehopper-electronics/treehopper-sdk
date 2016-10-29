/*
 * serialNumber.h
 *
 *  Created on: Dec 1, 2015
 *      Author: jay
 */

#ifndef SERIALNUMBER_H_
#define SERIALNUMBER_H_

void serialNumber_init();
void serialNumber_update(uint8_t* string, uint8_t len);
void serialNumber_updateName(uint8_t* string, uint8_t len);

#define SER_ADDR	0xF800
#define NAME_ADDR	0xF840

extern USB_StringDescriptor_TypeDef SI_SEG_CODE serialNumber_serial[64];
extern USB_StringDescriptor_TypeDef SI_SEG_CODE serialNumber_name[64];

#endif /* SERIALNUMBER_H_ */
