/*
 * i2c.h
 *
 *  Created on: Jul 23, 2015
 *      Author: jay
 */

#ifndef I2C_H_
#define I2C_H_

#include <stdint.h>

typedef struct {
	unsigned int IsEnabled;
	unsigned int TH0Val;
} I2cConfigData_t;

void I2C_Init();
void I2C_SetConfig(I2cConfigData_t* config);
void I2C_Transaction(uint8_t address, uint8_t* tx, uint8_t* rx, uint8_t txlen, uint8_t rxlen);
void I2C_Enable();
void I2C_Disable();

#endif /* I2C_H_ */
