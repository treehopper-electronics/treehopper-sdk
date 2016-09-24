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
    public class FirmwareUpdater
    {
        const int SizeIn = 4;
        const int SizeOut = 64;

        public event ProgressChangedEventHandler ProgressChanged;

        IFirmwareConnection connection;
        public FirmwareUpdater(IFirmwareConnection connection)
        {
            this.connection = connection;
        }

        public async Task<bool> ConnectAsync()
        {
            return await connection.OpenAsync();
        }

        public async Task<bool> Load(string path)
        {
            throw new NotImplementedException();
        }

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
                var reply = await SendFrame(frame);
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
                await connection.Write(buffer);
            }
            var res = await connection.Read(SizeIn);
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

        public async Task<bool> Load()
        {
            var stream = GetType().GetTypeInfo().Assembly.GetManifestResourceStream("Treehopper.treehopper.tfi");
            return await this.Load(stream);
        }
    }
}
