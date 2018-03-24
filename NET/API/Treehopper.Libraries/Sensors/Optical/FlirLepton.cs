using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Sensors.Optical
{
    /// <summary>
    ///     FLIR Lepton
    /// </summary>
    [Supports("FLIR", "Lepton")]
    public class FlirLepton
    {
        private readonly ushort[,] blackFrame;

        private readonly int height = 60;
        private readonly int width = 80;
        private SpiDevice dev;
        SpiChipSelectPin cs;
        /// <summary>
        ///     Construct a FLIR Lepton
        /// </summary>
        /// <param name="spi">The Spi module to use</param>
        /// <param name="cs">The chip-select pin</param>
        public FlirLepton(Spi spi, SpiChipSelectPin cs)
        {
            this.dev = new SpiDevice(spi, cs, ChipSelectMode.SpiActiveLow, 6, SpiMode.Mode11);
            this.cs = cs;

            blackFrame = new ushort[height, width];
            for (var i = 0; i < height; i++)
            for (var j = 0; j < width; j++)
                blackFrame[i, j] = 0xffff;
        }

        /// <summary>
        ///     Get the raw frame from the sensor
        /// </summary>
        /// <param name="timeoutFrame">Whether to timeout the device.</param>
        /// <param name="minimumHeight">Only return frames with the minimum-specified height.</param>
        /// <returns>An awaitable 2D-array of values</returns>
        public async Task<ushort[,]> GetRawFrameAsync(bool timeoutFrame = false, int minimumHeight = 20)
        {
            if(timeoutFrame)
            {
                cs.DigitalValue = false;
                await Task.Delay(185).ConfigureAwait(false);
                cs.DigitalValue = true;
            }
            
            var frame = new ushort[height, width];
            var frameAcquired = false;
            var syncAcquired = false;
            while (!frameAcquired)
            {
                syncAcquired = false;
                ushort[] packet = new ushort[1];
                while (!syncAcquired)
                {
                    packet = await GetPacketAsync().ConfigureAwait(false);
                    if ((packet[0] & 0x000f) != 0x000f) // check ID
                    {
                        syncAcquired = true;
                    }
                        
                }
                for (var i = 0; i < height; i++)
                {
                    if (packet[0] == 0 && packet[1] == 0 && packet[2] == 0 && packet[3] == 0)
                        frameAcquired = true;
                    if ((packet[0] & 0x000f) == 0x000f)
                    {
                        // lost sync
                        if(i >= minimumHeight)
                            frameAcquired = true;
                        break;
                    }

                    for (var j = 0; j < width; j++)
                        frame[i, j] = packet[j+2];
                    if (i == height - 1)
                    {
                        frameAcquired = true;
                    }
                        
                    packet = await GetPacketAsync().ConfigureAwait(false);
                }
            }

            return frame;
        }

        /// <summary>
        ///     Get the corrected raster frame
        /// </summary>
        /// <returns>A byte[] raster</returns>
        public async Task<byte[]> GetCorrectedRasterFrameArrayAsync()
        {
            var rawFrame = await GetRawFrameAsync();
            ushort maxVal = 0;
            var minVal = ushort.MaxValue;
            for (var i = 0; i < height; i++)
            for (var j = 0; j < width; j++)
            {
                if (rawFrame[i, j] > maxVal)
                    maxVal = rawFrame[i, j];
                if (rawFrame[i, j] < minVal)
                    minVal = rawFrame[i, j];
            }

            var range = maxVal - minVal;

            //Debug.WriteLine("max val: " + maxVal);
            //Debug.WriteLine("min val: " + minVal);

            var factor = 255.0 / range;

            var correctedFrame = new byte[width * height];

            for (var i = 0; i < height; i++)
            for (var j = 0; j < width; j++)
                correctedFrame[i * width + j] = (byte) (rawFrame[i, j] / 256);

            return correctedFrame;
        }

        private async Task<ushort[]> GetPacketAsync()
        {
            var data = await dev.SendReceiveAsync(new byte[164], SpiBurstMode.BurstRx).ConfigureAwait(false);
            ushort[] packet = new ushort[82];
            
            Buffer.BlockCopy(data, 0, packet, 0, 164);
            return packet;
        }
    }
}