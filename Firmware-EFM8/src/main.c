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
#include "gpio.h"
//-----------------------------------------------------------------------------
// Variables
//-----------------------------------------------------------------------------

//TIMERS
//Timer0: SMBus clock rate
//Timer1: UART clock rate
//Timer2: Unused
//Timer3: SMBus SCL low timeout detection
//Timer4: SoftPWM

int16_t main(void) {
	enter_DefaultMode_from_RESET();

	Treehopper_Init();

#ifdef ENABLE_TIMING_DEBUGGING
	GPIO_MakeOutput(10, PushPullOutput);
#endif

#ifdef ENABLE_UART_DEBGUGGING
	UART_StartDebugging115200();
#endif

	while (1) {
		Treehopper_Task();
	}

}
