// Copyright © 2006-2010 Travis Robinson. All rights reserved.
// 
// website: http://sourceforge.net/projects/libusbdotnet
// e-mail:  libusbdotnet@gmail.com
// 
// This program is free software; you can redistribute it and/or modify it
// under the terms of the GNU General Public License as published by the
// Free Software Foundation; either version 2 of the License, or 
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful, but 
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License
// for more details.
// 
// You should have received a copy of the GNU General Public License along
// with this program; if not, write to the Free Software Foundation, Inc.,
// 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA. or 
// visit www.gnu.org.
// 
// 
using System;
using System.Text;
using LibUsbDotNet.Main;

namespace LibUsbDotNet.LibUsb
{
    internal static class LibUsbDeviceRegistryKeyRequest
    {
        public static byte[] RegGetRequest(string name, int valueBufferSize)
        {
			throw new NotImplementedException();
        }

        public static byte[] RegSetBinaryRequest(string name, byte[] value)
        {
			throw new NotImplementedException();
        }

        public static byte[] RegSetStringRequest(string name, string value)
        {
			throw new NotImplementedException();
        }

        #region Nested Types

        private enum KeyType
        {
            RegSz = 1, // Unicode nul terminated string
            RegBinary = 3, // Free form binary
        }

        #endregion
    }
}