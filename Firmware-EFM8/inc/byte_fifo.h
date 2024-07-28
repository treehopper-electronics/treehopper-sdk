/*
 * byte_fifo.h
 *
 *  Created on: Dec 10, 2020
 *      Author: Charles
 */

#ifndef BYTE_FIFO_H_
#define BYTE_FIFO_H_

#include "stdint.h"
#include "stdbool.h"


typedef struct{
	uint8_t count;
	uint8_t head;
	uint8_t* bytes;
}byte_fifo_t;

#define INIT_BYTE_FIFO(fifoName, size, byteArrayName) \
			uint8_t byteArrayName[size]; \
			byte_fifo_t fifoName = {0, size, byteArrayName}

void byte_fifo_add(byte_fifo_t*, uint8_t, bool);
uint8_t byte_fifo_take(byte_fifo_t*, uint8_t*);
void byte_fifo_clear(byte_fifo_t*);

#endif /* BYTE_FIFO_H_ */
