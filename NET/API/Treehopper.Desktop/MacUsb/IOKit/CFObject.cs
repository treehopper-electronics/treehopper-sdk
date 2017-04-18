using System;

namespace Treehopper.Desktop.MacUsb.IOKit
{
    /// <summary>
    ///     Manages native CFObjects references.
    /// </summary>
    public class CFObject : IDisposable
    {
        private bool disposedValue; // To detect redundant calls

        /// <summary>
        ///     Initializes a new instance of the see <see cref="CFObject" />.
        /// </summary>
        public CFObject(IntPtr handle)
        {
            Handle = handle;
        }

        /// <summary>
        ///     Gets the native handle.
        /// </summary>
        public IntPtr Handle { get; private set; }

        /// <summary>
        ///     Frees managed and unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Finalizes this instance.
        /// </summary>
        ~CFObject()
        {
            Dispose(false);
        }

        /// <summary>
        ///     Frees managed and unmanaged resources.
        /// </summary>
        /// <param name="disposing">True if called from user space; Otherwise, false.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // dispose managed state (managed objects).
                }

                if (Handle != IntPtr.Zero)
                {
                    IOKitFramework.CFRelease(this);
                    Handle = IntPtr.Zero;
                }

                disposedValue = true;
            }
        }
    }
}