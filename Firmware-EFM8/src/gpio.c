/*
 * gpio.c
 *
 *  Created on: Jul 19, 2015
 *      Author: jay
 */
#include "gpio.h"
#include "treehopper.h"
#include <SI_EFM8UB1_Register_Enums.h>
SI_SEGMENT_VARIABLE(portBitNumber[], static const uint8_t, SI_SEG_CODE) = {
	0, // pin0
	1,
	2,
	3,
	6,
	4,
	5,
	7,// pin7
	0,// pin8
	1,
	2,
	3,
	4,
	5,
	6,
	7,// pin15

	// < Rev A2
	3,// pin16
	2,
	1,
	0,

	// > Rev A2
	// 0, // pin16
	// 1,
	// 2,
	// 3
};

void GPIO_MakeSpecialFunction(uint8_t pinNumber, uint8_t pushPull) {
	uint8_t portBit = portBitNumber[pinNumber];
	uint8_t SFRPAGE_save = SFRPAGE;
	SFRPAGE = 0;
	if (pinNumber < 8) {
		P0SKIP &= ~(1 << portBit);
		if (pushPull)
			P0MDOUT |= (1 << portBit);
		else
			P0MDOUT &= ~(1 << portBit);
	} else if (pinNumber < 16) {
		P1SKIP &= ~(1 << portBit);
		if (pushPull)
			P1MDOUT |= (1 << portBit);
		else
			P1MDOUT &= ~(1 << portBit);
	} else if(pinNumber < 20) {
		P2SKIP &= ~(1 << portBit);
		if (pushPull)
			P2MDOUT |= (1 << portBit);
		else
			P2MDOUT &= ~(1 << portBit);
	}
	SFRPAGE = SFRPAGE_save;
}

void GPIO_MakeInput(uint8_t pinNumber, uint8_t digital) {
	uint8_t portBit = portBitNumber[pinNumber];
	uint8_t SFRPAGE_save = SFRPAGE;
	SFRPAGE = 0;
	if (pinNumber < 8) {

		P0SKIP |= 1 << portBit;
		if (digital) {
			P0MDIN |= 1 << portBit;
			P0 |= 1 << portBit;
		} else {
			P0MDIN &= ~(1 << portBit);
		}
		P0MDOUT &= ~(1 << portBit);
	} else if (pinNumber < 16) {
		P1SKIP |= 1 << portBit;
		if (digital) {
			P1MDIN |= 1 << portBit;
			P1 |= 1 << portBit;
		} else {
			P1MDIN &= ~(1 << portBit);
		}
		P1MDOUT &= ~(1 << portBit);
	} else if(pinNumber < 20){
		SFRPAGE = 0x20;
		P2SKIP |= 1 << portBit;
		if (digital) {
			P2MDIN |= 1 << portBit;
			P2 |= 1 << portBit;
		} else {
			P2MDIN &= ~(1 << portBit);
		}
		P2MDOUT &= ~(1 << portBit);
	}
	SFRPAGE = SFRPAGE_save;
}

void GPIO_MakeOutput(uint8_t pinNumber, uint8_t OutputType) {
	uint8_t portBit = portBitNumber[pinNumber];
	GPIO_WriteValue(pinNumber, false);
	SFRPAGE = 0;
	if (pinNumber < 8) {
		P0SKIP |= 1 << portBit;
		P0MDIN |= 1 << portBit;
		if (OutputType == PushPullOutput)
			P0MDOUT |= 1 << portBit;
		else
			P0MDOUT &= ~(1 << portBit);
	} else if (pinNumber < 16) {
		P1SKIP |= 1 << portBit;
		P1MDIN |= 1 << portBit;
		if (OutputType == PushPullOutput)
			P1MDOUT |= 1 << portBit;
		else
			P1MDOUT &= ~(1 << portBit);
	} else if(pinNumber < 20) {
		P2SKIP |= 1 << portBit;
		P2MDIN |= 1 << portBit;
		if (OutputType == PushPullOutput)
			P2MDOUT |= 1 << portBit;
		else
			P2MDOUT &= ~(1 << portBit);
	}
}

void GPIO_WriteValue(uint8_t pinNumber, bool val) {
	// this only executes in 14 instructions, and this time doesn't change based on which pin is written to
	switch (pinNumber) {
	case 0:
		PIN0 = val;
		break;
	case 1:
		PIN1 = val;
		break;
	case 2:
		PIN2 = val;
		break;
	case 3:
		PIN3 = val;
		break;
	case 4:
		PIN4 = val;
		break;
	case 5:
		PIN5 = val;
		break;
	case 6:
		PIN6 = val;
		break;
	case 7:
		PIN7 = val;
		break;
	case 8:
		PIN8 = val;
		break;
	case 9:
		PIN9 = val;
		break;
	case 10:
		PIN10 = val;
		break;
	case 11:
		PIN11 = val;
		break;
	case 12:
		PIN12 = val;
		break;
	case 13:
		PIN13 = val;
		break;
	case 14:
		PIN14 = val;
		break;
	case 15:
		PIN15 = val;
		break;
	case 16:
		PIN16 = val;
		break;
	case 17:
		PIN17 = val;
		break;
	case 18:
		PIN18 = val;
		break;
	case 19:
		PIN19 = val;
		break;
	}
//	uint8_t portBit = portBitNumber[pinNumber];
//	if(pinNumber < 9)
//	{
//		if(val)
//			P0 |= 1 << portBit;
//		else
//			P0 &= ~(1 << portBit);
//	}
//	else if(pinNumber < 17)
//	{
//		if(val)
//			P1 |= 1 << portBit;
//		else
//			P1 &= ~(1 << portBit);
//	}
//	else
//	{
//		if(val)
//			P2 |= 1 << portBit;
//		else
//			P2 &= ~(1 << portBit);
//	}
}

bool GPIO_ReadValue(uint8_t pinNumber) {
	uint8_t portBit = portBitNumber[pinNumber];
	if (pinNumber < 9)
		return P0 & (1 << portBit);
	else if (pinNumber < 17)
		return P1 & (1 << portBit);
	else
		return P2 & (1 << portBit);
}
