\page pins Digital & Analog Pins
Treehopper boards have up to 20 pins — each of which can be used as analog inputs, digital inputs, or digital outputs.

# Pin Mode
You can choose whether a pin should be a digital input, output, or analog input by setting the pin's mode property. Consult the API documentation for the language you're using to determine how to set the pin mode.

## Digital outputs
All pins on Treehopper support both push-pull and open-drain outputs. Writing a true or false to the pin's digital value will flush that value to the pin.
## Push-Pull
Push-pull is the commonly used output mode; when a pin is set to true, Treehopper will attempt to drive the pin to logic HIGH (3.3V) — when a pin is set to false, Treehopper will attempt to drive the pin to logic LOW (0V — also referred to as ground).
## Open-Drain
Open-drain outputs can only drive a strong logic LOW (0V); in the other state, the pin is allowed to float.
## Output Current Limitations
Treehopper can source approximately 20 mA of current out of each pin when short-circuited. Treehopper can sink approximately 40 mA of current into each pin when short-circuited. While this is plenty of current for peripheral ICs and small indicator LEDs, do not expect to drive large arrays of LEDs, or low-impedance loads like motors, solenoids, or speakers directly from Treehopper's pins. There are a wide variety of peripherals in the Treehopper.Libraries namespace that can be used for interfacing with these peripherals.
\warning <b>To avoid damaging the device permanently, do not source or sink more than 400 mA of combined current out of the pins on the board!</b> Note that these limits have nothing to do with the supply pins found on Treehopper; you can comfortably source 500 mA out of either the 5V or 3.3V supply pins on the board.
## Digital input {#pins_digitalin}
Treehopper's digital inputs are used to sample digital signals — i.e., signals that have either a <i>LOW</i> or <i>HIGH</i> state. A <i>LOW</i> is considered a voltage less than or equal to 0.6V. Logic <b>HIGH</b> is considered a voltage greater than or equal to 2.7V.

Treehopper pins are 5V-tolerant; note that this means there is a 5V-capable protection diode shunting to VDD (3.3V) -- it does not mean you can drive a low-impedance (strong) 5V signal into a pin; if you have 5V outputs from peripherals or other devices that you wish to use, please place a resistor in series with the signal to limit current:
![Use a series resistor between 5V outputs and Treehopper input pins to limit current](images/pins-current-limiting.svg)

\warning <b>Do not exceed 5.8V on any pin at any time or you will permanently damage the board</b>.

Treehopper digital pins are sampled continuously onboard; when any pin changes, this data is sent to the host device. 

## Analog inputs
Test

# Performance Considerations
Writing values to (or changing pin modes of) Treehopper pins will flush to the OS's USB layer immediately, but there is no way of achieving guaranteed latency. Occasional writes (say, on the order of every 20 ms or more) will usually flush to the port within 0.1-1 ms. If your application is chatty (for example, toggling a pin continuously in a while() loop), expect several cycles of fast performance, followed by occasionally hangs (up to 6-10 ms has been observed).