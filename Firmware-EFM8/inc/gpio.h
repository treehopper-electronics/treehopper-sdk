/*
 * gpio.h
 *
 *  Created on: Jul 19, 2015
 *      Author: jay
 */

#ifndef GPIO_H_
#define GPIO_H_
#include <stdint.h>
#include <stdbool.h>

typedef enum pinMode {ReservedPin, DigitalInput, PushPullOutput, OpenDrainOutput, AnalogInput} pinMode_t;

void GPIO_MakeInput(uint8_t pinNumber);
void GPIO_MakeOutput(uint8_t pinNumber, uint8_t OutputType);
void GPIO_MakeSpecialFunction(uint8_t pinNumber, uint8_t pushPull);
void GPIO_WriteValue(uint8_t pinNumber, bool val);
bool GPIO_ReadValue(uint8_t pinNumber);


#endif /* GPIO_H_ */
