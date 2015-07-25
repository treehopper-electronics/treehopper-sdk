/*
*    LedControl.cpp - A library for controling Leds with a MAX7219/MAX7221
*    Copyright (c) 2007 Eberhard Fahle
* 
*    Permission is hereby granted, free of charge, to any person
*    obtaining a copy of this software and associated documentation
*    files (the "Software"), to deal in the Software without
*    restriction, including without limitation the rights to use,
*    copy, modify, merge, publish, distribute, sublicense, and/or sell
*    copies of the Software, and to permit persons to whom the
*    Software is furnished to do so, subject to the following
*    conditions:
* 
*    This permission notice shall be included in all copies or 
*    substantial portions of the Software.
* 
*    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
*    EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
*    OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
*    NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
*    HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
*    WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
*    FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
*    OTHER DEALINGS IN THE SOFTWARE.
*/


using System;
using System.Collections.Generic;

namespace Treehopper.Libraries
{
    public class SevenSegSpi
    {
        byte[] charTable = new byte[128]{ 0x7E, 0x30, 0x6D, 0x79, 0x33, 0x5B, 0x5F, 0x70, 0x7F, 0x7B, 0x77, 0x1F, 0xD, 0x3D, 0x4F, 0x47, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x80, 0x1, 0x80, 0x0, 0x7E, 0x30, 0x6D, 0x79, 0x33, 0x5B, 0x5F, 0x70, 0x7F, 0x7B, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x77, 0x1F, 0xD, 0x3D, 0x4F, 0x47, 0x0, 0x37, 0x0, 0x0, 0x0, 0xE, 0x0, 0x0, 0x0, 0x67, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x8, 0x0, 0x77, 0x1F, 0xD, 0x3D, 0x4F, 0x47, 0x0, 0x37, 0x0, 0x0, 0x0, 0xE, 0x0, 0x0, 0x0, 0x67, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 };
        byte[] status = new byte[64];
        //the opcodes for the MAX7221 and MAX7219
        byte OP_NOOP   = 0;
        byte OP_DIGIT0 = 1;
        byte OP_DIGIT1 =2;
        byte OP_DIGIT2 =3;
        byte OP_DIGIT3 =4;
        byte OP_DIGIT4 =5;
        byte OP_DIGIT5 =6;
        byte OP_DIGIT6 =7;
        byte OP_DIGIT7 =8;
        byte OP_DECODEMODE  =9;
        byte OP_INTENSITY   =10;
        byte OP_SCANLIMIT   =11;
        byte OP_SHUTDOWN    =12;
        byte OP_DISPLAYTEST =15;
        
        Spi SPIModule;
        
        Pin csPin;

        int numDevices;

        byte[] spiData;

        public SevenSegSpi(Spi SPIModule, Pin csPin, int numDevices = 1) 
        {
            this.SPIModule = SPIModule;
            //SPIModule.Start(SPIMode.Mode00, 1);
            this.csPin = csPin;

            if(numDevices<=0 || numDevices>8 )
                throw new Exception("This library supports 1 to 8 displays.");

            this.numDevices = numDevices;
            
            this.csPin.DigitalValue = true;

             spiData = new byte[numDevices*2];

            for(int i=0; i<numDevices; i++) 
            {
                spiTransfer(OP_DISPLAYTEST, 0, i);
                //scanlimit is set to max on startup
                setScanLimit(7, i);
                //decode is done in source
                spiTransfer(OP_DECODEMODE, 0, i);
                clearDisplay(i);
                //we go into shutdown-mode on startup
                shutdown(false, i);
                setIntensity(10, i);
            }
        }

        public int getDeviceCount() {
            return numDevices;
        }

        public void shutdown(bool b, int addr = 0)
        {
            if(addr<0 || addr>=numDevices)
	        return;
            if(b)
                spiTransfer(OP_SHUTDOWN, 0, addr);
            else
                spiTransfer(OP_SHUTDOWN, 1, addr);
        }

        public void setScanLimit(byte limit, int addr = 0)
        {
            if(addr<0 || addr>=numDevices)
	        return;
            if(limit>=0 || limit<8)
                spiTransfer(OP_SCANLIMIT, limit, addr);
        }

        public void setIntensity(byte intensity, int addr = 0)
        {
            if(addr<0 || addr>=numDevices)
	        return;
            if(intensity>=0 || intensity<16)
                spiTransfer(OP_INTENSITY, intensity, addr);
    
        }

        public void clearDisplay(int addr = 0)
        {
            int offset;

            if(addr<0 || addr>=numDevices)
	        return;
            offset=addr*8;
            for(byte i=0;i<8;i++) {
	        status[offset+i]=0;
            spiTransfer((byte)(i + 1), status[offset + i], addr);
            }
        }

        public void setLed(int row, int column, bool state, int addr = 0)
        {
            int offset;
            byte val=0x00;

            if(addr<0 || addr>=numDevices)
	        return;
            if(row<0 || row>7 || column<0 || column>7)
	        return;
            offset=addr*8;
            val=(byte)(0x80 >> column);
            if (state)
            {
                status[offset + row] = (byte)(status[offset + row] | val);
            }
            else
            {
                val = (byte)~val;
                status[offset + row] = (byte)(status[offset + row] & val);
            }

            spiTransfer((byte)(row + 1), status[offset + row], addr);
        }

        public void setRow(int row, byte value, int addr = 0)
        {
            int offset;
            if(addr<0 || addr>=numDevices)
	        return;
            if(row<0 || row>7)
	        return;
            offset=addr*8;
            status[offset+row]=value;
            spiTransfer((byte)(row + 1), status[offset + row], addr);
        }

        public void setColumn(int col, byte value, int addr = 0)
        {
            byte val;

            if(addr<0 || addr>=numDevices)
	        return;
            if(col<0 || col>7) 
	        return;
            for(int row=0;row<8;row++) {
	        val=(byte)(value >> (7-row));
	        val=(byte)(val & 0x01);
            setLed(row, col, val > 0, addr);
            }
        }

        public void setDigit(int digit, int value, bool dp, int addr = 0)
        {
            int offset;
            byte v;

            if(addr<0 || addr>=numDevices)
	        return;
            if(digit<0 || digit>7 || value>15)
	        return;
            offset=addr*8;
            v=charTable[value];
            if(dp)
	        v|=0x80;
            status[offset+digit]=v;
            spiTransfer((byte)(digit + 1), v, addr);
    
        }


        public void printNumber(decimal value, int addr = 0, bool rightAlign = true, bool alwaysDisplayDecimalPoint = true)
        {
            if (value > 99999999 || value < -9999999)
                throw new Exception("This display is limited to numbers between -9999999 and 99999999");

            bool isNegative = (value < 0);

            if (isNegative)
                value = -value; // make it positive for future operations

            // lc.printNumber(123456.789);
            int intPart = (int)Math.Truncate(value);
            decimal decimalPart = value - intPart;
            
            List<int> numbers = new List<int>();

            if (intPart == 0) // if we've got a decimal-only number, add a leading zero.
                numbers.Add(0);
            while(intPart > 0)
            {
                numbers.Add(intPart % 10);
                intPart /= 10;
            }

            // the list is flipped, so reverse it
            numbers.Reverse();
            int wholeNumberCount = numbers.Count;
            int decimalPointPosition = numbers.Count;
            int digitsRemaining = 8 - (isNegative ? 1 : 0) - numbers.Count; // 8 digits - negative sign - integers
            decimalPart = Math.Round(decimalPart, digitsRemaining);
            bool containsDecimals = decimalPart > 0;
            // how many decimal digits do we have left over?
            int numDecimalDigits = 0;

            while (numDecimalDigits < digitsRemaining)
            {
                decimalPart *= 10;
                numbers.Add((int)decimalPart % 10);
                numDecimalDigits++;
            }

            numbers.Reverse();

            decimalPointPosition = numbers.Count - decimalPointPosition;

            if(rightAlign) // trim off trailing zeros
            {
                while (numbers[0] == 0 && numbers.Count > 1 && numbers.Count > wholeNumberCount) // we only go up to count-1 in case the number is zero.
                {
                    numbers.RemoveAt(0);
                    decimalPointPosition--;
                }
            }

            clearDisplay(addr);
            for(int i=0; i<numbers.Count; i++)
            {
                if(i == decimalPointPosition && (alwaysDisplayDecimalPoint || containsDecimals))
                    setDigit(i, numbers[i], true, addr);
                else
                    setDigit(i, numbers[i], false, addr);
            }


        }

        public void setChar(int digit, char value, bool dp, int addr = 0)
        {
            int offset;
            byte index,v;

            if(addr<0 || addr>=numDevices)
	        return;
            if(digit<0 || digit>7)
 	        return;
            offset=addr*8;
            index=(byte)value;
            if(index >127) {
	        //no defined beyond index 127, so we use the space char
	        index=32;
            }
            v=charTable[index];
            if(dp)
            {
                v |= 0x80;
            }

            status[offset+digit]=v;
            spiTransfer((byte)(digit + 1), v, addr);
        }

        public void spiTransfer(byte opcode, byte data, int addr = 0)
        {
            //Create an array with the data to shift out
            int offset = addr*2;
            int maxbytes = numDevices*2;

            

            for(int i=0;i<maxbytes;i++)
	            spiData[i]=(byte)0;
            
            //put our device data into the array
            spiData[offset+1]=opcode;
            spiData[offset]=data;

            //enable the line 
            csPin.DigitalValue = false;

            //Now shift out the data 
            Array.Reverse(spiData);
            SPIModule.SendReceive(spiData);
            //latch the data onto the display
            csPin.DigitalValue = true;
        }    
    }
}
