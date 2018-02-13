/******************************************************************************
 * Copyright (c) 2014 by Silicon Laboratories Inc. All rights reserved.
 *
 * http://developer.silabs.com/legal/version/v11/Silicon_Labs_Software_License_Agreement.txt
 *****************************************************************************/

#ifndef __EFM8_CONFIG_H__
#define __EFM8_CONFIG_H__

#include "treehopper.h"

// $[USB driver options]
#define EFM8PDL_USB0_USE                1
#define EFM8PDL_USB0_IN_DATA_ENABLED    1
#define EFM8PDL_USB0_OUT_DATA_ENABLED   1
// [USB driver options]$

// SPI debugging
//#define SPI_DEBUGGING

#ifdef SPI_DEBUGGING
	#define SPI_DEBUG_PIN0_HIGH();	PIN8 = true;
	#define SPI_DEBUG_PIN0_LOW();	PIN8 = false;
	#define SPI_DEBUG_PIN1_HIGH();	PIN9 = true;
	#define SPI_DEBUG_PIN1_LOW();	PIN9 = false;
	#define SPI_DEBUG_PIN2_HIGH();	PIN10 = true;
	#define SPI_DEBUG_PIN2_LOW();	PIN10 = false;
	#define SPI_DEBUG_PIN3_HIGH();	PIN11 = true;
	#define SPI_DEBUG_PIN3_LOW();	PIN11 = false;
#else
	#define SPI_DEBUG_PIN0_HIGH();
	#define SPI_DEBUG_PIN0_LOW();
	#define SPI_DEBUG_PIN1_HIGH();
	#define SPI_DEBUG_PIN1_LOW();
	#define SPI_DEBUG_PIN2_HIGH();
	#define SPI_DEBUG_PIN2_LOW();
	#define SPI_DEBUG_PIN3_HIGH();
	#define SPI_DEBUG_PIN3_LOW();
#endif



#endif // __EFM8_CONFIG_H__
