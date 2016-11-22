/*
 * parallel.c
 *
 *  Created on: Nov 15, 2016
 *      Author: jay
 */

#include "parallel.h"
#include <si_toolchain.h>
#include "gpio.h"

SI_SEGMENT_VARIABLE(dir, uint8_t, SI_SEG_XDATA);
SI_SEGMENT_VARIABLE(IsEnabled, uint8_t, SI_SEG_XDATA);
SI_SEGMENT_VARIABLE(Delay, uint8_t, SI_SEG_XDATA);
SI_SEGMENT_VARIABLE(Count, uint8_t, SI_SEG_XDATA);
SI_SEGMENT_VARIABLE(RS, int8_t, SI_SEG_XDATA);
SI_SEGMENT_VARIABLE(RW, int8_t, SI_SEG_XDATA);
SI_SEGMENT_VARIABLE(EN, int8_t, SI_SEG_XDATA);
SI_SEGMENT_VARIABLE(DataBus[16], int8_t, SI_SEG_XDATA);

typedef enum ParallelCmd
{
	ParallelCmd_WriteCommand,
	ParallelCmd_ReadCommand,
	ParallelCmd_WriteData,
	ParallelCmd_ReadData
} ParallelCmd_t;

void setBus(uint16_t val);
void writeCommand(uint8_t);
void writeDataArray(uint8_t* dat, uint8_t len);
void writeData(uint16_t dat);
void makeBusOutput();
void makeBusInput();
void _delay(uint8_t delay);
void pulse();

void Parallel_SetConfig(uint8_t *config)
{
	uint8_t i;
	IsEnabled = config[0];
	Delay = config[1];
	Count = config[2];
	RS = (int8_t)config[3];
	RW = (int8_t)config[4];
	EN = (int8_t)config[5];
	if(IsEnabled)
	{
		// Note that if the user doesn't set these, they come in as -1,
		// but all GPIO functions handle invalid pin numbers with no issues,
		// so we don't need additional checks here
		GPIO_MakeOutput(RS, PushPullOutput);
		GPIO_MakeOutput(RW, PushPullOutput);
		GPIO_MakeOutput(EN, PushPullOutput);

		for(i=0;i<Count;i++) {
			DataBus[i] = (int8_t)config[6+i];
			GPIO_MakeOutput(DataBus[i], PushPullOutput);
		}

	} else {
		GPIO_MakeInput(RS, true);
		GPIO_MakeInput(RW, true);
		GPIO_MakeInput(EN, true);

		for(i=0;i<Count;i++) {
			GPIO_MakeInput(DataBus[i], true);
		}
	}




}

void Parallel_Transaction(uint8_t* transaction)
{
	uint8_t i;
	uint16_t tmp;
	switch(transaction[0])
	{
	case ParallelCmd_WriteCommand:
		// transaction[0] : WriteCommand
		// transaction[1] : Length
		// transaction[2] : Command
		// transaction[3..] : Data

		writeCommand(transaction[2]);
		if(transaction[1] > 0) // check length is positive
			writeDataArray(&(transaction[3]), transaction[1]);
		break;

	case ParallelCmd_WriteData:
		// transaction[0] : WriteData
		// transaction[1] : Length
		// transaction[2..] : Data
		writeDataArray(&(transaction[2]), transaction[1]);
		break;
	}
}

void writeDataArray(uint8_t* dat, uint8_t len)
{
	uint16_t tmp;
	uint8_t i;
	for(i=0;i<len;i++)
	{
		if(Count > 8)
		{
			// 16-bit bus
			tmp = dat[i*2] << 8 | dat[i*2 + 1];
			writeData(tmp);
		} else {
			// 8-bit bus
			writeData(dat[i]);
		}
	}
}

void writeCommand(uint8_t cmd)
{
	makeBusOutput();
	GPIO_WriteValue(RS, 0);
	GPIO_WriteValue(RW, 0);
	setBus(cmd);
	pulse();

}


void writeData(uint16_t dat)
{
	makeBusOutput();
	GPIO_WriteValue(RS, 1);
	GPIO_WriteValue(RW, 0);

	setBus(dat);
	pulse();
}


void setBus(uint16_t val)
{
	uint8_t i;
	for(i = 0; i<Count;i++)
	{
		GPIO_WriteValue(DataBus[i], (val >> i) & 0x01);
	}
}

void makeBusOutput()
{
	uint8_t i;
	if(dir == 0) return;
	for(i=0;i<Count;i++)
		GPIO_MakeOutput(DataBus[i], PushPullOutput);
	dir = 0;
}

void makeBusInput()
{
	uint8_t i;
	if(dir == 1) return;
	for(i=0;i<Count;i++)
		GPIO_MakeInput(DataBus[i], true);
	dir = 1;
}

void pulse()
{
	GPIO_WriteValue(EN, 0);
//	delay();
	GPIO_WriteValue(EN, 1);
//	delay();
	GPIO_WriteValue(EN, 0);
	delay(Delay);
}

void _delay(uint8_t delay)
{
	uint8_t i;
	for(i=0;i<delay;i++);
}
