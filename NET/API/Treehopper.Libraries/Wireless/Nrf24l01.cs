using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper.Utilities;

namespace Treehopper.Libraries.Wireless
{
    public class Nrf24l01
    {
        SpiDevice dev;
        DigitalIn irqPin;
        DigitalIn cePin;

        public Nrf24l01(Spi spi, SpiChipSelectPin csPin, DigitalIn irqPin, DigitalIn cePin)
        {
            dev = new SpiDevice(spi, csPin);
            this.irqPin = irqPin;
            this.cePin = cePin;

            // go into RX mode by default
            Task.Run(() => WriteRegister((byte)Registers.NrfConfig, (byte)channel)).Wait();
        }

        private int channel = 0;

        public int Channel
        {
            get { return channel; }
            set {
                channel = value;
                if (channel < 0 || channel >= 125)
                {
                    Utility.Error("Channel must be between 0 and 125, and will be clipped");
                    channel = Numbers.Constrain(channel, 0, 125);
                }
                    
                Task.Run(() => WriteRegister((byte)Registers.RfChannel, (byte)channel)).Wait();
            }
        }



        private async Task<byte> ReadRegister(byte register)
        {
            var bytes = new byte[] { register, 0xFF };
            var result = await dev.SendReceiveAsync(bytes).ConfigureAwait(false);
            return result[1];
        }

        private async Task<byte[]> ReadRegister(byte register, int numBytesToRead)
        {
            var bytes = new byte[numBytesToRead + 1];
            for (int i = 0; i < numBytesToRead; i++)
                bytes[i + 1] = 0xff;

            var result = await dev.SendReceiveAsync(bytes).ConfigureAwait(false);
            return result.Skip(1).Take(numBytesToRead).ToArray();
        }

        private async Task WriteRegister(byte register, byte value)
        {
            var data = new byte[] { (byte)(register | 0x20), value };
            await dev.SendReceiveAsync(data, SpiBurstMode.BurstTx);
        }

        private async Task WriteRegister(byte register, byte[] data)
        {
            var dataToSend = new byte[] { (byte)(register | 0x20) }.Concat(data).ToArray();
            await dev.SendReceiveAsync(dataToSend, SpiBurstMode.BurstTx);
        }
    }

    enum Registers
    {
        NrfConfig,
        EnableAutoAcknowledgment,
        EnableRxAddresses,
        SetupAddressWidth,
        SetupAutomaticRetransmission,
        RfChannel,
        RfSetup,
        Status,
        ObserveTx,
        Cd,
        RxAddrPipe0,
        RxAddrPipe1,
        RxAddrPipe2,
        RxAddrPipe3,
        RxAddrPipe4,
        RxAddrPipe5,
        TxAddr,
        RxPwP0,
        RxPwP1,
        RxPwP2,
        RxPwP3,
        RxPwP4,
        RxPwP5,
        FifoStatus,
        AckPld,
        TxPld,
        RxPld,
        DynamicPayloadLength,
        Feature
    }
}
