namespace Treehopper
{
    using System;

    /// <summary>
    /// A runtime exception caused in Treehopper
    /// </summary>
    public class TreehopperRuntimeException : Exception
    {
        private readonly string message;

        /// <summary>
        /// Construct a runtime exception
        /// </summary>
        /// <param name="message">the message to print</param>
        public TreehopperRuntimeException(string message)
        {
            this.message = message;
        }

        /// <summary>
        /// The message of the exception
        /// </summary>
        public override string Message => message;
    }
}
