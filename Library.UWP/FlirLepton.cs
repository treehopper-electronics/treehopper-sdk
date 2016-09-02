using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Diagnostics;
#if WINDOWS_UWP
using Windows.UI.Xaml.Media.Imaging;
#else
using System.Windows.Media;
using System.Windows.Media.Imaging;
#endif
namespace Treehopper.Libraries.Sensors.Optical
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 164)]
    public struct VoSPI
    {
        public ushort Id;
        public ushort Crc;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 80)]
        public ushort[] Payload;
    }
    public class FlirLepton
    {

        int height = 40;
        int width = 80;
        Spi spi;
        ushort[,] blackFrame;

        public FlirLepton(TreehopperUsb board)
        {
            spi = board.Spi;
            spi.Mode = SPIMode.Mode11;
            spi.Frequency = 12;
            spi.ChipSelect.DigitalValue = true;
            spi.Enabled = true;
            blackFrame = new ushort[height, width];
            for (int i=0;i<height;i++)
            {
                for (int j = 0; j < width; j++)
                    blackFrame[i,j] = (ushort)0xffff;
            }

        }

        public async Task<ushort[,]> GetRawFrame()
        {
            spi.ChipSelect.DigitalValue = false;
            await Task.Delay(185);
            spi.ChipSelect.DigitalValue = true;
            
            ushort[,] frame = new ushort[height, width];
            bool frameAcquired = false;
            bool syncAcquired = false;
            while (!frameAcquired)
            {
                syncAcquired = false;
                VoSPI packet = new VoSPI();
                while (!syncAcquired)
                {
                    packet = await GetPacket();
                    if ((packet.Id & 0x000f) != 0x000f)
                        syncAcquired = true;

                }
                for (int i = 0; i < height; i++)
                {
                    if ((packet.Id & 0x000f) == 0x000f)
                    {
                        //Debug.WriteLine("Lost sync on line "+i);
                        //Debug.WriteLine("PacketID: " + packet.Id);
                        break;
                    }
                        
                    for (int j = 0; j < width; j++)
                    {
                        frame[i, j] = packet.Payload[j];
                    }
                    if (i == height-1)
                        frameAcquired = true;
                    packet = await GetPacket();
                }
            }

            return frame;
        }

#if WINDOWS_UWP
        public async Task<WriteableBitmap> GetFrame()
        {
            WriteableBitmap frame  = new WriteableBitmap(width, height);
            var correctedFrame = await GetCorrectedRasterFrameArray();
            byte[] correctedArgbFrame = new byte[width * height * 4];
            for (int i=0;i<width*height;i++)
            {
                correctedArgbFrame[i * 4 + 0] = correctedFrame[i];
                correctedArgbFrame[i * 4 + 1] = correctedFrame[i];
                correctedArgbFrame[i * 4 + 2] = correctedFrame[i];
                correctedArgbFrame[i * 4 + 3] = 0xff;
            }
            await frame.PixelBuffer.AsStream().WriteAsync(correctedArgbFrame, 0, correctedArgbFrame.Length);
            return frame;
        }
#else
        public async Task<WriteableBitmap> GetFrame()
        {
            WriteableBitmap frame = new WriteableBitmap(80, 60, 300, 300, PixelFormats.Gray8, null);
            var correctedFrame = await GetCorrectedRasterFrameArray();
            frame.WritePixels(new Int32Rect(0, 0, 80, 60), correctedFrame, 1, 0);
            return frame;
        }
#endif
        public async Task<byte[]> GetCorrectedRasterFrameArray()
        {
            ushort[,] rawFrame = await GetRawFrame();
            ushort maxVal = 0;
            ushort minVal = ushort.MaxValue;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (rawFrame[i, j] > maxVal)
                        maxVal = rawFrame[i, j];
                    if (rawFrame[i, j] < minVal)
                        minVal = rawFrame[i, j];
                }
            }

            int range = maxVal - minVal;

            //Debug.WriteLine("max val: " + maxVal);
            //Debug.WriteLine("min val: " + minVal);

            double factor = 255.0 / range;

            byte[] correctedFrame = new byte[width * height];

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    correctedFrame[ i * width + j] = (byte)(rawFrame[i, j]/256);
                }
            }

            return correctedFrame;
        }

        async Task<VoSPI> GetPacket()
        {
            int rawsize = Marshal.SizeOf<VoSPI>();
            byte[] data = await spi.SendReceive(new byte[164]);
            IntPtr buffer = Marshal.AllocHGlobal(rawsize);
            Marshal.Copy(data, 0, buffer, rawsize);
            return Marshal.PtrToStructure<VoSPI>(buffer);
        }
    }

    public static class Crc16
    {
        const ushort polynomial = 0xA001;
        static readonly ushort[] table = new ushort[256];

        public static ushort ComputeChecksum(byte[] bytes)
        {
            ushort crc = 0;
            for (int i = 0; i < bytes.Length; ++i)
            {
                byte index = (byte)(crc ^ bytes[i]);
                crc = (ushort)((crc >> 8) ^ table[index]);
            }
            return crc;
        }

        static Crc16()
        {
            ushort value;
            ushort temp;
            for (ushort i = 0; i < table.Length; ++i)
            {
                value = 0;
                temp = i;
                for (byte j = 0; j < 8; ++j)
                {
                    if (((value ^ temp) & 0x0001) != 0)
                    {
                        value = (ushort)((value >> 1) ^ polynomial);
                    }
                    else
                    {
                        value >>= 1;
                    }
                    temp >>= 1;
                }
                table[i] = value;
            }
        }
    }
}
