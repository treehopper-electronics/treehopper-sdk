#pragma once

#include "Treehopper.h"
#include <stdint.h>
#include <functional>
#include "DigitalIn.h"
#include "DigitalOut.h"
#include "Event.h"
#include "SpiChipSelectPin.h"
#include "AdcPin.h"

using namespace std;

namespace Treehopper {
    class TreehopperUsb;

    enum class AdcReferenceLevel {
        VREF_3V3,
        VREF_1V65,
        VREF_1V8,
        VREF_2V4,
        VREF_3V3_DERIVED,
        VREF_3V6
    };

    enum class PinMode {
        Reserved,
        DigitalInput,
        PushPullOutput,
        OpenDrainOutput,
        AnalogInput,
        Unassigned
    };

/** Built-in I/O pins

# Quick guide
Once you have connected to a TreehopperUsb board, you can access pins through the \link TreehopperUsb.pins pins\endlink collection on the board.

You can manipulate pins directly:

```
auto board = ConnectionService::instance().getFirstDevice();
board.connect();
board.pins[3].mode(PinMode::PushPullOutout);
board.pins[3].digitalValue(true);
```

Or create reference variables:
```
auto board = ConnectionService::instance().getFirstDevice();
board.connect();
Pin redLed = board.pins[7];
redLed.mode(PinMode::OpenDrainOutput);
redLed.digitalValue(false);
```

You can choose whether a pin should be a digital input, digital output, analog input, or soft-PWM output by calling the pin's mode() property to one of the values in \link Treehopper::PinMode PinMode\endlink.

You can set or retrieve the digital value of a pin by accessing the digitalValue() property. Note that setting this property --- even if the pin is an input --- will implicitly force it to be an output.

If the pin is set as an analog input, you can access its data through any of the following properties:
 - \link Pin.analogValue analogValue\endlink: retrieve a normalized (0.0 - 1.0) pin value
 - \link Pin.analogVoltage analogVoltage\endlink: retrieve the voltage (0.0 - 3.3) on the pin
 - \link Pin.adcValue adcValue\endlink: retrieve the raw ADC value (0 - 4095) of the pin

# More information
This section dives into more details and electrical characteristics about %Treehopper's pins.

## %Pin mode
You can choose whether a pin should be a digital input, output, or analog input by setting the pin's mode() property.

## Digital outputs

All pins on %Treehopper support both push-pull and open-drain outputs. Writing a true or false to the pin's digital value will flush that value to the pin.
 - **Push-Pull**: Push-pull is the most commonly used output mode; when a pin is set to true, %Treehopper will attempt to drive the pin to logic HIGH (3.3V) — when a pin is set to false, %Treehopper will attempt to drive the pin to logic LOW (0V — ground).
 - **Open-Drain**: Open-drain outputs can only drive a strong logic LOW (0V); in the HIGH state, the pin is weakly pulled high.

### Output current limitations

%Treehopper's output impedance varies, but is roughly 100 ohm source and 50 ohm sink when supplying weaker loads, but increases as the load increases. In the worst-case scenario (when short-circuited), %Treehopper can source approximately 20 mA of current, and sink approximately 40 mA of current. The pin's drivers are rated for a maximum of 100 mA of output current, so you cannot damage the board by short-circuiting its output to ground or 3.3V.

While this is plenty of current for peripheral ICs and small indicator LEDs, do not expect to drive large arrays of LEDs, or low-impedance loads like motors, solenoids, or speakers directly from %Treehopper's pins. There are a wide variety of peripherals in the Treehopper::Libraries package for your language API that can be used for interfacing with these peripherals.

\warning **To avoid damaging the device permanently, do not source or sink more than 400 mA of combined current out of the pins on the board!** Note that these limits have nothing to do with the 3.3V supply pins found on %Treehopper, which can comfortably source 500 mA --- or the unfused 5V pin, which has no imposed current limit (other than that of your computer or smartphone).

## Digital input

%Treehopper's digital inputs are used to sample digital signals — i.e., signals that have either a <i>LOW</i> or <i>HIGH</i> state. Logic LOW (false) is considered a voltage less than or equal to 0.6V. Logic HIGH (true) is considered a voltage greater than or equal to 2.7V.

%Treehopper pins are true 5V-tolerant signals; consequently, you do not need any sort of logic-level conversion or series-limiting resistor when using the pin as a digital input with a 5V source.

You can access the most recent \link Pin.digitalValue() digitalValue()\endlink, or use the \link Pin.digitalValueChanged digitalValueChanged\endlink event to subscribe to change notifications.

## Analog inputs

Each Treehopper pin can be read using the on-board 12-bit ADC. There is no limit to the total number of analog pins activated at any time.

### Output Format

When the pin is sampled and sent to the host, the value is simultaneously available to the user in three forms:
 - \link Pin.adcValue adcValue\endlink -- the raw, 12-bit result from conversion.
 - \link Pin.analogValue analogValue\endlink -- the normalized value of the ADC (from 0-1).
 - \link Pin.analogVoltage analogVoltage\endlink -- the actual voltage at the pin (taking into account the reference level).

There are OnChanged events associated with each of these properties:
 - \link Pin.analogVoltageChanged analogVoltageChanged\endlink
 - \link Pin.analogValueChanged analogValueChanged\endlink
 - \link Pin.adcValueChanged adcValueChanged\endlink

### Reference Levels

Each pin has a configurable Treehopper::ReferenceLevel that can be used to measure the pin against. The possible reference levels are:
 - 3.3V generated by the on-board LDO, rated at 1.5% accuracy (default).
 - 3.7V (effective) reference derived from the on-chip 1.85V reference.
 - 2.4V on-chip reference rated at 2.1% accuracy.
 - 1.85V on-chip reference.
 - 1.65V on-chip reference, 1.8% accurate.
 - 3.3V (effective) reference that is derived from the on-chip 1.65V reference.

For most ratiometric applications --- i.e., when measuring a device whose output is ratioed to its power supply --- connect the sensor's power supply to the 3.3V supply pin the %Treehopper and use the default 3.3V reference. The other reference options are provided for advanced scenarios that involve reading from precision voltage outputs accurately.

## A note about pin reads

All of %Treehopper's pins configured as digital or analog inputs are sampled continuously onboard; when any pin changes, this data is sent to the host device. When you access the digital or one of the analog value properties, you're accessing the last received data. This makes property reads instantaneous --- keeping your GUI or application running responsively.

For almost all applications, changes to digital or analog inputs are to be reacted to (like with switches, interrupt outputs, encoders), or sampled (like with sensor outputs). Care must be taken, however, if you need to synchronize pin reads with other functions.

For example, consider the case where you electrically short pins 0 and 1 together on the board, and then run this code:

```
auto pin0 = board.pins[0];
auto pin1 = board.pins[1];

pin0.mode(PinMode::PushPullOutput);
pin1.mode(PinMode::DigitalInput);

pin0.digitalValue(true);
if(pin1.digitalValue() == pin0.digitalValue())
{
    // we generally won't get here, since pin1's digitalValue()
    // isn't explicitly read from the pin when we access it; it only returns
    // the last value read from a separate pin-reading thread
}
```
# SoftPWM functionality

Each %Treehopper pin can be used as a SoftPWM pin.

# Performance Considerations

Writing values to (or changing pin modes of) Treehopper pins will flush to the OS's USB layer immediately, but there is no way of achieving guaranteed latency.

Occasional writes (say, on the order of every 20 ms or more) will usually flush to the port within a few hundred microseconds. If your application is chatty, or the bus you're operating on has other devices (especially isochronous devices like webcams), you may see long periods (a millisecond or more) of delay.

Analog pins take a relatively long time to sample; if you enable tons of analog inputs, the effective sampling rate will drop by up to two times.

*/
    class TREEHOPPER_API Pin : public DigitalIn, public AdcPin, public SpiChipSelectPin {
        friend class TreehopperUsb;

    public:
        /** Construct a new pin attached to a given board */
        Pin(TreehopperUsb *board, uint8_t pinNumber);

        /** Set the PinMode of the pin. */
        void mode(PinMode value);

        /** Get the current PinMode of the pin */
        PinMode mode();

        /** Make the pin a PinMode::PushPullOutput */
        void makePushPullOutput() override;

        /** Make the pin a PinMode::DigitalInput */
        void makeDigitalInput() override;

        /** Make the pin a PinMode::AnalogInput */
        void makeAnalogInput();

        /** Set the digital value of the pin.
        Note that if the current PinMode of the pin is not a digital output (i.e. not PinMode::PushPullOutput or PinMode::OpenDrainOutput), it will be set as PinMode::PushPullOutput before writing the value to the pin.
        */
        void digitalValue(bool val) override;

        /** Get the digital value of the pin. */
        bool digitalValue() override;

        /** Toggle the output value of the pin.
        Note that since digitalValue(bool) is ultimately called, if the current PinMode of the pin is not a digital output, it will automatically become a PinMode::PushPullOutput
        */
        void toggleOutput();

        /** Set the AdcReferenceLevel of the pin. */
        void referenceLevel(AdcReferenceLevel value);

        /** Get the AdcReferenceLevel of the pin. */
        AdcReferenceLevel referenceLevel();

        /** Get the analog value (0-1) of the pin. */
        double analogValue();

        /** Get the voltage of the pin. */
        double analogVoltage();

        /** Get the ADC value (0-4095) of the pin */
        int adcValue();

        /** Fires when the analog value (0.0-1.0) changes. */
        Event<Pin, AnalogValueChangedEventArgs> analogValueChanged;

        /** Fires when the analog voltage (0.0-3.6) changes. */
        Event<Pin, AnalogVoltageChangedEventArgs> analogVoltageChanged;

        /** Fires when the ADC value (0-4091) changes. */
        Event<Pin, AdcValueChangedEventArgs> adcValueChanged;

    protected:
        void SendCommand(uint8_t *data, size_t length);

        TreehopperUsb *board;

        int _adcValue;
        double _analogValue;
        double _analogVoltage;

        virtual void updateValue(uint8_t high, uint8_t low);

        virtual void writeOutputValue() override;

        PinMode _mode;
        AdcReferenceLevel _referenceLevel;
    };
}