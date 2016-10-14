/*
 * adc.c
 *
 *  Created on: Jul 24, 2015
 *      Author: jay
 */

#include "adc.h"
#include <SI_EFM8UB1_Register_Enums.h>
#include "treehopper.h"
#include "gpio.h"

SI_SEGMENT_VARIABLE(referenceLevels[TREEHOPPER_NUM_PINS], AdcReferenceLevel_t,
		SI_SEG_XDATA);
SI_SEGMENT_VARIABLE(adcMxVal[], static const uint8_t, SI_SEG_CODE) = {
	0, // pin0
	1,
	2,
	3,
	6,
	4,
	5,
	7,// pin7
	8,// pin8
	9,
	10,
	11,
	12,
	13,
	14,
	15,// pin15

	// Rev < A2
	23,// pin16
	22,
	21,
	20,
	// Rev > A2
//		20, // pin16
//		21,
//		22,
//		23,
};
void ADC_Init() {

}

void ADC_Enable(uint8_t pinNumber, AdcReferenceLevel_t referenceLevel) {
	uint8_t SFRPAGE_save = SFRPAGE;
	SFRPAGE = 0x00;
	referenceLevels[pinNumber] = referenceLevel;
	GPIO_MakeInput(pinNumber, false);
	SFRPAGE = SFRPAGE_save;
}

static void ADC_Disable(uint8_t pin) {

}

uint16_t ADC_GetVal(uint8_t pin) {
	uint8_t SFRPAGE_save = SFRPAGE;
	uint16_t adcVal;
	SFRPAGE = 0x00;

	switch (referenceLevels[pin]) {
	case VREF_1V65_PRECISION:
		REF0CN = REF0CN_IREFLVL__1P65 | REF0CN_GNDSL__GND_PIN
				| REF0CN_REFSL__INTERNAL_VREF | REF0CN_TEMPE__TEMP_ENABLED;
		ADC0CF = ADC0CF_ADGN__GAIN_1; // all other options are 0, so don't even bother bitmasking them
		break;
	case VREF_3V3_PRECISION:
		REF0CN = REF0CN_IREFLVL__1P65 | REF0CN_GNDSL__GND_PIN
				| REF0CN_REFSL__INTERNAL_VREF | REF0CN_TEMPE__TEMP_ENABLED;
		ADC0CF = ADC0CF_ADGN__GAIN_0P5;
		break;
	case VREF_2V4_PRECISION:
		REF0CN = REF0CN_IREFLVL__2P4 | REF0CN_GNDSL__GND_PIN
				| REF0CN_REFSL__INTERNAL_VREF | REF0CN_TEMPE__TEMP_ENABLED;
		ADC0CF = ADC0CF_ADGN__GAIN_1;
		break;
	case VREF_4V8_PRECISION:
		REF0CN = REF0CN_IREFLVL__2P4 | REF0CN_GNDSL__GND_PIN
				| REF0CN_REFSL__INTERNAL_VREF | REF0CN_TEMPE__TEMP_ENABLED;
		ADC0CF = ADC0CF_ADGN__GAIN_0P5;
		break;
	case VREF_1V8:
		REF0CN = REF0CN_IREFLVL__2P4 | REF0CN_GNDSL__GND_PIN
				| REF0CN_REFSL__INTERNAL_LDO | REF0CN_TEMPE__TEMP_ENABLED;
		ADC0CF = ADC0CF_ADGN__GAIN_1;
		break;
	case VREF_3V6:
		REF0CN = REF0CN_IREFLVL__2P4 | REF0CN_GNDSL__GND_PIN
				| REF0CN_REFSL__INTERNAL_LDO | REF0CN_TEMPE__TEMP_ENABLED;
		ADC0CF = ADC0CF_ADGN__GAIN_0P5;
		break;
	case VREF_3V3:
		REF0CN = REF0CN_IREFLVL__2P4 | REF0CN_GNDSL__GND_PIN
				| REF0CN_REFSL__VDD_PIN | REF0CN_TEMPE__TEMP_ENABLED;
		ADC0CF = ADC0CF_ADGN__GAIN_1;
		break;
	case VREF_6V6:
		REF0CN = REF0CN_IREFLVL__2P4 | REF0CN_GNDSL__GND_PIN
				| REF0CN_REFSL__VDD_PIN | REF0CN_TEMPE__TEMP_ENABLED;
		ADC0CF = ADC0CF_ADGN__GAIN_0P5;
		break;
	}

	ADC0MX = adcMxVal[pin];
	ADC0CN0_ADBUSY = 1;
	while (!ADC0CN0_ADINT)
		;
	ADC0CN0_ADINT = 0;
	adcVal = ADC0 >> 1;
	SFRPAGE = SFRPAGE_save;
	return adcVal;
}
