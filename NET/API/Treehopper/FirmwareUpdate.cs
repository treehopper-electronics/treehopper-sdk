using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper
{
    /// <summary>
    /// A class for uploading new firmware images to a board
    /// </summary>
    public class FirmwareUpdater
    {
        const int SizeIn = 4;
        const int SizeOut = 64;

        /// <summary>
        /// Fires whenever firmware updating progress has changed.
        /// </summary>
        public event ProgressChangedEventHandler ProgressChanged;

        IFirmwareConnection connection;

        /// <summary>
        /// Create a new FirmwareUpdater from an <see cref="IFirmwareConnection"/>  connection.
        /// </summary>
        /// <param name="connection">A platform-specific FirmwareConnection</param>
        public FirmwareUpdater(IFirmwareConnection connection)
        {
            this.connection = connection;
        }

        /// <summary>
        /// Connect to a board that is currently in bootloader mode.
        /// </summary>
        /// <returns>An awaitable bool indicating whether a connection was successful.</returns>
        /// <remarks>
        /// <para>
        /// Unlike <see cref="IConnectionService.GetFirstDeviceAsync"/>, this call will not wait for a board to be connected.
        /// </para>
        /// </remarks>
        public Task<bool> ConnectAsync()
        {
            return connection.OpenAsync();
        }

        /// <summary>
        /// Load a firmware image specified by the path
        /// </summary>
        /// <param name="path">A file path to the firmware file to load</param>
        /// <returns>An awaitable bool indicating wheather loading was successful.</returns>
        public async Task<bool> Load(string path)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Load a firmware image from a Stream of data
        /// </summary>
        /// <param name="data">the firmware image Stream.</param>
        /// <returns>An awaitable bool indicating whether loading was successful.</returns>
        public async Task<bool> Load(Stream data)
        {
            byte dollarSign = Encoding.UTF8.GetBytes("$")[0];
            var header = ReadBytes(data, 2);
            while(header[0] == dollarSign)
            {
                var record = ReadBytes(data, header[1]);
                var frame = header.Concat(record).ToArray();
                if(header[0] != dollarSign || header[1] != frame.Length-2)
                {
                    throw new Exception("Bad record header");
                }
                header = ReadBytes(data, 2);
                var reply = await SendFrame(frame).ConfigureAwait(false);
                if(Encoding.UTF8.GetChars(reply)[0] != '@')
                {
                    throw new Exception("Received an invalid response");
                }

                var progress = (int)(100.0 * data.Position / data.Length);
                if (ProgressChanged != null) ProgressChanged(this, new ProgressChangedEventArgs(progress, null));
            }
            return true;
        }

        private async Task<byte[]> SendFrame(byte[] frame)
        {
            for(int i=0; i<frame.Length; i += SizeOut)
            {
                var buffer = new byte[SizeOut];
                Array.Copy(frame, i, buffer, 0, Math.Min(frame.Length-i, SizeOut));
                await connection.Write(buffer).ConfigureAwait(false);
            }
            var res = await connection.Read(SizeIn).ConfigureAwait(false);
            return res;
        }

        private byte[] ReadBytes(Stream source, int count)
        {
            var retVal = new byte[count];
            for(int i=0;i<count;i++)
            {
                retVal[i] = (byte)source.ReadByte();
            }
            return retVal;
        }

        /// <summary>
        /// Load the firmware image built-in to this assembly
        /// </summary>
        /// <returns>An awaitable bool indicating true for success or false for failure.</returns>
        public Task<bool> LoadAsync()
        {
            var stream = GetType().GetTypeInfo().Assembly.GetManifestResourceStream("Treehopper.treehopper.tfi");
            return Load(stream);
        }
    }
}
