using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Treehopper.Libraries
{
    public class I2cCharLcd
    {
        // commands
        int LCD_CLEARDISPLAY = 0x01;
        int LCD_RETURNHOME = 0x02;
        int LCD_ENTRYMODESET = 0x04;
        int LCD_DISPLAYCONTROL = 0x08;
        int LCD_CURSORSHIFT = 0x10;
        int LCD_FUNCTIONSET = 0x20;
        int LCD_SETCGRAMADDR = 0x40;
        int LCD_SETDDRAMADDR = 0x80;

        // flags for display entry mode
        int LCD_ENTRYRIGHT = 0x00;
        int LCD_ENTRYLEFT = 0x02;
        int LCD_ENTRYSHIFTINCREMENT = 0x01;
        int LCD_ENTRYSHIFTDECREMENT = 0x00;

        // flags for display on/off control
        int LCD_DISPLAYON = 0x04;
        int LCD_DISPLAYOFF = 0x00;
        int LCD_CURSORON = 0x02;
        int LCD_CURSOROFF = 0x00;
        int LCD_BLINKON = 0x01;
        int LCD_BLINKOFF = 0x00;

        // flags for display/cursor shift
        int LCD_DISPLAYMOVE = 0x08;
        int LCD_CURSORMOVE = 0x00;
        int LCD_MOVERIGHT = 0x04;
        int LCD_MOVELEFT = 0x00;

        // flags for function set
        int LCD_8BITMODE = 0x10;
        int LCD_4BITMODE = 0x00;
        int LCD_2LINE = 0x08;
        int LCD_1LINE = 0x00;
        int LCD_5x10DOTS = 0x04;
        int LCD_5x8DOTS = 0x00;

        // flags for backlight control
        int LCD_BACKLIGHT = 0x08;
        int LCD_NOBACKLIGHT = 0x00;

        int En = 4;  // Enable bit
        int Rw = 2;  // Read/Write bit
        int Rs = 1;  // Register select bit

        int _Addr;
        int _displayfunction;
        int _displaycontrol;
        int _displaymode;
        int _numlines;
        int _cols;
        int _rows;
        int _backlightval;

        I2c _I2C;

        // When the display powers up, it is configured as follows:
        //
        // 1. Display clear
        // 2. Function set: 
        //    DL = 1; 8-bit interface data 
        //    N = 0; 1-line display 
        //    F = 0; 5x8 dot character font 
        // 3. Display on/off control: 
        //    D = 0; Display off 
        //    C = 0; Cursor off 
        //    B = 0; Blinking off 
        // 4. Entry mode set: 
        //    I/D = 1; Increment by 1
        //    S = 0; No shift 
        //
        // Note, however, that resetting the Arduino doesn't reset the LCD, so we
        // can't assume that its in that state when a sketch starts (and the
        // LiquidCrystal ructor is called).

        public I2cCharLcd(TreehopperUsb board, int lcd_Addr,int lcd_cols,int lcd_rows)
        {
           _I2C = board.I2c;
          _Addr = lcd_Addr;
          _cols = lcd_cols;
          _rows = lcd_rows;
          _backlightval = LCD_NOBACKLIGHT;

          _I2C.Enabled = true;
          _displayfunction = LCD_4BITMODE | LCD_1LINE | LCD_5x8DOTS;
          begin(_cols, _rows);  
        }

        async void begin(int cols, int lines) {
	        if (lines > 1) {
		        _displayfunction |= LCD_2LINE;
	        }
	        _numlines = lines;

	        // SEE PAGE 45/46 FOR INITIALIZATION SPECIFICATION!
	        // according to datasheet, we need at least 40ms after power rises above 2.7V
	        // before sending commands. Arduino can turn on way befer 4.5V so we'll wait 50
            //delay(50); 
  
	        // Now we pull both RS and R/W low to begin commands
	        expanderWrite(_backlightval);	// reset expanderand turn backlight off (Bit 8 =1)
            await Task.Delay(2000);
            //delay(1000);

            //put the LCD into 4 bit mode
            // this is according to the hitachi HD44780 datasheet
            // figure 24, pg 46

            // we start in 8bit mode, try to set 4 bit mode
            write4bits(0x03 << 4);
            await Task.Delay(100);
           //delayMicroseconds(4500); // wait min 4.1ms
   
           // second try
           write4bits(0x03 << 4);
            //delayMicroseconds(4500); // wait min 4.1ms
            await Task.Delay(100);
            // third go!
            write4bits(0x03 << 4);
            //delayMicroseconds(150);
            await Task.Delay(100);
            // finally, set to 4-bit interface
            write4bits(0x02 << 4);

            await Task.Delay(100);

            // set # lines, font size, etc.
            command(LCD_FUNCTIONSET | _displayfunction);  
	
	        // turn the display on with no cursor or blinking default
	        _displaycontrol = LCD_DISPLAYON | LCD_CURSOROFF | LCD_BLINKOFF;
	        display();
            await Task.Delay(100);
            // clear it off
            clear();
            await Task.Delay(100);
            // Initialize to default text direction (for roman languages)
            _displaymode = LCD_ENTRYLEFT | LCD_ENTRYSHIFTDECREMENT;
	
	        // set the entry mode
	        command(LCD_ENTRYMODESET | _displaymode);
            await Task.Delay(100);
            home();
            await Task.Delay(100);
        }

        /********** high level commands, for the user! */
        public void clear(){
	        command(LCD_CLEARDISPLAY);// clear display, set cursor position to zero
            //delayMicroseconds(2000);  // this command takes a long time!
        }

        public void home(){
	        command(LCD_RETURNHOME);  // set cursor position to zero
            //delayMicroseconds(2000);  // this command takes a long time!
        }

        public void setCursor(int col, int row){
	        byte[] row_offsets = new byte[]{ 0x00, 0x40, 0x14, 0x54 };
	        if ( row > _numlines ) {
		        row = _numlines-1;    // we count rows starting w/0
	        }
	        command(LCD_SETDDRAMADDR | (col + row_offsets[row]));
        }

        // Turn the display on/off (quickly)
        public void noDisplay() {
	        _displaycontrol &= ~LCD_DISPLAYON;
	        command(LCD_DISPLAYCONTROL | _displaycontrol);
        }
        public void display()
        {
	        _displaycontrol |= LCD_DISPLAYON;
	        command(LCD_DISPLAYCONTROL | _displaycontrol);
        }

        // Turns the underline cursor on/off
        public void noCursor()
        {
	        _displaycontrol &= ~LCD_CURSORON;
	        command(LCD_DISPLAYCONTROL | _displaycontrol);
        }
        public void cursor()
        {
	        _displaycontrol |= LCD_CURSORON;
	        command(LCD_DISPLAYCONTROL | _displaycontrol);
        }

        // Turn on and off the blinking cursor
        public void noBlink()
        {
	        _displaycontrol &= ~LCD_BLINKON;
	        command(LCD_DISPLAYCONTROL | _displaycontrol);
        }
        public void blink()
        {
	        _displaycontrol |= LCD_BLINKON;
	        command(LCD_DISPLAYCONTROL | _displaycontrol);
        }

        // These commands scroll the display without changing the RAM
        public void scrollDisplayLeft()
        {
	        command(LCD_CURSORSHIFT | LCD_DISPLAYMOVE | LCD_MOVELEFT);
        }
        public void scrollDisplayRight()
        {
	        command(LCD_CURSORSHIFT | LCD_DISPLAYMOVE | LCD_MOVERIGHT);
        }

        // This is for text that flows Left to Right
        public void leftToRight()
        {
	        _displaymode |= LCD_ENTRYLEFT;
	        command(LCD_ENTRYMODESET | _displaymode);
        }

        // This is for text that flows Right to Left
        public void rightToLeft()
        {
	        _displaymode &= ~LCD_ENTRYLEFT;
	        command(LCD_ENTRYMODESET | _displaymode);
        }

        // This will 'right justify' text from the cursor
        public void autoscroll()
        {
	        _displaymode |= LCD_ENTRYSHIFTINCREMENT;
	        command(LCD_ENTRYMODESET | _displaymode);
        }

        // This will 'left justify' text from the cursor
        public void noAutoscroll()
        {
	        _displaymode &= ~LCD_ENTRYSHIFTINCREMENT;
	        command(LCD_ENTRYMODESET | _displaymode);
        }

        // Allows us to fill the first 8 CGRAM locations
        // with custom characters
        public void createChar(int location, int[] charmap)
        {
	        location &= 0x7; // we only have 8 locations 0-7
	        command(LCD_SETCGRAMADDR | (location << 3));
	        for (int i=0; i<8; i++) {
                //write(charmap[i]);
	        }
        }

        // Turn the (optional) backlight off/on
        public void noBacklight()
        {
	        _backlightval=LCD_NOBACKLIGHT;
	        expanderWrite(0);
        }

        public void backlight()
        {
	        _backlightval=LCD_BACKLIGHT;
	        expanderWrite(0);
        }



        /*********** mid level commands, for sending data/cmds */

        void command(int value) {
	        send(value, 0);
        }

        public void print(string str)
        {
           byte[] array = ASCIIEncoding.ASCII.GetBytes(str);
            foreach(byte character in array)
            {
                send(character, 0x01);
                
                // expanderWrite(0);
            }
        }

        /// <summary>
        /// Prints a full line, including any required whitespace to clear the existing display buffer.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="line"></param>
        public void printLine(string str, int line)
        {
            setCursor(0, line);
            print(str);
            int numberOfSpaces = _cols - str.Length;
            if(numberOfSpaces > 0)
            {
                for (int i = 0; i < numberOfSpaces; i++)
                    print(" ");
            }
        }


        /************ low level data pushing commands **********/

        // write either command or data
        void send(int value, int mode) {
	        int highnib=value&0xf0;
	        int lownib=(value<<4)&0xf0;
            write4bits((highnib)|mode);
	        write4bits((lownib)|mode);
        }

        void write4bits(int value) {
	        expanderWrite(value);
	        pulseEnable(value);
        }

        async void expanderWrite(int _data){
            //_I2C.Write((byte)_Addr, new byte[] {(byte)(_data | _backlightval) });
            _I2C.SendReceive((byte)_Addr, new byte[] { (byte)(_data | _backlightval) }, 0);
            await Task.Delay(2);
        }

        void pulseEnable(int _data){
	        expanderWrite(_data | En);	// En high
            //delayMicroseconds(1);		// enable pulse must be >450ns
	
	        expanderWrite(_data & ~En);	// En low
            //delayMicroseconds(50);		// commands need > 37us to settle
        } 


        // Alias functions

        void cursor_on(){
	        cursor();
        }

        void cursor_off(){
	        noCursor();
        }

        void blink_on(){
	        blink();
        }

        void blink_off(){
	        noBlink();
        }

        void load_custom_character(int char_num, int rows){
                //createChar(char_num, rows);
        }

        void setBacklight(bool new_val){
	        if(new_val){
		        backlight();		// turn backlight on
	        }else{
		        noBacklight();		// turn backlight off
	        }
        }

        public async void printMessage(string test, int delay=250)
        {
            string[] words = test.Split(' ');
            List<string> lines = new List<string>();
            string currentLine = "";
            foreach(string word in words)
            {
                // do we have enough space on the current line?
                if (currentLine.Length + word.Length + 1 > _cols)
                {
                    lines.Add(currentLine);
                    currentLine = "";                    
                }
                currentLine += word + " ";
            }
            lines.Add(currentLine);

            // Alright, now we have all our lines. Let's print them.
            clear();
            for (int i = 0; i < lines.Count-1; i++)
            {
                printLine(lines[i], 0);
                printLine(lines[i + 1], 1);
                await Task.Delay(delay);
            }
        }
    }
}
