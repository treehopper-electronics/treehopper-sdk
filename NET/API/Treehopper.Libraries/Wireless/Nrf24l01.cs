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
        public class Pipe
        {
            public Pipe(int index, Nrf24l01 rf)
            {
                this.index = index;
                this.rf = rf;
            }

            private bool dynamicPayloadEnabled;
            private int index;
            private Nrf24l01 rf;

            public bool DynamicPayloadEnabled
            {
                get { return dynamicPayloadEnabled; }
                set {
                    dynamicPayloadEnabled = value;
                }
            }

            internal long address;

            public long Address
            {
                get { return address; }
                set {
                    address = value;
                    var bytes = BitConverter.GetBytes(address);
                    Task.Run(() => rf.WriteRegister((byte)((byte)(Registers.RxAddrPipe0)+index), bytes.Take(5).ToArray())).Wait();
                }
            }
        }

        public enum CrcMode
        {
            CrcDisabled,
            Crc8Bit,
            Crc16Bit
        }

        public enum DataRate
        {
            Rate_1mbps,
            Rate_2mbps,
            Rate_250kbps
        }

        public enum PowerLevel
        {
            Min,
            Low,
            High,
            Max
        }

        private PowerLevel power;

        public PowerLevel Power
        {
            get { return power; }
            set { power = value; Task.Run(updateSetupRegister).Wait(); }
        }


        private DataRate rate = DataRate.Rate_2mbps;

        public DataRate Rate
        {
            get { return rate; }
            set { rate = value; Task.Run(updateSetupRegister).Wait(); }
        }

        private Task updateSetupRegister()
        {
            var rfPwr = (int)power;
            var rfDr = 0;
            switch(rate)
            {
                case DataRate.Rate_1mbps:
                    rfDr = 0b000;
                    break;
                case DataRate.Rate_2mbps:
                    rfDr = 0b001;
                    break;
                case DataRate.Rate_250kbps:
                    rfDr = 0b100;
                    break;
            }

            var regVal = rfDr << 3 | rfPwr << 1 | 1;
            return WriteRegister((byte)Registers.RfSetup, (byte)regVal);
        }

        private bool rxEnable;

        public bool RxEnable
        {
            get { return rxEnable; }
            set {
                rxEnable = value;

            }
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

            Task.Run(() => dev.SendReceiveAsync(new byte[] { 0x50, 0x73 })).Wait(); // enable feature register
            Task.Run(() => WriteRegister((byte)Registers.Feature, 0b00000111)).Wait();
            Task.Run(() => WriteRegister((byte)Registers.DynamicPayloadLength, 0b00111111)).Wait();
            Task.Run(FlushRx).Wait();
            Task.Run(FlushTx).Wait();

            Task.Run(writeConfig).Wait();

            for (int i = 0; i < 6; i++)
            {
                pipes[i] = new Pipe(i, this);
            }
            // set default addresses
            pipes[0].address = 0xE7E7E7E7E7;
            pipes[1].address = 0xC2C2C2C2C2;
            pipes[2].address = 0xC2C2C2C2C3;
            pipes[3].address = 0xC2C2C2C2C4;
            pipes[4].address = 0xC2C2C2C2C5;
            pipes[5].address = 0xC2C2C2C2C6;

            Task.Run(() => Task.Delay(5)).Wait();
        }

        private Pipe[] pipes = new Pipe[6];

        public Pipe[] Pipes => pipes;

        public Pipe this[int i]
        {
            get
            {
                return pipes[i];
            }
        }

        private async void IrqPin_DigitalValueChanged(object sender, DigitalInValueChangedEventArgs e)
        {
            Debug.WriteLine("IRQ");
            var data = new byte[] { (byte)((byte)Registers.Status | 0x20), 0b01110000 };
            var bytes = await dev.SendReceiveAsync(data);
            DebugStatus(bytes[0]);
        }

        private int retryCount = 3;

        public int RetryCount
        {
            get { return retryCount; }
            set {
                retryCount = value;
                Task.Run(updateAutoRetransmission).Wait();
            }
        }

        private int retryDelayUs = 250;

        public int RetryDelayUs
        {
            get { return retryDelayUs; }
            set {
                retryDelayUs = value;
                Task.Run(updateAutoRetransmission).Wait();
            }
        }

        private Task updateAutoRetransmission()
        {
            var ard = Numbers.Constrain((int)(retryDelayUs / 250.0) - 1, 0, 15);
            var arc = Numbers.Constrain(retryCount, 0, 15);
            var regVal = (ard << 4) | arc;
            return WriteRegister((byte)Registers.SetupAutomaticRetransmission, (byte)regVal);
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

        private bool enhancedShockburstEnabled = false;

        public bool EnhancedShockburstEnabled
        {
            get { return enhancedShockburstEnabled; }
            set { enhancedShockburstEnabled = value; }
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

        private bool ackPayloadEnabled;
        public bool AckPayloadEnabled
        {
            get { return ackPayloadEnabled;}
            set { ackPayloadEnabled = value;}
        }
        
        private byte[] ackPayload;
        public byte[] AckPayload
        {
            get { return ackPayload;}
            set { ackPayload = value;}
        }

        private int channel = 2; // this is the default power-up channel

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

        private long txAddress = 0xE7E7E7E7E7; // default address, per NRF datasheet

        public long TxAddress
        {
            get { return txAddress; }
            set {
                txAddress = value;
                var bytes = BitConverter.GetBytes(TxAddress);
                Task.Run(() => WriteRegister((byte)Registers.TxAddr, bytes.Take(5).ToArray())).Wait();
            }
        }

        public Task FlushRx()
        {
            return dev.SendReceiveAsync(new byte[] { 0b11100010 });
        }

        public Task FlushTx()
        {
            return dev.SendReceiveAsync(new byte[] { 0b11100001 });
        }

        /// <summary>
        /// Transmit a packet to the specified receiver slot
        /// </summary>
        /// <param name="data">The data to send</param>
        /// <param name="requestAck">Whether the receiver should ACK</param>
        /// <returns></returns>
        public async Task Transmit(byte[] data, bool requestAck=true)
        {
            var dataToSend = new byte[] { 0xA0 }.Concat(data).ToArray();
            await dev.SendReceiveAsync(dataToSend);
            cePin.DigitalValue = true;
            cePin.DigitalValue = false;
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
            bytes[0] = register;
            for (int i = 0; i < numBytesToRead; i++)
                bytes[i + 1] = 0xff;

            var result = await dev.SendReceiveAsync(bytes).ConfigureAwait(false);
            return result.Skip(1).Take(numBytesToRead).ToArray();
        }

        private async Task WriteRegister(byte register, byte value)
        {
            var data = new byte[] { (byte)(register | 0x20), value };
            await dev.SendReceiveAsync(data, SpiBurstMode.BurstTx);
            //await Task.Delay(100);
            //var result = await ReadRegister(register);
            //if (result != value)
            //{
            //    Debug.WriteLine("ERROR: Register read-back failed");
            //}
        }

        private Task WriteRegister(byte register, byte[] data)
        {
            var dataToSend = new byte[] { (byte)(register | 0x20) }.Concat(data).ToArray();
            return dev.SendReceiveAsync(dataToSend, SpiBurstMode.BurstTx);
        }

        public Task<byte> GetStatus()
        {
            return ReadRegister((byte)Registers.Status);
        }

        public void DebugStatus(byte status)
        {
            if((status & 0x01) == 0x01)
            {
                Debug.WriteLine("TX_FULL");
            }
            if ((status >> 4 & 0x01) == 0x01)
            {
                Debug.WriteLine("MAX_RT");
            }
            if ((status >> 5 & 0x01) == 0x01)
            {
                Debug.WriteLine("TX_DS");
            }
            if ((status >> 6 & 0x01) == 0x01)
            {
                Debug.WriteLine("RX_DR");
            }

            if(((status >> 1) & 0b111) == 0b111)
                Debug.WriteLine("No bytes received");
        }

        public async Task DebugDetails()
        {
            Debug.WriteLine("STATUS: ");
            DebugStatus(await GetStatus());

            Debug.WriteLine("Config: {0}", await ReadRegister((byte)Registers.NrfConfig));
            Debug.WriteLine("Enhanced Shockburst: {0}", await ReadRegister((byte)Registers.EnableAutoAcknowledgment));
            Debug.WriteLine("Enabled RX addresses: {0}", await ReadRegister((byte)Registers.EnableRxAddresses));
            Debug.WriteLine("Address widths: {0}", await ReadRegister((byte)Registers.SetupAddressWidth));
            Debug.WriteLine("Retransmission: {0}", await ReadRegister((byte)Registers.SetupAutomaticRetransmission));
            Debug.WriteLine("RF Channel: {0}", await ReadRegister((byte)Registers.RfChannel));
            Debug.WriteLine("RF Setup: {0}", await ReadRegister((byte)Registers.RfSetup));
            Debug.WriteLine("TX Address: " + (await ReadRegister((byte)Registers.TxAddr, 5)).ToHexString());
            Debug.WriteLine("RX0 Address: " + (await ReadRegister((byte)Registers.RxAddrPipe0, 5)).ToHexString());
            Debug.WriteLine("RX1 Address: " + (await ReadRegister((byte)Registers.RxAddrPipe1, 5)).ToHexString());
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
        DynamicPayloadLength = 0x1C,
        Feature = 0x1D
    }
}
