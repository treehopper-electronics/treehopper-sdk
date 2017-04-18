using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Treehopper.Utilities;

namespace Treehopper.Libraries.ArduinoShim
{
    /// <summary>
    ///     Base class that Arduino-like sketches can extend.
    /// </summary>
    public abstract class Sketch
    {
        /// <summary>
        ///     Input mode for pins.
        /// </summary>
        public const int INPUT = 1;

        /// <summary>
        ///     Output mode for pins.
        /// </summary>
        public const int OUTPUT = 2;

        /// <summary>
        ///     Input, with pullup, for pins.
        /// </summary>
        public const int INPUT_PULLUP = 3;

        /// <summary>
        ///     Logic high: 3.3V on most boards.
        /// </summary>
        public const bool HIGH = true;

        /// <summary>
        ///     Logic low: 0V on most boards.
        /// </summary>
        public const bool LOW = false;

        /// <summary>
        ///     ledPin is pre-defined for use with digitalWrite().
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         To avoid bus contention, Treehopper's LED pin is not attached to a
        ///         user-accessible GPIO pin, so <see cref="int.MaxValue" /> is used as a hack. The
        ///         <see cref="digitalWrite(int, bool)" />
        ///         function intercepts this value and directs write operations to the LED.
        ///     </para>
        /// </remarks>
        public const int ledPin = int.MaxValue;

        /// <summary>
        ///     Shift data MSB-first.
        /// </summary>
        public const bool MSBFIRST = true;

        /// <summary>
        ///     Shift data LSB-first.
        /// </summary>
        public const bool LSBFIRST = false;

        private readonly Stopwatch sw = new Stopwatch();
        private readonly bool throwExceptions;
        private int adcResolution = 10;

        private int pwmResolution = 8;

        /// <summary>
        ///     The Serial port to use with this sketch.
        /// </summary>
        public SerialShim Serial;

        /// <summary>
        ///     Initialize and run a sketch
        /// </summary>
        /// <param name="board">The Treehopper board to use</param>
        /// <param name="throwExceptions">Whether unimplemented or miscalled functions should throw exceptions or fail silently</param>
        public Sketch(TreehopperUsb board, bool throwExceptions = true)
        {
            Board = board;
            this.throwExceptions = throwExceptions;
            board.ConnectAsync().Wait();

            Serial = new SerialShim(board);
        }

        /// <summary>
        ///     The board to use with this sketch.
        /// </summary>
        public TreehopperUsb Board { get; }

        /// <summary>
        ///     Run the sketch synchronously
        /// </summary>
        public void Run()
        {
            sw.Start();
            setup();
            while (true)
                loop();
        }

        /// <summary>
        ///     Implement this function in your sketch to provide one-time setup functionality
        /// </summary>
        public abstract void setup();

        /// <summary>
        ///     Implement this function in your sketch that gets called repeatedly until the program exits.
        /// </summary>
        public abstract void loop();


        /// <summary>
        ///     Configures the specified pin to behave either as an input or an output.
        /// </summary>
        /// <param name="pin">the number of the pin whose mode you wish to set</param>
        /// <param name="mode">
        ///     INPUT, OUTPUT, or INPUT_PULLUP. Note that Treehopper inputs are always weakly pulled-up, so there is
        ///     no difference between INPUT and INPUT_PULLUP
        /// </param>
        public void pinMode(int pin, int mode)
        {
            if (pin == ledPin)
                return; // LED is always an output

            if (!throwExceptions && (pin >= Board.NumberOfPins || pin <= 0)) // fail silently
                return;

            switch (mode)
            {
                case INPUT:
                    Board.Pins[pin].Mode = PinMode.DigitalInput;
                    break;
                case OUTPUT:
                    Board.Pins[pin].Mode = PinMode.PushPullOutput;
                    break;

                case INPUT_PULLUP:
                    Board.Pins[pin].Mode = PinMode.DigitalInput;
                    break;
            }
        }

        /// <summary>
        ///     Write a HIGH or a LOW value to a digital pin. To set the LED, use "LedPin"
        /// </summary>
        /// <param name="pin">the pin number (or ledPin)</param>
        /// <param name="value">HIGH or LOW</param>
        public void digitalWrite(int pin, bool value)
        {
            if (pin == ledPin)
            {
                Board.Led = value;
                return;
            }

            if (!throwExceptions && (pin >= Board.NumberOfPins || pin <= 0)) // fail silently
                return;

            if (Board.Pins[pin].Mode == PinMode.PushPullOutput)
                Board.Pins[pin].DigitalValue = value;
        }

        /// <summary>
        ///     Reads the value from a specified digital pin, either HIGH or LOW.
        /// </summary>
        /// <param name="pin">the number of the digital pin you want to read (int)</param>
        /// <returns>bool true or false (can compare with HIGH or LOW)</returns>
        public bool digitalRead(int pin)
        {
            if (pin == ledPin)
                return Board.Led;

            if (!throwExceptions && (pin >= Board.NumberOfPins || pin <= 0)) // fail silently
                return false;

            if (Board.Pins[pin].Mode == PinMode.DigitalInput)
                return Board.Pins[pin].DigitalValue;

            return false; // some other state
        }

        /// <summary>
        ///     Reads the value from the specified analog pin.
        /// </summary>
        /// <param name="pin">the number of the analog input pin to read from</param>
        /// <returns>int (0 to 2^analogReadResolution -- 1024 by default)</returns>
        public int analogRead(int pin)
        {
            if (!throwExceptions && (pin >= Board.NumberOfPins || pin <= 0)) // fail silently
                return 0;

            Board.Pins[pin].Mode = PinMode.AnalogInput;
            return (int) Math.Round(Board.Pins[pin].AdcValue * (Math.Pow(2, adcResolution) / 4096.0));
        }

        /// <summary>
        ///     Writes a PWM value to a pin. Uses zero-jitter hardware PWM on pins 7, 9, and 9. Uses software PWM on all other
        ///     pins.
        /// </summary>
        /// <param name="pin">the pin to write to</param>
        /// <param name="value">the duty cycle between 0 (always off) and 2^analogWriteResolution (defaults to 255) (always on)</param>
        public void analogWrite(int pin, int value)
        {
            if (!throwExceptions && (pin >= Board.NumberOfPins || pin <= 0)) // fail silently
                return;

            if (pin == 7)
            {
                Board.Pwm1.Enabled = true;
                Board.Pwm1.DutyCycle = value / Math.Pow(2, pwmResolution);
            }
            else if (pin == 8)
            {
                Board.Pwm2.Enabled = true;
                Board.Pwm2.DutyCycle = value / Math.Pow(2, pwmResolution);
            }
            else if (pin == 9)
            {
                Board.Pwm3.Enabled = true;
                Board.Pwm3.DutyCycle = value / Math.Pow(2, pwmResolution);
            }
            else
            {
                Board.Pins[pin].SoftPwm.Enabled = true;
                Board.Pins[pin].SoftPwm.DutyCycle = value / Math.Pow(2, pwmResolution);
            }
        }

        /// <summary>
        ///     Set the number of bits used for PWM resolution
        /// </summary>
        /// <param name="bits">The number of bits to use</param>
        public void analogWriteResolution(int bits)
        {
            pwmResolution = bits;
        }

        /// <summary>
        ///     Sets the number of bits used for ADC resolution
        /// </summary>
        /// <param name="bits">The number of bits to use</param>
        public void analogReadResolution(int bits)
        {
            adcResolution = bits;
        }

        /// <summary>
        ///     Returns the number of milliseconds since the current sketch started running
        /// </summary>
        /// <returns>the number of milliseconds since the program started</returns>
        public long millis()
        {
            return sw.ElapsedMilliseconds;
        }

        /// <summary>
        ///     Returns the number of microseconds since the current sketch started running
        /// </summary>
        /// <returns>the number of microseconds since the program started</returns>
        public long micros()
        {
            return sw.ElapsedTicks / (Stopwatch.Frequency / 1000000);
        }

        /// <summary>
        ///     Pauses the program for the amount of time (in miliseconds) specified as parameter.
        /// </summary>
        /// <param name="ms">the number of milliseconds to pause</param>
        public void delay(int ms)
        {
            Task.Delay(ms).Wait();
        }

        /// <summary>
        ///     Re-maps a number from one range to another.
        /// </summary>
        /// <param name="value">the number to map</param>
        /// <param name="fromLow">the lower bound of the value's current range</param>
        /// <param name="fromHigh">the upper bound of the value's current range</param>
        /// <param name="toLow">the lower bound of the value's target range</param>
        /// <param name="toHigh">the upper bound of the value's target range</param>
        /// <returns>the mapped value</returns>
        public double map(double value, double fromLow, double fromHigh, double toLow, double toHigh)
        {
            return Numbers.Map(value, fromLow, fromHigh, toLow, toHigh);
        }

        /// <summary>
        ///     Constrains a number to be within a range
        /// </summary>
        /// <param name="x">the number to constrain</param>
        /// <param name="a">the lower end of the range</param>
        /// <param name="b">the upper end of the range</param>
        /// <returns>the constrained number</returns>
        public double constrain(double x, double a, double b)
        {
            return x.Constrain(a, b);
        }
    }
}