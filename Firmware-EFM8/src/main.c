/////////////////////////////////////////////////////////////////////////////
// main.c
/////////////////////////////////////////////////////////////////////////////

/////////////////////////////////////////////////////////////////////////////
// Includes
/////////////////////////////////////////////////////////////////////////////
#include "InitDevice.h"
#include "efm8_usb.h"
#include "descriptors.h"
#include "usbconfig.h"
#include "treehopper.h"
#include "adc.h"
#include "spi.h"
#include "uart.h"
#include "i2c.h"
#include "i2c_0.h"
#include "serialNumber.h"
#include "led.h"
#include "pwm.h"
#include "softPwm.h"
//-----------------------------------------------------------------------------
// Variables
//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------
// main() Routine
// ----------------------------------------------------------------------------
//
// Note: the software watchdog timer is not disabled by default in this
// example, so a long-running program will reset periodically unless
// the timer is disabled or your program periodically writes to it.
//
// Review the "Watchdog Timer" section under the part family's datasheet
// for details.
//
//-----------------------------------------------------------------------------
int16_t main(void) {

	enter_DefaultMode_from_RESET();

	serialNumber_init();

	LED_Init();
	SPI_Disable();
	UART_Disable();
	I2C_Disable();
	PWM_Disable();

	Treehopper_Init();

	SoftPwm_Init();
//  ServoController_Init();

	while (1) {
		Treehopper_Task();
	}

}
