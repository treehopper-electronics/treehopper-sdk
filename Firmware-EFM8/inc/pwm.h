/*
 * pwm.h
 *
 *  Created on: Jul 25, 2015
 *      Author: jay
 */

#ifndef PWM_H_
#define PWM_H_

#include <stdint.h>

void PWM_Init();
void PWM_Disable();
void PWM_SetConfig(uint8_t* configuration);

extern volatile uint8_t PWM_1L, PWM_1H, PWM_2L, PWM_2H, PWM_3L, PWM_3H;

#endif /* PWM_H_ */
