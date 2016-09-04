using System;

namespace LibUsb.Profile
{
    /// <summary>
    /// Describes a device arrival/removal notification event
    /// </summary>
    public class AddRemoveEventArgs : EventArgs
    {
        private readonly AddRemoveType mAddRemoveType;
        private readonly LibUsbProfile mMonoUSBProfile;

        internal AddRemoveEventArgs(LibUsbProfile monoUSBProfile, AddRemoveType addRemoveType)
        {
            mMonoUSBProfile = monoUSBProfile;
            mAddRemoveType = addRemoveType;
        }
        /// <summary>
        /// The <see cref ="LibUsbProfile"/> that was added or removed.
        /// </summary>
        public LibUsbProfile MonoUSBProfile
        {
            get { return mMonoUSBProfile; }
        }

        /// <summary>
        /// The type of event that occured.
        /// </summary>
        public AddRemoveType EventType
        {
            get { return mAddRemoveType; }
        }
    }
}