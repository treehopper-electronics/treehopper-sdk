/*
 * SoftPwm.c
 *
 *  Created on: Jan 3, 2016
 *      Author: jay
 */

#include "softPwm.h"
#include "gpio.h"
#include "treehopper.h"

//#define	DEBUG_SOFTPWM

static SI_SEGMENT_VARIABLE(currentConfig[TREEHOPPER_NUM_PINS], softPwmPinConfig_t, SI_SEG_XDATA);
static SI_SEGMENT_VARIABLE(newConfig[TREEHOPPER_NUM_PINS], softPwmPinConfig_t, SI_SEG_XDATA);

uint8_t currentNumConfigs = 0;
uint8_t newNumConfigs = 0;

uint8_t currentConfigIdx  = 0;

volatile bit lock = false;
bit isRunning = false;
void SoftPwm_Init()
{
#ifdef DEBUG_SOFTPWM
	GPIO_MakeOutput(9, PushPullOutput);
	GPIO_MakeOutput(7, PushPullOutput);
#endif
	memset(currentConfig, 0, TREEHOPPER_NUM_PINS * sizeof(softPwmPinConfig_t));
	memset(newConfig, 0, TREEHOPPER_NUM_PINS * sizeof(softPwmPinConfig_t));

//		SoftPwm_Test();
}


bit shouldReload = false;

void loadNewConfig()
{
	currentNumConfigs = newNumConfigs;
	memcpy(currentConfig, newConfig, newNumConfigs * sizeof(softPwmPinConfig_t));
}

void SoftPwm_SetConfig(softPwmPinConfig_t* config, uint8_t len)
{
#ifdef DEBUG_SOFTPWM
	PIN7 = true; // use pin9 to test the interrupt speed
#endif

	if(len > TREEHOPPER_NUM_PINS) // sanity check to prevent bad memory copying
		return;

	SFRPAGE = 0x10;
	if(len == 0)
	{
		TMR3 = 0x0000;
		isRunning = false;
//		TMR3CN0 &= ~(TMR3CN0_TR3__RUN);
		newNumConfigs = 0;
	} else {
		lock = true;
		memcpy(newConfig, config, len * sizeof(softPwmPinConfig_t));
		newNumConfigs = len;



		if(!isRunning)
		{
			loadNewConfig(); // we're not running so we can force a reload now
			currentConfigIdx = 0; // reset the index
			// we weren't running before
			isRunning = true;
			TMR3 = 0xffff;
		}
		lock = false;
//		if(!(TMR3CN0 & TMR3CN0_TR3__RUN))
//		{
//			TMR3CN0 |= TMR3CN0_TR3__RUN;
//			TMR3 = 0xfff0; // let's get this show on the road, so trigger an interrupt ASAP
//		}
	}


#ifdef DEBUG_SOFTPWM
	PIN7 = false; // use pin9 to test the interrupt speed
#endif

}

void SoftPwm_Task()
{
	uint8_t i;
#ifdef DEBUG_SOFTPWM
		PIN9 = true; // use pin9 to test the interrupt speed
#endif
		if(!isRunning)
			return;

		if(currentConfigIdx == 0) // we're at the first state, so set all pins high
		{
			for(i=1;i<currentNumConfigs;i++)
			{
				GPIO_WriteValue(currentConfig[i].pinNumber, true);
			}
		} else {
			GPIO_WriteValue(currentConfig[currentConfigIdx].pinNumber, false);
		}

		SFRPAGE = 0x00;
		TMR3 = currentConfig[currentConfigIdx].nextTmrVal;

		currentConfigIdx++;

		if(currentConfigIdx == currentNumConfigs)
		{
			if(!lock) // reload if we can, otherwise, wait until next time
			{
				loadNewConfig();
			}

			currentConfigIdx = 0;
		}




#ifdef DEBUG_SOFTPWM
	PIN9 = false; // use pin9 to test the interrupt speed
#endif
}

void SoftPwm_Test()
{
	softPwmPinConfig_t testConfig[3];

	GPIO_MakeOutput(1, PushPullOutput);
	GPIO_MakeOutput(10, PushPullOutput);

	testConfig[0].pinNumber = 0;
	testConfig[0].nextTmrVal = 32768;

	testConfig[1].pinNumber = 10;
	testConfig[1].nextTmrVal = 65535;

	testConfig[2].pinNumber = 1;
	testConfig[2].nextTmrVal = 32768;

	SoftPwm_SetConfig(&testConfig, 3);
}


//#include "softPwm.h"
//#include "gpio.h"
//#include "treehopper.h"
//#include "string.h"
//
////#define	DEBUG_SOFTPWM
//
//SI_SEGMENT_VARIABLE(config[TREEHOPPER_NUM_PINS], softPwmPinConfig_t, SI_SEG_XDATA);
//
//uint8_t numSlots = 0;
//void SoftPwm_Init()
//{
//#ifdef DEBUG_SOFTPWM
//	GPIO_MakeOutput(9, PushPullOutput);
//	GPIO_MakeOutput(7, PushPullOutput);
//#endif
//	memset(config, 0, TREEHOPPER_NUM_PINS * sizeof(softPwmPinConfig_t));
////	SoftPwm_Test();
//}
//
//volatile bit lock = false;
//
//void SoftPwm_SetConfig(softPwmPinConfig_t* newConfig, uint8_t len)
//{
//#ifdef DEBUG_SOFTPWM
//	PIN7 = true; // use pin9 to test the interrupt speed
//#endif
//	lock = true;
//	if(len > TREEHOPPER_NUM_PINS) // sanity check to prevent bad memory copying
//		return;
//
//	SFRPAGE = 0x10;
//	if(len == 0)
//	{
//		TMR4CN0_TR4 = false;
//	} else {
//		if(!TMR4CN0_TR4)
//		{
//			TMR4CN0_TR4 = true;
//		}
//
//		memcpy(config, newConfig, len * sizeof(softPwmPinConfig_t));
//	}
//	numSlots = len;
//	lock = false;
//#ifdef DEBUG_SOFTPWM
//	PIN7 = false; // use pin9 to test the interrupt speed
//#endif
//}
//uint8_t currentTime = 0;
//void SoftPwm_Task()
//{
//	uint8_t i;
//#ifdef DEBUG_SOFTPWM
//	PIN9 = true; // use pin9 to test the interrupt speed
//#endif
//	if(!lock)
//	{
//		if(currentTime == 0)
//		{
//			for(i = 0; i<numSlots;i++)
//			{
//				if(config[i].dutyCycle != 0)
//					GPIO_WriteValue(config[i].pinNumber, true);
//			}
//		} else {
//			for(i = 0; i<numSlots;i++)
//			{
//				if(config[i].dutyCycle < currentTime)
//					GPIO_WriteValue(config[i].pinNumber, false);
//			}
//		}
//
//		currentTime++;
//	}
//
//#ifdef DEBUG_SOFTPWM
//	PIN9 = false;
//#endif
//}
//
//void SoftPwm_Test()
//{
//	softPwmPinConfig_t testConfig;
//
//	GPIO_MakeOutput(10, PushPullOutput);
//	testConfig.pinNumber = 10;
//	testConfig.dutyCycle = 128;
//	SoftPwm_SetConfig(&testConfig, 1);
//}
