/*
 * adc.h
 *
 *  Created on: Jul 24, 2015
 *      Author: jay
 */

#ifndef ADC_H_
#define ADC_H_

#include <stdint.h>
#include <stdbool.h>


typedef enum AdcReferenceLevel
    {
        /// <summary>
        /// 3.3V rail, useful for ratiometric sensors hooked up to 3.3V supply.
        /// </summary>
        VREF_3V3,
        /// <summary>
        /// Double the 3.3V VDD rail, useful for 5V-output ratiometric sensors
        /// </summary>
        /// <remarks>
        /// This setting is derived from the 3.3V VDD rail, so it's useful for measuring absolute-output sensors that range
        /// from 0 to 5V. Using this mode to sample ratiometric sensors powered from the raw 5V USB VBUS supply is not recommended,
        /// since this supply voltage varies considerably, and the reference voltage used for this setting is derived from the regulated 3.3V
        /// VDD voltage, not the 5V USB VBUS voltage.
        /// </remarks>
        VREF_1V65_PRECISION,
        VREF_1V8,
        VREF_2V4_PRECISION,
        VREF_3V3_PRECISION,
        VREF_3V6,
        VREF_4V8_PRECISION, // these don't work above 3.6V
        VREF_6V6
    } AdcReferenceLevel_t;

void ADC_Init();
void ADC_Enable(uint8_t pin, AdcReferenceLevel_t);
void ADC_Disable(uint8_t pin);
uint16_t ADC_GetVal(uint8_t pin);

#endif /* ADC_H_ */
