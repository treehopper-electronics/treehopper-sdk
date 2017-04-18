using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Displays
{
    /// <summary>
    /// Represents displays that can print strings of alphanumeric characters on one or more lines
    /// </summary>
    public abstract class CharacterDisplay
    {
        /// <summary>
        /// History of previously-written lines from the display
        /// </summary>
        public Collection<string> History { get; private set; } = new Collection<string>();

        /// <summary>
        /// Current lines of the display
        /// </summary>
        public Collection<string> Lines { get; private set; } = new Collection<string>();

        /// <summary>
        /// Number of columns of the display
        /// </summary>
        public int Columns { get; private set; }

        /// <summary>
        /// Number of rows of the display
        /// </summary>
        public int Rows { get; private set; }

        private int cursorLeft;

        private int cursorTop;

        /// <summary>
        /// Construct a character display
        /// </summary>
        /// <param name="Columns">Number of columns</param>
        /// <param name="Rows">Number of rows</param>
        public CharacterDisplay(int Columns, int Rows)
        {
            this.Columns = Columns;
            this.Rows = Rows;
            for (int i = 0; i < Rows; i++)
                Lines.Add("");
        }

        /// <summary>
        /// Cursor left position
        /// </summary>
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

        /// <summary>
        /// Cursor top position
        /// </summary>
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

        /// <summary>
        /// Set left/right cursor position
        /// </summary>
        /// <param name="left">The left position</param>
        /// <param name="top">The top position</param>
        /// <returns></returns>
        public Task SetCursorPosition(int left, int top)
        {
            cursorLeft = left;
            cursorTop = top;
            return updateCursorPosition();
        }

        /// <summary>
        /// Write a line of text, advancing the cursor to the next line
        /// </summary>
        /// <param name="value">the text to write</param>
        /// <returns>An awaitable task that completes when finished</returns>
        public Task WriteLine(dynamic value)
        {
            return Write(value + "\n");
        }

        /// <summary>
        /// Write text
        /// </summary>
        /// <param name="value">the text to write</param>
        /// <returns>An awaitable task that completes when finished</returns>
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

        /// <summary>
        /// Clear the display
        /// </summary>
        /// <returns>An awaitable task that completes when finished</returns>
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

        /// <summary>
        /// internally updates the current cursor position
        /// </summary>
        /// <returns></returns>
        protected abstract Task updateCursorPosition();

        /// <summary>
        /// write text at the current cursor position
        /// </summary>
        /// <param name="value">the data to write</param>
        /// <returns>An awaitable task that completes when finished</returns>
        protected abstract Task write(dynamic value);
    }
}
