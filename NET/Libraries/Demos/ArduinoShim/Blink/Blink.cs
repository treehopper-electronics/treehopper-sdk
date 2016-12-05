using Treehopper.Libraries.ArduinoShim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper;
namespace Demo
{
    class Blink : Sketch
    {
        public Blink(TreehopperUsb board, bool throwExceptions = true) : base(board, throwExceptions)
        {
        }

        public override void setup()
        {
            Serial.begin(9600);
            Serial.write("Starting sketch...\n");
            pinMode(ledPin, OUTPUT);      // sets the digital pin as output          
        }

        public override void loop()
        {
            digitalWrite(ledPin, HIGH);   // sets the LED on
            delay(1000);                  // waits for a second
            digitalWrite(ledPin, LOW);    // sets the LED off
            delay(1000);                  // waits for a second
        }


    }
}
