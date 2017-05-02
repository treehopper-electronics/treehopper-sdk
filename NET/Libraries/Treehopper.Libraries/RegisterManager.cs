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

        public RegisterManager(SMBusDevice dev)
        {
            _dev = dev;
        }

        public async Task Read(Register register)
        {
            register.SetBytes(await _dev.ReadBufferData((byte) register.Address, register.Width).ConfigureAwait(false));
        }

        public async Task ReadRange(Register start, Register end)
        {
            var count = (end.Address + end.Width) - start.Address;
            var bytes = await _dev.ReadBufferData((byte) start.Address, count).ConfigureAwait(false);
            int i = 0;
            foreach (var reg in _registers.Where(reg => reg.Address >= start.Address && reg.Address <= end.Address))
            {
                reg.SetBytes(bytes.Skip(i).Take(reg.Width).ToArray());
                i += reg.Width;
            }
        }

        public Task Write(Register register)
        {
            return _dev.WriteBufferData((byte) register.Address, register.GetBytes());
        }

        public Task WriteRange(Register start, Register end)
        {
            List<byte> bytes = new List<byte>();
            foreach (var reg in _registers.Where(reg => reg.Address >= start.Address && reg.Address <= end.Address))
            {
                bytes.AddRange(reg.GetBytes());
            }
            return _dev.WriteBufferData((byte) start.Address, bytes.ToArray());
        }
    }
}
