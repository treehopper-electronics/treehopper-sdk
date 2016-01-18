/*
 * adc.c
 *
 *  Created on: Jul 24, 2015
 *      Author: jay
 */

#include "adc.h"
#include <SI_EFM8UB1_Register_Enums.h>
#include "treehopper.h"
SI_SEGMENT_VARIABLE(referenceLevels[TREEHOPPER_NUM_PINS+1], AdcReferenceLevel_t, SI_SEG_XDATA);
SI_SEGMENT_VARIABLE(adcMxVal[],  static const uint8_t, SI_SEG_CODE) = {
		0,
		0, // pin1
		1,
		2,
		3,
		6,
		4,
		5,
		7, // pin8
		8, // pin9
		9,
		10,
		11,
		12,
		13,
		14,
		15, // pin16
		20, // pin17
		21,
		22,
		23,
};
void ADC_Init()
{

}

void ADC_Enable(uint8_t pinNumber, AdcReferenceLevel_t referenceLevel)
{
	uint8_t SFRPAGE_save = SFRPAGE;
	SFRPAGE = 0x00;
	referenceLevels[pinNumber] = referenceLevel;
	if(pinNumber < 9)
	{
		P0SKIP |= 1 << (pinNumber-1);
		P0MDIN &= ~(1 << (pinNumber-1));
		P0 |= 1 << (pinNumber-1);
	}
	else if(pinNumber < 17)
	{
		P1SKIP |= 1 << (pinNumber-9);
		P1MDIN &= ~(1 << (pinNumber-9));
		P1 |= 1 << (pinNumber-9);
	} else {
		P2SKIP |= 1 << (pinNumber-17);
		P2MDIN &= ~(1 << (pinNumber-17));
		P2 |= 1 << (pinNumber-17);
	}
	SFRPAGE = SFRPAGE_save;
}

static void ADC_Disable(uint8_t pin)
{

}

uint16_t ADC_GetVal(uint8_t pin)
{
	uint8_t SFRPAGE_save = SFRPAGE;
	uint16_t adcVal;
	SFRPAGE = 0x00;

	switch(referenceLevels[pin])
	{
	case VREF_1V65_PRECISION:
		REF0CN = REF0CN_IREFLVL__1P65 | REF0CN_GNDSL__GND_PIN | REF0CN_REFSL__INTERNAL_VREF | REF0CN_TEMPE__TEMP_ENABLED;
		ADC0CF = ADC0CF_ADGN__GAIN_1; // all other options are 0, so don't even bother bitmasking them
		break;
	case VREF_3V3_PRECISION:
		REF0CN = REF0CN_IREFLVL__1P65 | REF0CN_GNDSL__GND_PIN | REF0CN_REFSL__INTERNAL_VREF | REF0CN_TEMPE__TEMP_ENABLED;
		ADC0CF = ADC0CF_ADGN__GAIN_0P5;
		break;
	case VREF_2V4_PRECISION:
		REF0CN = REF0CN_IREFLVL__2P4 | REF0CN_GNDSL__GND_PIN | REF0CN_REFSL__INTERNAL_VREF | REF0CN_TEMPE__TEMP_ENABLED;
		ADC0CF = ADC0CF_ADGN__GAIN_1;
		break;
	case VREF_4V8_PRECISION:
		REF0CN = REF0CN_IREFLVL__2P4 | REF0CN_GNDSL__GND_PIN | REF0CN_REFSL__INTERNAL_VREF | REF0CN_TEMPE__TEMP_ENABLED;
		ADC0CF = ADC0CF_ADGN__GAIN_0P5;
		break;
	case VREF_1V8:
		REF0CN = REF0CN_IREFLVL__2P4 | REF0CN_GNDSL__GND_PIN | REF0CN_REFSL__INTERNAL_LDO | REF0CN_TEMPE__TEMP_ENABLED;
		ADC0CF = ADC0CF_ADGN__GAIN_1;
		break;
	case VREF_3V6:
		REF0CN = REF0CN_IREFLVL__2P4 | REF0CN_GNDSL__GND_PIN | REF0CN_REFSL__INTERNAL_LDO | REF0CN_TEMPE__TEMP_ENABLED;
		ADC0CF = ADC0CF_ADGN__GAIN_0P5;
		break;
	case VREF_3V3:
		REF0CN = REF0CN_IREFLVL__2P4 | REF0CN_GNDSL__GND_PIN | REF0CN_REFSL__VDD_PIN | REF0CN_TEMPE__TEMP_ENABLED;
		ADC0CF = ADC0CF_ADGN__GAIN_1;
		break;
	case VREF_6V6:
		REF0CN = REF0CN_IREFLVL__2P4 | REF0CN_GNDSL__GND_PIN | REF0CN_REFSL__VDD_PIN | REF0CN_TEMPE__TEMP_ENABLED;
		ADC0CF = ADC0CF_ADGN__GAIN_0P5;
		break;
	}

	ADC0MX = adcMxVal[pin];
	ADC0CN0_ADBUSY = 1;
	while(!ADC0CN0_ADINT);
	ADC0CN0_ADINT = 0;
	adcVal = ADC0;
	SFRPAGE = SFRPAGE_save;
	return adcVal;
}
