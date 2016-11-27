using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Treehopper.Libraries.Displays
{
    public abstract class CharacterDisplay
    {
        public CharacterDisplay(int Columns, int Rows)
        {
            this.Columns = Columns;
            this.Rows = Rows;
            for (int i = 0; i < Rows; i++)
                Lines.Add("");
        }
        public Collection<string> History { get; private set; } = new Collection<string>();
        public Collection<string> Lines { get; private set; } = new Collection<string>();
        public int Columns { get; private set; }
        public int Rows { get; private set; }

        private int cursorLeft;

        public int CusorLeft
        {
            get { return cursorLeft; }
            set
            {
                if (cursorLeft == value) return;
                cursorLeft = value;
                updateCursorPosition();
            }
        }

        private int cursorTop;
        public int CusorTop
        {
            get { return cursorTop; }
            set
            {
                if (cursorTop == value) return;
                cursorTop = value;
                updateCursorPosition();
            }
        }
        public void SetCursorPosition(int left, int top)
        {
            cursorLeft = left;
            cursorTop = top;
            updateCursorPosition();
        }

        public void WriteLine(dynamic value)
        {
            Write(value + "\n");
        }

        public void Write(dynamic value)
        {
            if(cursorTop > Rows-1)
            {
                // we need to shift all the text up, so just clear the display and resend
                // from history
                Clear();
                int startingRow = History.Count - Rows + 1;
                for (int i=0;i<Rows-1;i++)
                {
                    write(History[startingRow + i]);
                    SetCursorPosition(0, i + 1);
                }
            }
            string str = value.ToString();
            StringBuilder s = new StringBuilder();
            foreach(char c in str)
            {
                if(c == '\n')
                {
                    if(s.Length > 0)
                    {
                        History.Add(s.ToString());
                        write(s);
                    }
                    s.Clear();
                    SetCursorPosition(0, cursorTop + 1);
                } else
                {
                    s.Append(c);
                }
            }
            if(s.Length > 0)
            {
                History.Add(s.ToString());
                write(s);
            }
            
        }

        public void Clear()
        {
            cursorLeft = 0;
            cursorTop = 0;
            clear();
        }

        // Methods that display drivers need to implement

        /// <summary>
        /// Clear the display. The driver is expected to reset the cursor to home
        /// </summary>
        protected abstract void clear();
        protected abstract void updateCursorPosition();
        protected abstract void write(dynamic value);
    }
}
