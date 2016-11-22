/*******************************************************************************
 * @file callback.c
 * @brief USB Callbacks.
 *******************************************************************************/
//=============================================================================
// src/callback.c: generated by Hardware Configurator
//
// This file is only generated if it does not exist. Modifications in this file
// will persist even if Configurator generates code. To refresh this file,
// you must first delete it and then regenerate code.
//=============================================================================
//-----------------------------------------------------------------------------
// Includes
//-----------------------------------------------------------------------------
#include <SI_EFM8UB1_Register_Enums.h>
#include <efm8_usb.h>
#include "descriptors.h"
#include "treehopper.h"
#include "gpio.h"
#include "adc.h"
#include "pwm.h"
#include "led.h"
#include "spi.h"
#include "i2c.h"
#include "serialNumber.h"
#include <stdint.h>
#include "uart.h"
//-----------------------------------------------------------------------------
// Constants
//-----------------------------------------------------------------------------
//-----------------------------------------------------------------------------
// Variables
//-----------------------------------------------------------------------------
SI_SEGMENT_VARIABLE(msCompatDesc[], static const USB_StringDescriptor_TypeDef, SI_SEG_CODE) = {
	0x28, 0x00, 00, 0x00,
	0x00, 0x01,
	0x04, 0x00,
	0x01,
	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
	0x00,
	0x01,
	0x57, 0x49, 0x4E, 0x55, 0x53, 0x42, 0x00, 0x00,
	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
	0x00, 0x00, 0x00, 0x00, 0x00, 0x00
};

typedef struct _MS_EXT_PROPERTY_FEATURE_DESC {
	uint32_t dwLength;
	uint16_t bcdVersion;
	uint16_t wIndex;
	uint16_t wCount;
	uint32_t dwSize;
	uint32_t dwPropertyDataType;
	uint16_t wPropertyNameLength;
	uint16_t bPropertyName[20];
	uint32_t dwPropertyDataLength;
	uint16_t bPropertyData[39];
} MS_EXT_PROPERTY_FEATURE_DESC;
SI_SEGMENT_VARIABLE(ExtPropertyFeatureDescriptor[], static const USB_StringDescriptor_TypeDef, SI_SEG_CODE) = {
	//----------Header Section--------------
	0x92, 0x00, 0x00, 0x00,//dwLength
	0x00, 0x01,//bcdVersion = 1.00
	0x05, 0x00,//wIndex
	0x01, 0x00,//wCount - 0x0001 "Property Sections" implemented in this descriptor
	//----------Property Section 1----------
	0x88, 0x00, 0x00, 0x00,//dwSize - 136 bytes in this Property Section
	0x07, 0x00, 0x00, 0x00 ,//dwPropertyDataType (Unicode REG_MULTI_SZ string)
	0x2a, 0x00,//wPropertyNameLength - 40 bytes in the bPropertyName field
	'D',0x00, 'e', 0x00, 'v', 0x00, 'i', 0x00, 'c', 0x00, 'e', 0x00, 'I', 0x00, 'n', 0x00, 't', 0x00, 'e', 0x00, 'r', 0x00, 'f', 0x00, 'a', 0x00, 'c', 0x00, 'e', 0x00, 'G', 0x00, 'U', 0x00, 'I', 0x00, 'D', 0x00,'s', 0x00, 0x00, 0x00,
	//bPropertyName - "DeviceInterfaceGUID"
	0x50, 0x00, 0x00, 0x00,//dwPropertyDataLength - 78 bytes in the bPropertyData field (GUID value in UNICODE formatted string, with braces and dashes)
	'{', 0x00, '5', 0x00, 'B', 0x00, '3', 0x00, '4', 0x00, 'B', 0x00, '3', 0x00, '8', 0x00, 'B', 0x00, '-', 0x00, 'F', 0x00, '4', 0x00, 'C', 0x00, 'D', 0x00, '-', 0x00, '4', 0x00, '9', 0x00, 'C', 0x00, '3', 0x00, '-', 0x00, 'B', 0x00, '2', 0x00, 'B', 0x00, 'B', 0x00, '-', 0x00, '6', 0x00, '0', 0x00, 'E', 0x00, '4', 0x00, '7', 0x00, 'A', 0x00, '4', 0x00, '3', 0x00, 'E', 0x00, '1', 0x00, '2', 0x00, 'D', 0x00, '}', 0x00, 0x00, 0x00, 0x00, 0x00//bPropertyData - this is the actual GUID value.  Make sure this matches the PC application code trying to connect to the device.
};

uint8_t totalBytesToSend = 0;
uint8_t currentOffsetToSend = 0;

//AppState_t state;

void USBD_ResetCb(void) {

}

void USBD_SofCb(uint16_t sofNr) {

}

void USBD_DeviceStateChangeCb(USBD_State_TypeDef oldState,
		USBD_State_TypeDef newState) {
	if (newState == USBD_STATE_CONFIGURED) {
		// Arm these endpoints once we're configured
		USBD_Read(EP1OUT, &Treehopper_PinConfig, 8, false);
		USBD_Read(EP2OUT, &Treehopper_PeripheralConfig, 64, false);
	}
}

bool USBD_IsSelfPoweredCb(void) {

	return true;
}

USB_Status_TypeDef USBD_SetupCmdCb(
		SI_VARIABLE_SEGMENT_POINTER(setup, USB_Setup_TypeDef, MEM_MODEL_SEG)) {
	uint16_t length = 0;
	USB_Status_TypeDef retVal = USB_STATUS_REQ_UNHANDLED;

	if (*((uint8_t *) &(setup->bmRequestType)) == 0xC0) {
		if (setup->wIndex == EXTENDED_COMPAT_ID) {
			retVal = USB_STATUS_OK;
			length = 40;
			if (setup->wLength < length) {
				length = setup->wLength;
			}

			USBD_Write(EP0, msCompatDesc, length, false);
		}

	} else if (*((uint8_t *) &(setup->bmRequestType)) == 0xC1) {
		retVal = USB_STATUS_OK;
		if (setup->wIndex == EXTENDED_PROPERTIES) {
			length = sizeof(ExtPropertyFeatureDescriptor);
			if (setup->wLength < length) {
				length = setup->wLength;
			}

			USBD_Write(EP0, &ExtPropertyFeatureDescriptor, length, false);
		}

	}

	return retVal;
}

uint16_t USBD_XferCompleteCb(uint8_t epAddr, USB_Status_TypeDef status,
		uint16_t xferred, uint16_t remaining) {
	return 0;
}
