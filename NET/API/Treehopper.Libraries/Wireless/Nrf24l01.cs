using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper.Utilities;

namespace Treehopper.Libraries.Wireless
{
    public class Nrf24l01
    {
        public enum CrcMode
        {
            CrcDisabled,
            Crc8Bit,
            Crc16Bit
        }

        public enum DataRate
        {
            Rate_1mbps,
            Rate_500kbps,
            Rate_250kbps
        }

        public enum PowerLevel
        {
            Min,
            Low,
            High,
            Max
        }

        public class PacketReceivedEventArgs : EventArgs
        {
            public byte[] Data { get; set; }
        }

        public delegate void PacketReceivedEventHandler(object sender, PacketReceivedEventArgs e);

        SpiDevice dev;
        DigitalIn irqPin;
        DigitalOut cePin;

        public Nrf24l01(Spi spi, SpiChipSelectPin csPin, DigitalOut cePin, DigitalIn irqPin)
        {
            dev = new SpiDevice(spi, csPin);
            this.irqPin = irqPin;
            this.cePin = cePin;
            Task.Run(irqPin.MakeDigitalInAsync).Wait();
            Task.Run(cePin.MakeDigitalPushPullOutAsync).Wait();
            cePin.DigitalValue = false;
            irqPin.DigitalValueChanged += IrqPin_DigitalValueChanged;
            Task.Run(writeConfig).Wait();
            Task.Run(() => dev.SendReceiveAsync(new byte[] {0x50, 0x73 })).Wait();
            Task.Run(() => WriteRegister((byte)Registers.Feature, 0x03)).Wait();
        }

        private void IrqPin_DigitalValueChanged(object sender, DigitalInValueChangedEventArgs e)
        {
            Debug.WriteLine("IRQ");
        }

        private int retryCount;

        public int RetryCount
        {
            get { return retryCount; }
            set { retryCount = value; }
        }

        private int retryDelay;

        public int RetryDelay
        {
            get { return retryDelay; }
            set { retryDelay = value; }
        }

        private CrcMode crc = CrcMode.Crc8Bit;

        public CrcMode Crc
        {
            get { return crc; }
            set {
                crc = value;
                Task.Run(writeConfig).Wait();
            }
        }

        private Task writeConfig()
        {
            var config = 0;
            switch(Crc)
            {
                case CrcMode.CrcDisabled:
                    config = 0b0000;
                    break;
                case CrcMode.Crc8Bit:
                    config = 0b1000;
                    break;
                case CrcMode.Crc16Bit:
                    config = 0b1100;
                    break;
            }

            config |= 0b10; // PWR_UP

            return WriteRegister((byte)Registers.NrfConfig, (byte)config);
        }

        private bool autoAckEnabled;

        public bool AutoAckEnabled
        {
            get { return autoAckEnabled; }
            set { autoAckEnabled = value; }
        }

        private int channel = 76; // this is the default power-up channel

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

        private int addressWidth = 5;

        public int AddressWidth
        {
            get { return addressWidth; }
            set {
                if(value != 3 && value != 4 && value != 5)
                {
                    Utility.Error("AddressWidth must be of value 3, 4, or 5.");
                }

                addressWidth = value;
                Task.Run(() => WriteRegister((byte)Registers.SetupAddressWidth, (byte)(addressWidth - 2))).Wait();
            }
        }

        private long address = 0xE7E7E7E7E7; // default address, per NRF datasheet

        public long Address
        {
            get { return address; }
            set { address = value; }
        }


        /// <summary>
        /// Transmit a packet to the specified receiver slot
        /// </summary>
        /// <param name="data">The data to send</param>
        /// <param name="requestAck">Whether the receiver should ACK</param>
        /// <returns></returns>
        public async Task Transmit(byte[] data, bool requestAck=true)
        {

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
