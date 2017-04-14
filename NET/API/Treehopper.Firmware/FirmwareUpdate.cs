namespace Treehopper.Firmware
{
    using HidSharp;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// A class for uploading new firmware images to a board
    /// </summary>
    public class FirmwareUpdater
    {
        static HidDeviceLoader loader = new HidDeviceLoader();

        private const int SizeIn = 4;
        private const int SizeOut = 64;
        
        private HidDevice connection;
        private HidStream stream;

        public static IList<FirmwareUpdater> Boards
        {
            get
            {
                Collection<FirmwareUpdater> collection = new Collection<FirmwareUpdater>();
                var devices = loader.GetDevices(FirmwareUpdater.BootloaderVid, FirmwareUpdater.BootloaderPid);
                foreach(var device in devices)
                {
                    collection.Add(new FirmwareUpdater(device));
                }

                return collection;
            }
        }

        public FirmwareUpdater(HidDevice connection)
        {
            this.connection = connection;
        }


        /// <summary>
        /// Fires whenever firmware updating progress has changed.
        /// </summary>
        public event ProgressChangedEventHandler ProgressChanged;


        /// <summary>
        /// The PID used by the bootloader
        /// </summary>
        public static int BootloaderPid { get; set; } = 0xeac9;

        /// <summary>
        /// The VID used by the bootloader
        /// </summary>
        public static int BootloaderVid { get; set; } = 0x10c4;

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
            this.stream = connection.Open();
            byte dollarSign = Encoding.UTF8.GetBytes("$")[0];
            var header = ReadBytes(data, 2);
            while (header[0] == dollarSign)
            {
                var record = ReadBytes(data, header[1]);
                var frame = header.Concat(record).ToArray();
                if (header[0] != dollarSign || header[1] != frame.Length - 2)
                {
                    throw new Exception("Bad record header");
                }

                header = ReadBytes(data, 2);
                var reply = await SendFrame(frame).ConfigureAwait(false);
                if (Encoding.UTF8.GetChars(reply)[0] != '@')
                {
                    throw new Exception("Received an invalid response");
                }

                var progress = (int)(100.0 * data.Position / data.Length);
                if (ProgressChanged != null)
                    ProgressChanged(this, new ProgressChangedEventArgs(progress, null));
            }

            stream.Close();

            return true;
        }

        /// <summary>
        /// Load the firmware image built-in to this assembly
        /// </summary>
        /// <returns>An awaitable bool indicating true for success or false for failure.</returns>
        public Task<bool> LoadAsync()
        {
            var stream = GetType().GetTypeInfo().Assembly.GetManifestResourceStream("Treehopper.Firmware.treehopper.tfi");
            return Load(stream);
        }

        private async Task<byte[]> SendFrame(byte[] frame)
        {
            for (int i = 0; i < frame.Length; i += SizeOut)
            {
                var buffer = new byte[SizeOut + 1];
                Array.Copy(frame, i, buffer, 1, Math.Min(frame.Length - i, SizeOut));
                await stream.WriteAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
                await stream.FlushAsync().ConfigureAwait(false);
            }

            byte[] result = new byte[SizeIn+1];
            await stream.ReadAsync(result, 0, SizeIn+1).ConfigureAwait(false);
            return result.Skip(1).Take(SizeIn).ToArray(); ;
        }

        private byte[] ReadBytes(Stream source, int count)
        {
            var retVal = new byte[count];
            for (int i = 0; i < count; i++)
            {
                retVal[i] = (byte)source.ReadByte();
            }

            return retVal;
        }
    }
}
