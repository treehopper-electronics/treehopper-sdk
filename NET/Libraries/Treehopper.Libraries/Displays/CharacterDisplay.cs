using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Displays
{
    public abstract class CharacterDisplay
    {
        public Collection<string> History { get; private set; } = new Collection<string>();
        public Collection<string> Lines { get; private set; } = new Collection<string>();
        public int Columns { get; private set; }
        public int Rows { get; private set; }

        private int cursorLeft;

        private int cursorTop;

        public CharacterDisplay(int Columns, int Rows)
        {
            this.Columns = Columns;
            this.Rows = Rows;
            for (int i = 0; i < Rows; i++)
                Lines.Add("");
        }

        public int CusorLeft
        {
            get { return cursorLeft; }
            set
            {
                if (cursorLeft == value) return;
                cursorLeft = value;
                updateCursorPosition().Wait();
            }
        }

        public int CusorTop
        {
            get { return cursorTop; }
            set
            {
                if (cursorTop == value) return;
                cursorTop = value;
                updateCursorPosition().Wait();
            }
        }
        public Task SetCursorPosition(int left, int top)
        {
            cursorLeft = left;
            cursorTop = top;
            return updateCursorPosition();
        }

        public Task WriteLine(dynamic value)
        {
            return Write(value + "\n");
        }

        public async Task Write(dynamic value)
        {
            if(cursorTop > Rows-1)
            {
                // we need to shift all the text up, so just clear the display and resend
                // from history
                await Clear().ConfigureAwait(false);
                int startingRow = History.Count - Rows + 1;
                for (int i=0;i<Rows-1;i++)
                {
                    await write(History[startingRow + i]).ConfigureAwait(false);
                    await SetCursorPosition(0, i + 1).ConfigureAwait(false);
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
                        await write(s).ConfigureAwait(false);
                    }
                    s.Clear();
                    await SetCursorPosition(0, cursorTop + 1).ConfigureAwait(false);
                } else
                {
                    s.Append(c);
                }
            }
            if(s.Length > 0)
            {
                History.Add(s.ToString());
                await write(s).ConfigureAwait(false);
            }
            
        }

        public Task Clear()
        {
            cursorLeft = 0;
            cursorTop = 0;
            return clear();
        }

        // Methods that display drivers need to implement

        /// <summary>
        /// Clear the display. The driver is expected to reset the cursor to home
        /// </summary>
        protected abstract Task clear();
        protected abstract Task updateCursorPosition();
        protected abstract Task write(dynamic value);
    }
}
