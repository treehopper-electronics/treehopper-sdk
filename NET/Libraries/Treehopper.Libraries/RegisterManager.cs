using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries
{
    public class RegisterManager
    {
        protected List<Register> _registers = new List<Register>();
        protected SMBusDevice _dev;
        protected bool multiRegisterAccess;

        public RegisterManager(SMBusDevice dev, bool multiRegisterAccess)
        {
            _dev = dev;
            this.multiRegisterAccess = multiRegisterAccess;
        }

        public async Task Read(Register register)
        {
            var data = await _dev.ReadBufferData((byte) register.Address, register.Width).ConfigureAwait(false);
            register.SetBytes(data);
        }

        public async Task ReadRange(Register start, Register end)
        {
            if (multiRegisterAccess)
            {
                var count = (end.Address + end.Width) - start.Address;
                var bytes = await _dev.ReadBufferData((byte)start.Address, count).ConfigureAwait(false);
                int i = 0;
                foreach (var reg in _registers.Where(reg => reg.Address >= start.Address && reg.Address <= end.Address))
                {
                    reg.SetBytes(bytes.Skip(i).Take(reg.Width).ToArray());
                    i += reg.Width;
                }
            }
            else
            {
                foreach (var reg in _registers.Where(reg => reg.Address >= start.Address && reg.Address <= end.Address))
                {
                    await Read(reg);
                }
            }

        }

        public Task Write(Register register)
        {
            return _dev.WriteBufferData((byte) register.Address, register.GetBytes());
        }

        public async Task WriteRange(Register start, Register end)
        {
            if (multiRegisterAccess)
            {
                List<byte> bytes = new List<byte>();
                foreach (var reg in _registers.Where(reg => reg.Address >= start.Address && reg.Address <= end.Address))
                {
                    bytes.AddRange(reg.GetBytes());
                }
                await _dev.WriteBufferData((byte) start.Address, bytes.ToArray()).ConfigureAwait(false);
            }
            else
            {
                foreach (var reg in _registers.Where(reg => reg.Address >= start.Address && reg.Address <= end.Address))
                {
                    await Write(reg).ConfigureAwait(false);
                }
            }



        }
    }
}
