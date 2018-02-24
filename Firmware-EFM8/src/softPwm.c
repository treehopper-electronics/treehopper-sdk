/*
 * SoftPwm.c
 *
 *  Created on: Jan 3, 2016
 *      Author: jay
 */

#include "softPwm.h"
#include "gpio.h"
#include "treehopper.h"

static SI_SEGMENT_VARIABLE(currentConfig[TREEHOPPER_NUM_PINS], softPwmPinConfig_t, SI_SEG_XDATA);
static SI_SEGMENT_VARIABLE(newConfig[TREEHOPPER_NUM_PINS], softPwmPinConfig_t, SI_SEG_XDATA);

uint8_t currentNumConfigs;
uint8_t newNumConfigs;

uint8_t currentConfigIdx;

volatile bit lock = false;
bit isRunning = false;
void SoftPwm_Init() {
	currentNumConfigs = 0;
	newNumConfigs = 0;
	currentConfigIdx = 0;
	memset(currentConfig, 0, TREEHOPPER_NUM_PINS * sizeof(softPwmPinConfig_t));
	memset(newConfig, 0, TREEHOPPER_NUM_PINS * sizeof(softPwmPinConfig_t));
	TMR4CN0 &= ~(TMR4CN0_TR4__BMASK); // disable timer
}

bit shouldReload = false;

void loadNewConfig() {
	currentNumConfigs = newNumConfigs;
	memcpy(currentConfig, newConfig,
			newNumConfigs * sizeof(softPwmPinConfig_t));
}

void SoftPwm_SetConfig(softPwmPinConfig_t* config, uint8_t len) {
	if (len > TREEHOPPER_NUM_PINS) // sanity check to prevent bad memory copying
		return;

	SFRPAGE = 0x10;
	if (len == 0) {
		TMR4 = 0x0000;
		isRunning = false;
		TMR4CN0 &= ~(TMR4CN0_TR4__BMASK);
		newNumConfigs = 0;
	} else {
		lock = true;
		memcpy(newConfig, config, len * sizeof(softPwmPinConfig_t));
		newNumConfigs = len;

		if (!isRunning) {
			loadNewConfig(); // we're not running so we can force a reload now
			currentConfigIdx = 0; // reset the index
			// we weren't running before
			TMR4CN0 |= TMR4CN0_TR4__RUN;
			isRunning = true;
			TMR4 = 0xffff;
		}
		lock = false;
	}
}

void SoftPwm_Task() {
	uint8_t i;
	if (!isRunning)
		return;

	if (currentConfigIdx == 0) // we're at the first state, so set all pins high
	{
		for (i = 1; i < currentNumConfigs; i++) {
			GPIO_WriteValue(currentConfig[i].pinNumber, true);
		}
	} else {
		GPIO_WriteValue(currentConfig[currentConfigIdx].pinNumber, false);
	}

	SFRPAGE = 0x10;
	TMR4 = currentConfig[currentConfigIdx].nextTmrVal;

	currentConfigIdx++;

	if (currentConfigIdx == currentNumConfigs) {
		if (!lock) // reload if we can, otherwise, wait until next time
		{
			loadNewConfig();
		}

		currentConfigIdx = 0;
	}
}

void SoftPwm_Test() {
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
