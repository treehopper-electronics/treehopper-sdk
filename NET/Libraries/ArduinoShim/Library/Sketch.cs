using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Treehopper;
using System.Threading.Tasks;

namespace ArduinoShim
{
    public abstract class Sketch
    {
        private bool throwExceptions;
        public SerialShim Serial;
        /// <summary>
        /// Initialize and run a sketch
        /// </summary>
        /// <param name="board">The Treehopper board to use</param>
        /// <param name="throwExceptions">Whether unimplemented or miscalled functions should throw exceptions or fail silently</param>
        public Sketch(TreehopperUsb board, bool throwExceptions = true)
        {
            Board = board;
            this.throwExceptions = throwExceptions;
            board.Connect().Wait();

            Serial = new SerialShim(board);
        }

        public void Run()
        {
            sw.Start();
            setup();
            while (true)
                loop();
        }

        Stopwatch sw = new Stopwatch();

        int pwmResolution = 8;
        int adcResolution = 10;

        public abstract void setup();

        public abstract void loop();

        public const int INPUT = 1;
        public const int OUTPUT = 2;
        public const int INPUT_PULLUP = 3;

        public const bool HIGH = true;
        public const bool LOW = false;

        public const int ledPin = int.MaxValue;

        public const bool MSBFIRST = true;
        public const bool LSBFIRST = false;



        /// <summary>
        /// Configures the specified pin to behave either as an input or an output.
        /// </summary>
        /// <param name="pin">the number of the pin whose mode you wish to set</param>
        /// <param name="mode">INPUT, OUTPUT, or INPUT_PULLUP. Note that Treehopper inputs are always weakly pulled-up, so there is no difference between INPUT and INPUT_PULLUP</param>
        public void pinMode(int pin, int mode)
        {
            if (pin == ledPin)
                return; // LED is always an output

            if (!throwExceptions && (pin >= Board.NumberOfPins || pin <= 0))  // fail silently
                return;

            switch(mode)
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
        /// Write a HIGH or a LOW value to a digital pin. To set the LED, use "LedPin"
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

            if (!throwExceptions && (pin >= Board.NumberOfPins || pin <= 0))  // fail silently
                return;

            if (Board.Pins[pin].Mode == PinMode.PushPullOutput)
                Board.Pins[pin].DigitalValue = value;
        }

        /// <summary>
        /// Reads the value from a specified digital pin, either HIGH or LOW.
        /// </summary>
        /// <param name="pin">the number of the digital pin you want to read (int)</param>
        /// <returns>bool true or false (can compare with HIGH or LOW)</returns>
        public bool digitalRead(int pin)
        {
            if (pin == ledPin)
                return Board.Led;

            if (!throwExceptions && (pin >= Board.NumberOfPins || pin <= 0))  // fail silently
                return false;

            if (Board.Pins[pin].Mode == PinMode.DigitalInput)
                return Board.Pins[pin].DigitalValue;

            return false; // some other state
        }

        /// <summary>
        /// Reads the value from the specified analog pin.
        /// </summary>
        /// <param name="pin">the number of the analog input pin to read from</param>
        /// <returns>int (0 to 2^analogReadResolution -- 1024 by default)</returns>
        public int analogRead(int pin)
        {
            if (!throwExceptions && (pin >= Board.NumberOfPins || pin <= 0))  // fail silently
                return 0;

            Board.Pins[pin].Mode = PinMode.AnalogInput;
            return (int)Math.Round(Board.Pins[pin].AdcValue * (Math.Pow(2, adcResolution) / 4096.0 ));
        }

        /// <summary>
        /// Writes a PWM value to a pin. Uses zero-jitter hardware PWM on pins 7, 9, and 9. Uses software PWM on all other pins.
        /// </summary>
        /// <param name="pin">the pin to write to</param>
        /// <param name="value">the duty cycle between 0 (always off) and 2^analogWriteResolution (defaults to 255) (always on)</param>
        public void analogWrite(int pin, int value)
        {
            if (!throwExceptions && (pin >= Board.NumberOfPins || pin <= 0))  // fail silently
                return;

            if (pin == 7)
            {
                Board.Pwm1.Enabled = true;
                Board.Pwm1.DutyCycle = value / Math.Pow(2, pwmResolution);
            } else if (pin == 8)
            {
                Board.Pwm2.Enabled = true;
                Board.Pwm2.DutyCycle = value / Math.Pow(2, pwmResolution);
            } else if (pin == 9)
            {
                Board.Pwm3.Enabled = true;
                Board.Pwm3.DutyCycle = value / Math.Pow(2, pwmResolution);
            } else
            {
                Board.Pins[pin].SoftPwm.Enabled = true;
                Board.Pins[pin].SoftPwm.DutyCycle = value / Math.Pow(2, pwmResolution);
            }
        }

        public void analogWriteResolution(int bits)
        {
            pwmResolution = bits;
        }

        public void analogReadResolution(int bits)
        {
            adcResolution = bits;
        }

        /// <summary>
        /// Returns the number of milliseconds since the current sketch started running
        /// </summary>
        /// <returns>the number of milliseconds since the program started</returns>
        public long millis()
        {
            return sw.ElapsedMilliseconds;
        }

        /// <summary>
        /// Returns the number of microseconds since the current sketch started running
        /// </summary>
        /// <returns>the number of microseconds since the program started</returns>
        public long micros()
        {
            return sw.ElapsedTicks / (Stopwatch.Frequency / (1000000));
        }

        /// <summary>
        /// Pauses the program for the amount of time (in miliseconds) specified as parameter.
        /// </summary>
        /// <param name="ms">the number of milliseconds to pause</param>
        public void delay(int ms)
        {
            Task.Delay(ms).Wait();
        }

        /// <summary>
        /// Re-maps a number from one range to another. 
        /// </summary>
        /// <param name="value">the number to map</param>
        /// <param name="fromLow">the lower bound of the value's current range</param>
        /// <param name="fromHigh">the upper bound of the value's current range</param>
        /// <param name="toLow">the lower bound of the value's target range</param>
        /// <param name="toHigh">the upper bound of the value's target range</param>
        /// <returns>the mapped value</returns>
        public double map(double value, double fromLow, double fromHigh, double toLow, double toHigh)
        {
            return Utilities.Map(value, fromLow, fromHigh, toLow, toHigh);
        }

        /// <summary>
        /// Constrains a number to be within a range
        /// </summary>
        /// <param name="x">the number to constrain</param>
        /// <param name="a">the lower end of the range</param>
        /// <param name="b">the upper end of the range</param>
        /// <returns>the constrained number</returns>
        public double constrain(double x, double a, double b)
        {
            return Utilities.Constrain(x, a, b);
        }


        #region unimplementedFuncs

        public void delayMicroseconds(int us)
        {
            if (throwExceptions)
                throw new NotImplementedException("delayMicroseconds() is not implemented in this library, since timing precision greater than about 15 milliseconds cannot be guaranteed.");
        }

        public void tone(int pin, int frequency)
        {
            if (throwExceptions)
                throw new NotImplementedException("tone() is not implemented on Treehopper");
        }

        public void noTone(int pin)
        {
            if (throwExceptions)
                throw new NotImplementedException("noTone() is not implemented on Treehopper");
        }

        public void shiftOut(int dataPin, int clockPin, bool bitOrder, byte value)
        {
            if (throwExceptions)
                throw new NotImplementedException("shiftOut() is not implemented on Treehopper");
        }

        public byte shiftIn(int dataPin, int clockPin, bool bitOrder)
        {
            if (throwExceptions)
                throw new NotImplementedException("shiftIn() is not implemented on Treehopper");
            return 0;
        }

        public int pulseIn(int pin, bool value, int timeout = 0)
        {
            if (throwExceptions)
                throw new NotImplementedException("pulseIn() is not implemented on Treehopper");
            return 0;
        }

        public void analogReference(int type)
        {
            if (throwExceptions)
                throw new NotImplementedException("analogReference() is not implemented on Treehopper");
        }
        #endregion
    }
}
