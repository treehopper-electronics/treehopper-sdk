/*
 * parallel.h
 *
 *  Created on: Nov 15, 2016
 *      Author: jay
 */

#ifndef PARALLEL_H_
#define PARALLEL_H_

#include <stdint.h>

void Parallel_SetConfig(uint8_t* config);
void Parallel_Transaction(uint8_t* transaction);

#endif /* PARALLEL_H_ */
