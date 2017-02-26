#pragma once

#include "Treehopper.h"
#include <stdint.h>
#include <functional>
#include "DigitalIn.h"
#include "DigitalOut.h"
#include "Event.h"
#include "SpiChipSelectPin.h"

using namespace std;

namespace Treehopper 
{
	class TreehopperUsb;

	enum class AdcReferenceLevel
	{
		VREF_3V3,
		VREF_1V65,
		VREF_1V8,
		VREF_2V4,
		VREF_3V3_DERIVED,
		VREF_3V6
	};

	enum class PinMode
	{
		Reserved,
		DigitalInput,
		PushPullOutput,
		OpenDrainOutput,
		AnalogInput,
		Unassigned
	};

	/** Represents an I/O pin on Treehopper; it provides core digital I/O (GPIO) and ADC functionality.

	Each Pin can function as a digital input, digital output (in either push-pull or open-drain configurations), or an analog input. Configure this by calling Pin::mode(). Many Treehopper pins are also used by peripherals (Spi, I2c, Uart, or Pwm); when these peripherals are active, the respective pins is set to PinMode::Reserved.
	*/
	class TREEHOPPER_API Pin : public DigitalIn, public DigitalOut, public SpiChipSelectPin
	{
		friend class TreehopperUsb;

	public:
		/** Construct a new pin attached to a given board */
		Pin(TreehopperUsb* board, uint8_t pinNumber);
		
		/** Set the PinMode of the pin. */
		void mode(PinMode value);
		
		/** Get the current PinMode of the pin */
		PinMode mode();
		
		/** Make the pin a PinMode::PushPullOutput */
		void makePushPullOutput();
		
		/** Make the pin a PinMode::DigitalInput */
		void makeDigitalInput();
		
		/** Make the pin a PinMode::AnalogInput */
		void makeAnalogInput();
		
		/** Set the digital value of the pin.
		Note that if the current PinMode of the pin is not a digital output (i.e. not PinMode::PushPullOutput or PinMode::OpenDrainOutput), it will be set as PinMode::PushPullOutput before writing the value to the pin.
		*/
		void digitalValue(bool val);
		
		/** Get the digital value of the pin. */
		bool digitalValue();

		/** Toggle the output value of the pin. 
		Note that since digitalValue(bool) is ultimately called, if the current PinMode of the pin is not a digital output, it will automatically become a PinMode::PushPullOutput
		*/
		void toggleOutput();

		/** Set the AdcReferenceLevel of the pin. */
		void setReferenceLevel(AdcReferenceLevel value);

		/** Get the AdcReferenceLevel of the pin. */
		AdcReferenceLevel getReferenceLevel();

		/** Get the analog value (0-1) of the pin. */
		double analogValue();

		/** Get the voltage of the pin. */
		double analogVoltage();

		/** Get the ADC value (0-4095) of the pin */
		int adcValue();

		function<void(int)> AnalogValueChanged;
		function<void(double)> AnalogVoltageChanged;

		void SendCommand(uint8_t* data, size_t length);
		TreehopperUsb* board;
	protected:
		int _adcValue;
		double _analogValue;
		double _analogVoltage;
		virtual void updateValue(uint8_t high, uint8_t low);
		virtual void writeOutputValue();
		PinMode _mode;
		AdcReferenceLevel referenceLevel;
	};
}