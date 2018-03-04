using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Displays
{
    public interface ICharacterDisplay
    {
        /// <summary>
        ///     Number of columns of the display
        /// </summary>
        int Columns { get; }

        /// <summary>
        ///     Number of rows of the display
        /// </summary>
        int Rows { get; }

        /// <summary>
        ///     Cursor left position
        /// </summary>
        int CursorLeft { get; set; }

        /// <summary>
        ///     Cursor top position
        /// </summary>
        int CursorTop { get; set; }

        /// <summary>
        ///     Set left/right cursor position
        /// </summary>
        /// <param name="left">The left position</param>
        /// <param name="top">The top position</param>
        /// <returns>An awaitable task that completes once the cursor is updated</returns>
        Task SetCursorPosition(int left, int top);

        /// <summary>
        ///     Write text
        /// </summary>
        /// <param name="value">the text to write</param>
        /// <returns>
        ///     An awaitable task that completes when finished
        /// </returns>
        Task Write(dynamic value);

        /// <summary>
        ///     Write a line of text, advancing the cursor to the next line
        /// </summary>
        /// <param name="value">the text to write</param>
        /// <returns>
        ///     An awaitable task that completes when finished
        ///     </returns>
        Task WriteLine(dynamic value);

        /// <summary>
        ///     Clear the display and reset the cursor to (0,0)
        /// </summary>
        /// <returns>
        ///     An awaitable task that completes when finished
        /// </returns>
        Task Clear();
    }
}
