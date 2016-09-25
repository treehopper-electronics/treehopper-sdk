/*
 * led.c
 *
 *  Created on: Dec 1, 2015
 *      Author: jay
 */
#include <SI_EFM8UB1_Register_Enums.h>

void LED_Init() {
	SFRPAGE = 0x20;

	P3MDIN |= 0x2;
	P3MDOUT |= 0x2;
	P3_B1 = 0;
}

void LED_SetVal(uint8_t val) {
	P3_B1 = val;
}
