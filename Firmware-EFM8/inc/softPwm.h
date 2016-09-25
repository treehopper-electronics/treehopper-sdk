/*
 * softPwm.h
 *
 *  Created on: Jan 3, 2016
 *      Author: jay
 */

#ifndef SOFTPWM_H_
#define SOFTPWM_H_

#include "efm8_usb.h"
#include <stdint.h>

typedef struct {
	uint8_t pinNumber;
	uint16_t nextTmrVal;
} softPwmPinConfig_t;

void SoftPwm_Init();
void SoftPwm_SetConfig(softPwmPinConfig_t* config, uint8_t len);
void SoftPwm_Task();
void SoftPwm_Test();
#endif /* SOFTPWM_H_ */
