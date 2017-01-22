namespace Treehopper
{
    using System;

    public class TreehopperRuntimeException : Exception
    {
        private string message;

        public TreehopperRuntimeException(string message)
        {
            this.message = message;
        }

        public override string Message
        {
            get { return message; }
        }
    }
}
