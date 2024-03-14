using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Motors
{
    public partial class Tmc5160 : StepperMotorController
    {
        private SpiDevice dev;
        private Tmc5160Registers registers;

        public override StepperMode Mode { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override MicrostepResolution Resolution { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override bool Enabled { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override int TargetPosition { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override int TargetVelocity { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override int ActualPosition { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override int ActualVelocity { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override int AccelerationLimit { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override double HoldingCurrent { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override double RunningCurrent { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override void SetZeroPosition()
        {
            throw new NotImplementedException();
        }

        public Tmc5160(Spi spi, SpiChipSelectPin chipSelect)
        {
            dev = new SpiDevice(spi, chipSelect);
            registers = new Tmc5160Registers(new Tmc5160SpiRegisterManagerAdapter(dev));

            /* Clear the reset and charge pump undervoltage flags */
            registers.gstat.reset = 1;
            registers.gstat.uv_cp = 1;
            registers.gstat.writeAsync().Wait();
        }

        internal class Tmc5160SpiRegisterManagerAdapter : IRegisterManagerAdapter
        {
            protected SpiDevice _dev;

            public Tmc5160SpiRegisterManagerAdapter(SpiDevice dev)
            {
                _dev = dev;
            }

            public Task<byte[]> read(int address, int width)
            {
                var datagram = new byte[width+1];
                datagram[0] = (byte)(address & ~0x08);
                return _dev.SendReceiveAsync(datagram, SpiBurstMode.BurstRx);
            }

            public Task write(int address, byte[] data)
            {
                byte[] datagram = new byte[data.Length + 1];
                datagram[0] = (byte)(address | 0x08);
                data.CopyTo(datagram, 1);
                return _dev.SendReceiveAsync(data, SpiBurstMode.BurstTx);
            }
        }
    }
}
