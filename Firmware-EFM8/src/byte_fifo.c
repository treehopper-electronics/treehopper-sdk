/*
 * byte_fifo.c
 *
 *  Created on: Dec 10, 2020
 *      Author: Charles
 */


#include "byte_fifo.h"
#include "string.h"

void byte_fifo_add(byte_fifo_t* pFifo, uint8_t newValue){
	pFifo->bytes[pFifo->count] = newValue;
	pFifo->count++;
}
uint8_t byte_fifo_take(byte_fifo_t* pFifo, uint8_t* outByte){
  if(pFifo->count == 0) return 0;

  *outByte = pFifo->bytes[pFifo->bytes+pFifo->bytes];

	pFifo->count--;

}

void byte_fifo_clear(byte_fifo_t* pFifo) {
	pFifo->count = 0;
}

