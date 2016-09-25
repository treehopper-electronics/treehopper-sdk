#include "pwm.h"
#include <SI_EFM8UB1_Register_Enums.h>
#include "gpio.h"
/*
 * pwm.c
 *
 *  Created on: Jul 25, 2015
 *      Author: jay
 */

enum {
	PwmMode_None, PwmMode_Pin7, PwmMode_Pin7_Pin8, PwmMode_Pin7_Pin8_Pin9
};

uint8_t PWM_1L = 0;
uint8_t PWM_1H = 0;

uint8_t PWM_2L = 0;
uint8_t PWM_2H = 0;

uint8_t PWM_3L = 0;
uint8_t PWM_3H = 0;

uint8_t oldMode = 0;
uint8_t oldFreq = 0;
void PWM_Init() {

}

void PWM_SetConfig(uint8_t* configuration) {
	uint8_t SFRPAGE_save = SFRPAGE;

	uint8_t mode = configuration[0];
	uint8_t freq = configuration[1];

	SFRPAGE = 0x00;

	if (freq != oldFreq)

		if (freq != oldFreq) {
			PCA0MD &= ~(PCA0MD_CPS__SYSCLK | PCA0MD_CPS__SYSCLK_DIV_4
					| PCA0MD_CPS__SYSCLK_DIV_12);

			switch (freq) {
			case 0:
				PCA0MD |= PCA0MD_CPS__SYSCLK;
				break;

			case 1:
				PCA0MD |= PCA0MD_CPS__SYSCLK_DIV_4;
				break;

			case 2:
				PCA0MD |= PCA0MD_CPS__SYSCLK_DIV_12;
				break;
			}
		}

	PWM_1L = configuration[2];
	PWM_1H = configuration[3];

	PWM_2L = configuration[4];
	PWM_2H = configuration[5];

	PWM_3L = configuration[6];
	PWM_3H = configuration[7];

	if (PWM_1L == 0 && PWM_1H == 0) {
		PCA0CPL0 = 0;
		PCA0CPH0 = 0;
	}

	if (PWM_2L == 0 && PWM_2H == 0) {
		PCA0CPL1 = 0;
		PCA0CPH1 = 0;
	}

	if (PWM_3L == 0 && PWM_3H == 0) {
		PCA0CPL2 = 0;
		PCA0CPH2 = 0;
	}

	if (oldMode != mode) {
		oldMode = mode;
		switch (mode) {
		case PwmMode_None:
			PWM_Disable();
			break;
		case PwmMode_Pin7: // PWM1
			XBR1 |= XBR1_PCA0ME__CEX0;
			GPIO_MakeSpecialFunction(7, true);
			break;
		case PwmMode_Pin7_Pin8: // PWM1, PWM2
			XBR1 |= XBR1_PCA0ME__CEX0_CEX1;
			GPIO_MakeSpecialFunction(7, true);
			GPIO_MakeSpecialFunction(8, true);
			break;
		case PwmMode_Pin7_Pin8_Pin9: // PWM1, PWM2, PWM3
			XBR1 |= XBR1_PCA0ME__CEX0_CEX1_CEX2;
			GPIO_MakeSpecialFunction(7, true);
			GPIO_MakeSpecialFunction(8, true);
			GPIO_MakeSpecialFunction(9, true);
			break;
		}
	}

	SFRPAGE = SFRPAGE_save;
}

void PWM_Disable() {
	uint8_t SFRPAGE_save = SFRPAGE;
	SFRPAGE = 0x00;
	GPIO_MakeInput(8, true);
	GPIO_MakeInput(9, true);
	GPIO_MakeInput(10, true);
	XBR1 &= ~(XBR1_PCA0ME__CEX0_CEX1_CEX2);
	SFRPAGE = SFRPAGE_save;
}

//P0SKIP |= SCL_BIT | SDA_BIT;
//XBR0 &= ~XBR0_SMB0E__ENABLED;
