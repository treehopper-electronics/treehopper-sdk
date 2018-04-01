using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Treehopper.Utilities;

namespace Treehopper
{
/** Built-in I/O pins

# Quick guide
Once you have connected to a TreehopperUsb board, you can access pins through the \link TreehopperUsb.Pins Pins\endlink property of the board. 

You can manipulate pins directly:

```
board = await ConnectionService.Instance.GetFirstDeviceAsync();
await board.ConnectAsync();
board.Pins[3].Mode = PinMode.PushPullOutput;
board.Pins[3].DigitalValue = true;
```

Or create reference variables:
```
board = await ConnectionService.Instance.GetFirstDeviceAsync();
await board.ConnectAsync();
Pin redLed = board.Pins[7];
redLed.Mode = PinMode.OpenDrainOutput;
redLed.DigitalValue = false;
```

You can choose whether a pin should be a digital input, digital output, analog input, or soft-PWM output by setting the pin's \link Pin.Mode Mode\endlink property to one of the values in PinMode.

You can set or retrieve the digital value of a pin by accessing the \link Pin.DigitalValue DigitalValue\endlink property. Note that writing to this property --- even if the pin is an input --- will implicitly force it to be an output.

If the pin is set as an analog input, you can access its data through any of the following properties:
 - \link Pin.AnalogValue AnalogValue\endlink: retrieve a normalized (0.0 - 1.0) pin value
 - \link Pin.AnalogVoltage AnalogVoltage\endlink: retrieve the voltage (0.0 - 3.3) on the pin
 - \link Pin.AdcValue AdcValue\endlink: retrieve the raw ADC value (0 - 4095) of the pin

# More information
This section dives into more details and electrical characteristics about %Treehopper's pins.

## %Pin mode
You can choose whether a pin should be a digital input, output, or analog input by setting the pin's Mode property.

## Digital outputs
All pins on %Treehopper support both push-pull and open-drain outputs. Writing a true or false to the pin's digital value will flush that value to the pin.
 - **Push-Pull**: Push-pull is the most commonly used output mode; when a pin is set to true, %Treehopper will attempt to drive the pin to logic HIGH (3.3V) — when a pin is set to false, %Treehopper will attempt to drive the pin to logic LOW (0V — ground).
 - **Open-Drain**: Open-drain outputs can only drive a strong logic LOW (0V); in the HIGH state, the pin is weakly pulled high.

### Output current limitations
%Treehopper's output impedance varies, but is roughly 100 ohm source and 50 ohm sink when supplying weaker loads, but increases as the load increases. In the worst-case scenario (when short-circuited), %Treehopper can source approximately 20 mA of current, and sink approximately 40 mA of current. The pin's drivers are rated for a maximum of 100 mA of output current, so you cannot damage the board by short-circuiting its output to ground or 3.3V.

While this is plenty of current for peripheral ICs and small indicator LEDs, do not expect to drive large arrays of LEDs, or low-impedance loads like motors, solenoids, or speakers directly from %Treehopper's pins. There are a wide variety of peripherals in the Treehopper.Libraries package for your language API that can be used for interfacing with these peripherals.

\warning **To avoid damaging the device permanently, do not source or sink more than 400 mA of combined current out of the pins on the board!** Note that these limits have nothing to do with the 3.3V supply pins found on %Treehopper, which can comfortably source 500 mA --- or the unfused 5V pin, which has no imposed current limit (other than that of your computer).

## Digital input
%Treehopper's digital inputs are used to sample digital signals — i.e., signals that have either a <i>LOW</i> or <i>HIGH</i> state. Logic LOW (false) is considered a voltage less than or equal to 0.6V. Logic HIGH (true) is considered a voltage greater than or equal to 2.7V.

%Treehopper pins are true 5V-tolerant signals; consequently, you do not need any sort of logic-level conversion or series-limiting resistor when using the pin as a digital input with a 5V source.

You can access the most recent \link Pin.DigitalValue DigitalValue\endlink, or use the \link Pin.DigitalValueChanged DigitalValueChanged\endlink event to subscribe to change notifications.

## Analog inputs
Each Treehopper pin can be read using the on-board 12-bit ADC. There is no limit to the total number of analog pins activated at any time. 

### Output Format
When the pin is sampled and sent to the host, the value is simultaneously available to the user in three forms:
 - \link Pin.AdcValue AdcValue\endlink -- the raw, 12-bit result from conversion.
 - \link Pin.AnalogValue AnalogValue\endlink -- the normalized value of the ADC (from 0-1).
 - \link Pin.AnalogVoltage AnalogVoltage\endlink -- the actual voltage at the pin (taking into account the reference level).

There are OnChanged events associated with each of these properties:
 - \link Pin.AnalogVoltageChanged AnalogVoltageChanged\endlink
 - \link Pin.AnalogValueChanged AnalogValueChanged\endlink
 - \link Pin.AdcValueChanged AdcValueChanged\endlink

Plus thresholds for each of these events that give you fine-grained control over when the event will fire:
 - \link Pin.AnalogVoltageChangedThreshold AnalogVoltageChangedThreshold\endlink
 - \link Pin.AnalogValueChangedThreshold AnalogValueChangedThreshold\endlink
 - \link Pin.AdcValueChangedThreshold AdcValueChangedThreshold\endlink

### Reference Levels
Each pin has a configurable ReferenceLevel that can be used to measure the pin against. The possible reference levels are:
 - 3.3V generated by the on-board LDO, rated at 1.5% accuracy (default).
 - 3.7V (effective) reference derived from the on-chip 1.85V reference.
 - 2.4V on-chip reference rated at 2.1% accuracy.
 - 1.85V on-chip reference.
 - 1.65V on-chip reference, 1.8% accurate.
 - 3.3V (effective) reference that is derived from the on-chip 1.65V reference.

For most ratiometric applications --- i.e., when measuring a device whose output is ratioed to its power supply --- connect the sensor's power supply to the 3.3V supply pin the %Treehopper and use the default 3.3V reference. The other reference options are provided for advanced scenarios that involve reading from precision voltage outputs accurately.

## INotifyPropertyChanged
%Pin implements INotifyPropertyChanged, and all the digital and analog value properties will also fire PropertyChanged messages.

As a result, you can directly bind GUI controls like progress bars, checkboxes, etc to %Treehopper pins in WPF, UWP, or Xamarin.Forms applications, and they will automatically update.

## A note about pin reads
All of %Treehopper's pins configured as digital or analog inputs are sampled continuously onboard; when any pin changes, this data is sent to the host device. When you access the digital or one of the analog value properties, you're accessing the last received data. This makes property reads instantaneous --- keeping your GUI or application running responsively.

For almost all applications, changes to digital or analog inputs are to be reacted to (like with switches, interrupt outputs, encoders), or sampled (like with sensor outputs). Care must be taken, however, if you need to synchronize pin reads with other functions.

For example, consider the case where you electrically short pins 0 and 1 together on the board, and then run this code:

```
var pin0 = board.Pins[0];
var pin1 = board.Pins[1];

pin0.Mode = PinMode.PushPullOutput;
pin1.Mode = PinMode.DigitalInput;

await pin0.DigitalValue = 0;
if(pin1.DigitalValue == pin0.DigitalValue)
{
    // we generally won't get here, since pin1's DigitalValue 
    // isn't explicitly read from the pin when we access it; it only returns
    // the last value read from a separate pin-reading thread
}
```

A work around is to wait for two consecutive pin updates to be received before checking the pin's value. This can be accomplished by awaiting TreehopperUsb.AwaitPinUpdateAsync(). 

However, pin updates are only sent to the computer when a pin's value changes, so if you wish to synchronously sample a pin that might not change, you should set an unused pin as an analog input, which will almost certainly guarantee a constant stream of pin updates:

```
board.Pins[19].Mode = PinMode.AnalogInput; // this will ensure we get continuous pin updates

var pin0 = board.Pins[0];
var pin1 = board.Pins[1];

pin0.Mode = PinMode.PushPullOutput;
pin1.Mode = PinMode.DigitalInput;

await pin0.DigitalValue = 0;
await board.AwaitPinUpdateAsync(); // this first report may have been captured before the output was written
await board.AwaitPinUpdateAsync(); // this report should have the effects of the digital output in it
if(pin1.DigitalValue == pin0.DigitalValue)
{
    // we should always get here
}
```

# SoftPWM functionality
Each %Treehopper pin can be used as a SoftPWM pin.

# Performance Considerations
Writing values to (or changing pin modes of) Treehopper pins will flush to the OS's USB layer immediately, but there is no way of achieving guaranteed latency. 

Occasional writes (say, on the order of every 20 ms or more) will usually flush to the port within a few hundred microseconds. If your application is chatty, or the bus you're operating on has other devices (especially isochronous devices like webcams), you may see long periods (a millisecond or more) of delay. 

Analog pins take a relatively long time to sample; if you enable tons of analog inputs, the effective sampling rate will drop by up to two times.

*/
    public class Pin : INotifyPropertyChanged, DigitalIn, DigitalOut, AdcPin, SpiChipSelectPin, Pwm
    {
        private int _adcValue;
        private TaskCompletionSource<int> _adcValueSignal = new TaskCompletionSource<int>();
        private TaskCompletionSource<double> _analogValueSignal = new TaskCompletionSource<double>();
        private TaskCompletionSource<double> _analogVoltageSignal = new TaskCompletionSource<double>();
        private TaskCompletionSource<bool> _digitalSignal = new TaskCompletionSource<bool>();
        private bool _digitalValue;
        private PinMode _mode = PinMode.Unassigned;
        private int _prevAdcValue;
        private double _prevAnalogValue;
        private double _prevAnalogVoltage;
        private AdcReferenceLevel _referenceLevel;
        private double _referenceLevelVoltage;

        internal Pin(TreehopperUsb board, byte pinNumber)
        {
            Board = board;
            PinNumber = pinNumber;
            ReferenceLevel = AdcReferenceLevel.Vref_3V3;
            Name = "Pin " + pinNumber; // default name
        }


        /** @name Core components 
            @{
        */
        /// <summary>
        ///     Get or set the mode of the pin.
        /// </summary>
        public PinMode Mode
        {
            get { return _mode; }

            set
            {
                if (value == _mode)
                    return;
                if (_mode == PinMode.Reserved && value != PinMode.Unassigned)
                    throw new Exception(
                        "This pin is reserved; you must disable the peripheral using it before interacting with it");

                _mode = value;

                switch (_mode)
                {
                    case PinMode.AnalogInput:
                        if (TreehopperUsb.Settings.PropertyWritesReturnImmediately)
                            MakeAnalogInAsync().Forget();
                        else
                            Task.Run(MakeAnalogInAsync).Wait();
                        break;

                    case PinMode.DigitalInput:
                        if (TreehopperUsb.Settings.PropertyWritesReturnImmediately)
                            MakeDigitalInAsync().Forget();
                        else
                            Task.Run(MakeDigitalInAsync).Wait();
                        break;

                    case PinMode.OpenDrainOutput:
                        if (TreehopperUsb.Settings.PropertyWritesReturnImmediately)
                            MakeDigitalOpenDrainOutAsync().Forget();
                        else
                            Task.Run(MakeDigitalOpenDrainOutAsync).Wait();

                        _digitalValue = false; // set initial state
                        break;

                    case PinMode.PushPullOutput:
                        if (TreehopperUsb.Settings.PropertyWritesReturnImmediately)
                            MakeDigitalPushPullOutAsync().Forget();
                        else
                            Task.Run(MakeDigitalPushPullOutAsync).Wait();

                        _digitalValue = false; // set initial state
                        break;

                    case PinMode.SoftPwm:
                        if (TreehopperUsb.Settings.PropertyWritesReturnImmediately)
                            EnablePwmAsync().Forget();
                        else
                            Task.Run(EnablePwmAsync).Wait();
                        break;
                }
            }
        }

        /// <summary>
        /// Gets the name of the pin
        /// </summary>
        /**
        This is a useful property when binding pins in drop-down lists and other GUI controls. Pin names include the number and special function (if any) the pin has.
         */
        public string Name { get; internal set; }

        /// <summary>
        ///     This event fires whenever a property changes
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        ///@}




        /** @name Digital components
            @{
        */

        /// <summary>
        ///     Gets or sets the digital value of the pin.
        /// </summary>
        /** 
        Setting a value to a pin that is configured as an input will automatically make the pin an output before writing the value to it.

        The value retrieved from this pin will read as "0" when then pin is being used for other purposes.
         */
        public bool DigitalValue
        {
            get
            {
                if (Mode == PinMode.Reserved || Mode == PinMode.AnalogInput || Mode == PinMode.Unassigned)
                    Debug.WriteLine(
                        $"NOTICE: Pin {PinNumber} must be in digital I/O mode to read from. This call will always return 0.");

                return _digitalValue;
            }

            set
            {
                if (_digitalValue == value) return;
                _digitalValue = value;
                if (TreehopperUsb.Settings.PropertyWritesReturnImmediately)
                    WriteDigitalValueAsync(_digitalValue).Forget(); // send off the request and move on.
                else
                    WriteDigitalValueAsync(_digitalValue).Wait(); // wait for it to complete
            }
        }

        /// <summary>
        ///     Wait for the digital input value of the pin to change
        /// </summary>
        /// <returns>An awaitable bool, indicating the pin's state</returns>
        public Task<bool> AwaitDigitalValueChangeAsync()
        {
            _digitalSignal = new TaskCompletionSource<bool>();
            return _digitalSignal.Task;
        }

        /// <summary>
        ///     Toggles the output value of the pin.
        /// </summary>
        /** 
        Calling this function on a pin that is configured as an input will automatically make the pin an output before writing the value to it.

        In this example, an LED attached to pin 4 is made to blink.

        ```
        Pin led = myTreehopperBoard.Pin4; // create a reference to Pin4 to keep code concise.
        led.MakeDigitalOutput();
        while(true)
        {
            led.Toggle();
            Thread.Sleep(500);
        }
        ```

         */
        public Task ToggleOutputAsync()
        {
            return WriteDigitalValueAsync(!DigitalValue);
        }

        /// <summary>
        ///     Occurs when the input on the pin changes.
        /// </summary>
        /// <remarks>
        ///     This event will only fire when the pin is configured as a digital input.
        /// </remarks>
        public event OnDigitalInValueChanged DigitalValueChanged;

        /// <summary>
        ///     Write a value to a pin
        /// </summary>
        /// <param name="value">The value to write</param>
        /// <returns>An awaitable task that completes upon success</returns>
        /// <remarks>
        ///     <para>
        ///         This method is functionally equivalent of calling "<see cref="DigitalValue" />=value", however, it provides
        ///         finer-grained control over asynchronous behavior. You may choose to await it, block it synchronously, or
        ///         "forget" it (continue execution without waiting at all).
        ///     </para>
        /// </remarks>
        public async Task WriteDigitalValueAsync(bool value)
        {
            _digitalValue = value;
            if (!(Mode == PinMode.PushPullOutput || Mode == PinMode.OpenDrainOutput))
                await MakeDigitalPushPullOutAsync().ConfigureAwait(false); // assume they want push-pull

            var byteVal = (byte) (_digitalValue ? 0x01 : 0x00);
            await SendCommandAsync(new[] {(byte) PinConfigCommands.SetDigitalValue, byteVal}).ConfigureAwait(false);
        }

        /// <summary>
        ///     Make the pin a digital input.
        /// </summary>
        public Task MakeDigitalInAsync()
        {
            _mode = PinMode.DigitalInput;
            return SendCommandAsync(new byte[] {(byte) PinConfigCommands.MakeDigitalInput, 0});
        }

        /// <summary>
        ///     Make the pin a push-pull output.
        /// </summary>
        public Task MakeDigitalPushPullOutAsync()
        {
            _mode = PinMode.PushPullOutput;
            return SendCommandAsync(new byte[] {(byte) PinConfigCommands.MakePushPullOutput, 0});
        }

        /// <summary>
        ///     Make the pin a push-pull output.
        /// </summary>
        public Task MakeDigitalOpenDrainOutAsync()
        {
            _mode = PinMode.OpenDrainOutput;
            return SendCommandAsync(new byte[] {(byte) PinConfigCommands.MakeOpenDrainOutput, 0});
        }
       
        ///@}


        /** @name AnalogValue components
            @{
        */
        /// <summary>
        ///     Retrieve the last reading from the ADC, expressed on a unit range (0.0 - 1.0)
        /// </summary>
        /// <remarks>
        /// </remarks>
        public double AnalogValue
        {
            get
            {
                return Math.Round(AdcValue / 4092.0, 4);
            }
        } 

        /// <summary>
        ///     Occurs when an analog value is changed, according to the set threshold.
        /// </summary>
        /// <remarks>
        ///     The Changed event is raised when the 10-bit ADC value obtained is different from the previous reading
        ///     by at least the value specified by <see cref="AdcValueChangedThreshold" />
        /// </remarks>
        public event OnAnalogValueChanged AnalogValueChanged;

        /// <summary>
        ///     Wait for the pin's analog value to change.
        /// </summary>
        /// <returns>An awaitable double of the analog value, normalized from 0-1.</returns>
        public Task<double> AwaitAnalogValueChangeAsync()
        {
            _analogValueSignal = new TaskCompletionSource<double>();
            return _analogValueSignal.Task;
        }

        /// <summary>
        ///     Gets or sets the value threshold required to fire the AnalogValueChanged event.
        /// </summary>
        public double AnalogValueChangedThreshold { get; set; } = 0.01;

        ///@}

        /** @name AnalogVoltage components
            @{
        */
        /// <summary>
        ///     Retrieve the last voltage reading from the ADC.
        /// </summary>
        public double AnalogVoltage
        {
            get
            {
                return Math.Round(AdcValue * (_referenceLevelVoltage / 4092.0), 4);
            }
        } 

        /// <summary>
        ///     Occurs when an analog voltage is changed, according to the set threshold.
        /// </summary>
        /// <remarks>
        ///     The Changed event is raised when the 12-bit ADC value obtained is different from the previous reading
        ///     by at least the value specified by <see cref="AnalogVoltageChangedThreshold" />.
        /// </remarks>
        public event OnAnalogVoltageChanged AnalogVoltageChanged;

        /// <summary>
        ///     Wait for the pin's analog voltage to change.
        /// </summary>
        /// <returns>An awaitable double of the pin's analog voltage, measured in volts.</returns>
        public Task<double> AwaitAnalogVoltageChangeAsync()
        {
            _analogVoltageSignal = new TaskCompletionSource<double>();
            return _analogVoltageSignal.Task;
        }

        /// <summary>
        ///     Gets or sets the voltage threshold required to fire the AnalogVoltageChanged event.
        /// </summary>
        public double AnalogVoltageChangedThreshold { get; set; } = 0.05;

        ///@}

        /** @name AdcValue components
            @{
        */

        /// <summary>
        ///     Retrieve the last value obtained from the ADC.
        /// </summary>
        /// <remarks>
        ///     Treehopper has a 12-bit ADC, so ADC values will range from 0-4095.
        /// </remarks>
        public int AdcValue
        {
            get
            {
                if (Mode != PinMode.AnalogInput)
                    Debug.WriteLine(
                        $"NOTICE: Attempting to read AdcValue from Pin {PinNumber}, which is configured for {Mode}. This call will always return 0");

                return _adcValue;
            }
        }

        /// <summary>
        ///     Occurs when the normalized analog value is changed, according to the set threshold.
        /// </summary>
        /// <remarks>
        ///     The Changed event is raised when the 10-bit ADC value obtained is different from the previous reading.
        /// </remarks>
        public event OnAdcValueChanged AdcValueChanged;

        /// <summary>
        ///     Wait for the pin's ADC value to change.
        /// </summary>
        /// <returns>An awaitable int, in the range of 0-4095, of the pin's ADC value.</returns>
        public Task<int> AwaitAdcValueChangeAsync()
        {
            _adcValueSignal = new TaskCompletionSource<int>();
            return _adcValueSignal.Task;
        }

        /// <summary>
        ///     Gets or sets the value threshold required to fire the AdcValueChanged event.
        /// </summary>
        public int AdcValueChangedThreshold { get; set; } = 10;

        /// @}
        /** @name Core analog components
            @{
        */
        /// <summary>
        ///     Sets the ADC reference value used
        /// </summary>
        public AdcReferenceLevel ReferenceLevel
        {
            get { return _referenceLevel; }

            set
            {
                _referenceLevel = value;
                switch (_referenceLevel)
                {
                    case AdcReferenceLevel.Vref_1V65:
                        _referenceLevelVoltage = 1.65;
                        break;
                    case AdcReferenceLevel.Vref_1V85:
                        _referenceLevelVoltage = 1.8;
                        break;
                    case AdcReferenceLevel.Vref_2V4:
                        _referenceLevelVoltage = 2.4;
                        break;
                    case AdcReferenceLevel.Vref_3V3:
                        _referenceLevelVoltage = 3.3;
                        break;
                    case AdcReferenceLevel.Vref_3V3Derived:
                        _referenceLevelVoltage = 3.3;
                        break;
                    case AdcReferenceLevel.Vref_3V7:
                        _referenceLevelVoltage = 3.6;
                        break;
                }

                // if we're already an analog input, re-send the command to set the new reference level
                if (Mode == PinMode.AnalogInput)
                    SendCommandAsync(new[] {(byte) PinConfigCommands.MakeAnalogInput, (byte) ReferenceLevel});
            }
        }

        /// <summary>
        ///     Make the pin an analog input.
        /// </summary>
        public Task MakeAnalogInAsync()
        {
            _mode = PinMode.AnalogInput;
            return SendCommandAsync(new[] {(byte) PinConfigCommands.MakeAnalogInput, (byte) ReferenceLevel});
        }

        /// @}


        /** @name SoftPWM components
            @{
        */

        public double DutyCycle
        {
            get => Board.SoftPwmMgr.GetDutyCycle(this);
            set => Task.Run(() => Board.SoftPwmMgr.SetDutyCycleAsync(this, value)).Wait();
        }

        public double PulseWidth
        {
            get => Board.SoftPwmMgr.GetPulseWidth(this);
            set => Task.Run(() => Board.SoftPwmMgr.SetPulseWidthAsync(this, value)).Wait();
        }

        public async Task EnablePwmAsync()
        {
            _mode = PinMode.SoftPwm;
            await SendCommandAsync(new byte[] { (byte)PinConfigCommands.MakePushPullOutput, 0 }).ConfigureAwait(false);
            await Board.SoftPwmMgr.StartPinAsync(this).ConfigureAwait(false);
        }

        public async Task DisablePwmAsync()
        {
            _mode = PinMode.DigitalInput;
            await Board.SoftPwmMgr.StopPinAsync(this).ConfigureAwait(false);
            await SendCommandAsync(new byte[] { (byte)PinConfigCommands.MakeDigitalInput, 0 }).ConfigureAwait(false);
        }

        /// @}


        /** @name Other components 
            @{
        */

        /// <summary>
        ///     The pin number of the pin.
        /// </summary>
        public int PinNumber { get; internal set; }

        /// <summary>
        ///     This returns a reference to the Treehopper board this pin belongs to.
        /// </summary>
        public TreehopperUsb Board { get; }


        /// <summary>
        ///     Gets the Spi module that can use this pin for chip-select duties
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Since the SPI chip-select functionality is done in-hardware, the SPI module must check to ensure the pin
        ///         you're using for chip-select actually belongs to the same board as the SPI module does (as you may have
        ///         multiple boards attached).
        ///     </para>
        /// </remarks>
        public Spi SpiModule
        {
            get
            {
                return Board.Spi;
            }
        }

        /// <summary>
        ///     Gets a string representation of the pin's current state
        /// </summary>
        /// <returns>the pin's current state</returns>
        public override string ToString()
        {
            switch (Mode)
            {
                case PinMode.AnalogInput:
                    return $"{Name}: Analog input, {AnalogVoltage:0.00} volts";
                case PinMode.DigitalInput:
                    return $"{Name}: Digital input, {DigitalValue}";
                case PinMode.OpenDrainOutput:
                    return $"{Name}: Open-drain output, {DigitalValue}";
                case PinMode.PushPullOutput:
                    return $"{Name}: Push-pull output, {DigitalValue}";
                case PinMode.SoftPwm:
                    return $"{Name}: {DutyCycle * 100:0.00}% duty cycle ({PulseWidth:0.00} us pulse width)";
                case PinMode.Reserved:
                    return $"{Name}: In use by peripheral";

                default:
                    return Name + ": Unassigned";
            }
        }

        ///@}


        internal virtual void UpdateValue(byte highByte, byte lowByte)
        {
            if (Mode == PinMode.DigitalInput)
            {
                var newVal = highByte > 0;
                if (_digitalValue != newVal)
                {
                    // we have a new value!
                    _digitalValue = newVal;
                    RaiseDigitalInValueChanged();
                    RaisePropertyChanged("DigitalValue");
                }
            }
            else if (Mode == PinMode.AnalogInput)
            {
                var val = highByte << 8;
                val |= lowByte;
                _adcValue = val;
                RaiseAnalogInChanged();
            }
        }

        internal void RaisePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        internal void RaiseDigitalInValueChanged()
        {
            DigitalValueChanged?.Invoke(this, new DigitalInValueChangedEventArgs(_digitalValue));

            _digitalSignal.TrySetResult(_digitalValue);
        }

        internal void RaiseAnalogInChanged()
        {
            if (!_prevAdcValue.CloseTo(_adcValue, AdcValueChangedThreshold))
            {
                _prevAdcValue = _adcValue;
                AdcValueChanged?.Invoke(this, new AdcValueChangedEventArgs(_adcValue));

                _adcValueSignal.TrySetResult(_adcValue);

                RaisePropertyChanged("AdcValue");
            }

            if (!_prevAnalogVoltage.CloseTo(AnalogVoltage, AnalogVoltageChangedThreshold))
            {
                _prevAnalogVoltage = AnalogVoltage;
                AnalogVoltageChanged?.Invoke(this, new AnalogVoltageChangedEventArgs(AnalogVoltage));

                _analogVoltageSignal.TrySetResult(AnalogVoltage);

                RaisePropertyChanged("AnalogVoltage");
            }

            if (!_prevAnalogValue.CloseTo(AnalogValue, AnalogValueChangedThreshold))
            {
                _prevAnalogValue = AnalogValue;
                AnalogValueChanged?.Invoke(this, new AnalogValueChangedEventArgs(AnalogValue));

                _analogValueSignal.TrySetResult(AnalogValue);

                RaisePropertyChanged("AnalogValue");
            }
        }

        internal Task SendCommandAsync(byte[] cmd)
        {
            var data = new byte[6];
            data[0] = (byte) PinNumber;
            cmd.CopyTo(data, 1);
            return Board.SendPinConfigPacketAsync(data);
        }



        internal enum PinConfigCommands
        {
            Reserved = 0,
            MakeDigitalInput,
            MakePushPullOutput,
            MakeOpenDrainOutput,
            MakeAnalogInput,
            SetDigitalValue
        }
    }
}